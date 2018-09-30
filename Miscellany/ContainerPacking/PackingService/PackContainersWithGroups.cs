using System;
using System.Collections.Generic;
using System.Linq;
using CromulentBisgetti.ContainerPacking.Entities; //added to interact with ContainerPacking
using Autodesk.DesignScript.Runtime;

namespace Miscellany.ContainerPacking
{
    /// <summary>
    /// PackingService
    /// </summary>
    public static partial class PackingService
    {

        #region PackContainersWithGroups

        /// <summary>
        /// Runs the chosen packing algorithm as a greedy strategy on a list of containers and groups of items to pack. When a group of items is exhausted the algorithm moves on to the next container and starts a new group. It aims to solve optimally at each container in turn and is not globally optimal. The default algorithm (1) is EB-AFIT from David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="containers">Containers in order</param>
        /// <param name="itemsToPack">Lists of Items to pack</param>
        /// <param name="algorithm">Algorithm ID</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <returns name="isCompletePack">Are all items packed?</returns>
        /// <returns name="packTimeInMilliseconds">Total pack time</returns>
        /// <returns name="totalPackTimeInMilliseconds">Pack time per container</returns>
        /// <returns name="percentContainerVolumePacked">Percentage of the container that is packed</returns>
        /// <returns name="percentItemVolumePacked">Percentage of items packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems", "isCompletePack", "packTimeInMilliseconds", "totalPackTimeInMilliseconds", "percentContainerVolumePacked", "percentItemVolumePacked" })]
        public static Dictionary<string, object> PackContainersWithGroups(List<Miscellany.ContainerPacking.Entities.Container> containers, List<List<Miscellany.ContainerPacking.Entities.Item>> itemsToPack, int algorithm = 1)
        {
            //Select algorithm using integer
            List<int> algorithms = new List<int> { algorithm };
            //Create CromulentBisgetti Items
            List<List<Item>> items = new List<List<Item>>();
            foreach (List<Miscellany.ContainerPacking.Entities.Item> l in itemsToPack)
            {
                List<Item> subList = new List<Item>();
                foreach (Miscellany.ContainerPacking.Entities.Item i in l)
                {
                    decimal ddim1 = Miscellany.Math.Functions.ToDecimal(i.Dim1);
                    decimal ddim2 = Miscellany.Math.Functions.ToDecimal(i.Dim2);
                    decimal ddim3 = Miscellany.Math.Functions.ToDecimal(i.Dim3);
                    Item cbItem = new Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                    subList.Add(cbItem);
                }
                items.Add(subList);
            }

            //Reverse List to start from back so removals don't cause problems to indexing
            items.Reverse();

            //Output lists
            List<List<Miscellany.ContainerPacking.Entities.Item>> itemsPacked = new List<List<Miscellany.ContainerPacking.Entities.Item>>();
            bool IsCompletePack = false;
            List<int> PackTimeInMilliseconds = new List<int>();
            int TotalPackTimeInMilliseconds = 0;
            List<double> PercentContainerVolumePacked = new List<double>();
            List<double> PercentItemVolumePacked = new List<double>();

            //Items Count
            int currentPackGroup = items.Count - 1;

            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                if (items.Count == 0)
                {
                    break;
                }
                if (items[currentPackGroup].Count == 0)
                {
                    items.RemoveAt(currentPackGroup); //Remove empty list
                    currentPackGroup--; //move to next group
                }

                //Create list of items to pack
                List<Item> itemsToPackGroup = items[currentPackGroup];

                //Create CromulentBisgetti Container
                decimal dLength = Miscellany.Math.Functions.ToDecimal(container.Length);
                decimal dWidth = Miscellany.Math.Functions.ToDecimal(container.Width);
                decimal dHeight = Miscellany.Math.Functions.ToDecimal(container.Height);
                Container con = new Container(container.ID, dLength, dWidth, dHeight);
                List<Container> cons = new List<Container> { con };

                //Get container packing result
                ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(cons, itemsToPackGroup, algorithms).FirstOrDefault();

                //Get the single algorthim packing result from the container packing result
                AlgorithmPackingResult algorithmPackingResult = containerPackingResult.AlgorithmPackingResults.FirstOrDefault();

                //Packed Items
                List<Miscellany.ContainerPacking.Entities.Item> itemsPackedPass = new List<Miscellany.ContainerPacking.Entities.Item>();
                foreach (Item i in algorithmPackingResult.PackedItems)
                {
                    Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                    itemsPackedPass.Add(mItem);
                }
                itemsPacked.Add(itemsPackedPass);
                IsCompletePack = algorithmPackingResult.IsCompletePack;
                if (IsCompletePack) //If all the items from that group are packed
                {
                    items.RemoveAt(currentPackGroup); //Remove group from list to pack
                    currentPackGroup--; //Move on to the next group
                }
                else
                {
                    items[currentPackGroup] = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                }
                PackTimeInMilliseconds.Add(Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds));
                TotalPackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
                PercentContainerVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentContainerVolumePacked));
                PercentItemVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentItemVolumePacked));
                if (items.Count == 0) //No more groups to pack
                {
                    break;
                }
            }

            //Convert CromulentBisgetti items to Miscellany Items for Unpacked Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (List<Item> l in items)
            {
                foreach (Item i in l)
                {
                    Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                    itemsUnpacked.Add(mItem);
                }
            }

            //Return values
            var d = new Dictionary<string, object>();
            d.Add("packedItems", itemsPacked);
            d.Add("unpackedItems", itemsUnpacked);
            d.Add("isCompletePack", IsCompletePack);
            d.Add("packTimeInMilliseconds", PackTimeInMilliseconds);
            d.Add("totalPackTimeInMilliseconds", TotalPackTimeInMilliseconds);
            d.Add("percentContainerVolumePacked", PercentContainerVolumePacked);
            d.Add("percentItemVolumePacked", PercentItemVolumePacked);
            return d;
        }

        #endregion

    }
}
