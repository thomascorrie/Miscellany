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

        #region PackContainersWithGroupsContinuously

        /// <summary>
        /// Runs the chosen packing algorithm as a greedy strategy on a list of containers and a list of items to pack. When the number of Items in a group dwindles below the minimum number, the difference is made up with Items taken from the next group. It aims to solve optimally at each container in turn and is not globally optimal. The default algorithm (1) is EB-AFIT from David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="containers">Containers in order</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <param name="algorithm">Algorithm ID</param>
        /// <param name="minimumItems">The minimum number of items to consider for a container</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <returns name="isCompletePack">Are all items packed?</returns>
        /// <returns name="packTimeInMilliseconds">Pack time per container</returns>
        /// <returns name="totalPackTimeInMilliseconds">Total pack time</returns>
        /// <returns name="percentContainerVolumePacked">Percentage of the container that is packed</returns>
        /// <returns name="percentItemVolumePacked">Percentage of items packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems", "isCompletePack", "packTimeInMilliseconds", "totalPackTimeInMilliseconds", "percentContainerVolumePacked", "percentItemVolumePacked" })]
        public static Dictionary<string, object> PackContainersWithGroupsContinuously(List<Miscellany.ContainerPacking.Entities.Container> containers, List<List<Miscellany.ContainerPacking.Entities.Item>> itemsToPack, int algorithm = 1, int minimumItems = 20)
        {
            //Select algorithm using integer
            List<int> algorithms = new List<int> { algorithm };

            //Create CromulentBisgetti Items, retaining the nested structure
            List<List<Item>> items = new List<List<Item>>();
            foreach (List<Miscellany.ContainerPacking.Entities.Item> l in itemsToPack)
            {
                List<Item> subList = new List<Item>();
                foreach (Miscellany.ContainerPacking.Entities.Item i in l)
                {
                    Item cbItem = ItemToCB(i);
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

            //At least one Item must be considered per container
            if (minimumItems < 1)
            {
                minimumItems = 1;
            }

            //Sequence for packed Items
            int seq = 1;

            //Temporary group of Items to pack
            List<Item> tempItemsToPack = new List<Item>();

            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                //Top up the temporary group from the next groups until it reaches the minimum or no Items are left
                while (tempItemsToPack.Count < minimumItems && items.Count > 0)
                {
                    int currentPackGroup = items.Count - 1; //Groups are reversed so the next group is the last one
                    List<Item> group = items[currentPackGroup];
                    int numberToTake = minimumItems - tempItemsToPack.Count;
                    if (group.Count <= numberToTake)
                    {
                        //Fewer Items are available than desired so take the whole group (this also removes empty groups)
                        tempItemsToPack.AddRange(group);
                        items.RemoveAt(currentPackGroup);
                    }
                    else
                    {
                        //More Items are available than desired so take the first ones in the group
                        tempItemsToPack.AddRange(group.GetRange(0, numberToTake));
                        group.RemoveRange(0, numberToTake);
                    }
                }

                //No Items left so break the loop
                if (tempItemsToPack.Count == 0)
                {
                    break;
                }

                //Create CromulentBisgetti Container
                Container con = ContainerToCB(container);
                List<Container> cons = new List<Container> { con };

                //Get container packing result
                ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(cons, tempItemsToPack, algorithms).FirstOrDefault();

                //Get the single algorthim packing result from the container packing result
                AlgorithmPackingResult algorithmPackingResult = AsSingleUnits(containerPackingResult.AlgorithmPackingResults.FirstOrDefault());

                //Packed Items
                List<Miscellany.ContainerPacking.Entities.Item> itemsPackedPass = new List<Miscellany.ContainerPacking.Entities.Item>();
                foreach (Item i in algorithmPackingResult.PackedItems)
                {
                    Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                    mItem.Sequence = seq;
                    seq++;
                    itemsPackedPass.Add(mItem);
                }
                itemsPacked.Add(itemsPackedPass);
                if (algorithmPackingResult.IsCompletePack) //If all the items from that group are packed
                {
                    tempItemsToPack.Clear(); //Clear all Items from temp list
                }
                else
                {
                    tempItemsToPack = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                }
                PackTimeInMilliseconds.Add(Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds));
                TotalPackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
                PercentContainerVolumePacked.Add(Miscellany.Maths.ToDouble(algorithmPackingResult.PercentContainerVolumePacked));
                PercentItemVolumePacked.Add(Miscellany.Maths.ToDouble(algorithmPackingResult.PercentItemVolumePacked));
            }

            //Complete only if nothing is left in the temporary group or any remaining group
            IsCompletePack = tempItemsToPack.Count == 0 && items.All(l => l.Count == 0);

            //Convert CromulentBisgetti items to Miscellany Items for Unpacked Items
            //Items still in the temporary group come first, then the remaining groups in their original order
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (Item i in tempItemsToPack)
            {
                Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                itemsUnpacked.Add(mItem);
            }
            for (int g = items.Count - 1; g >= 0; g--)
            {
                foreach (Item i in items[g])
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
