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
        /// <returns name="packTimeInMilliseconds">Total pack time</returns>
        /// <returns name="totalPackTimeInMilliseconds">Pack time per container</returns>
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

            //Items Count
            int currentPackGroup = items.Count - 1; //Set to last index

            //Temporary group of Items to pack
            List<Item> tempItemsToPack = new List<Item>();

            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                //How many Items in temp list?
                int tempCount = tempItemsToPack.Count;
                
                //Ensure count matches the number of groups remaining
                currentPackGroup = items.Count - 1;

                if (tempCount >= minimumItems)
                {
                    //There are more Items than the minimum or the same
                }
                else if (currentPackGroup < 0 || (currentPackGroup == 0 && items[currentPackGroup].Count == 0))
                {
                    //There are fewer Items than the minimum but there are no more Items to draw on
                    if (tempCount == 0)
                    {
                        //No Items left so break the loop
                        break;
                    }
                }
                else
                {
                    //There are fewer Items than the minimum but there are further Items to use
                    while (tempCount < minimumItems)
                    {
                        if (items[currentPackGroup].Count == 0)
                        {
                            //No items left, move onto next group for next loop
                            items.RemoveAt(currentPackGroup);
                            currentPackGroup--;
                        }
                        else if (items[currentPackGroup].Count < minimumItems - tempCount)
                        {
                            //Fewer Items are available than desired
                            tempItemsToPack.AddRange(items[currentPackGroup]); //Add all Items
                            items.RemoveAt(currentPackGroup); //Remove all items from the group
                            currentPackGroup--; //Move to the next group index for the next loop
                            tempCount = tempItemsToPack.Count; //Set the count
                        }
                        else
                        {
                            //The desired number or greater are available
                            int numberToTake = minimumItems - tempCount;
                            for (int i = numberToTake - 1; i >= 0; i--)
                            {
                                //Take each required Item and remove from list
                                tempItemsToPack.Add(items[currentPackGroup][i]);
                                items[currentPackGroup].RemoveAt(i);
                            }
                            tempCount = tempItemsToPack.Count; //Report the current size of the temporary group
                        }
                    }
                }
                //Create CromulentBisgetti Container
                Container con = ContainerToCB(container);
                List<Container> cons = new List<Container> { con };

                //Get container packing result
                ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(cons, tempItemsToPack, algorithms).FirstOrDefault();

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
                    tempItemsToPack.Clear(); //Clear all Items from temp list
                }
                else
                {
                    tempItemsToPack = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                }
                PackTimeInMilliseconds.Add(Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds));
                TotalPackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
                PercentContainerVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentContainerVolumePacked));
                PercentItemVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentItemVolumePacked));
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
