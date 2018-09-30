﻿using System;
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
        /// <param name="minimumBooks">The minimum number of items to consider for a container</param>
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
        public static Dictionary<string, object> PackContainersWithGroupsContinuously(List<Miscellany.ContainerPacking.Entities.Container> containers, List<List<Miscellany.ContainerPacking.Entities.Item>> itemsToPack, int algorithm = 1, int minimumBooks = 20)
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

            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                if (items.Count == 0) //There are no lists of items left
                {
                    break;
                }
                if (items[currentPackGroup].Count == 0) //There are no items left in the current list
                {
                    if (currentPackGroup == 0) //If the current list is the last
                    {
                        break;
                    }
                    items.RemoveAt(currentPackGroup); //Remove empty list
                    currentPackGroup--; //move to next group
                }

                //Create list of items to pack
                List<Item> itemsToPackGroup = items[currentPackGroup];
                //If there are fewer than the minimum item count per bin, then make up numbers from the next group
                if (itemsToPackGroup.Count < minimumBooks && currentPackGroup > 0)
                {
                    //If there are fewer items in the next group than the minimum, include them all
                    if (items[currentPackGroup - 1].Count < minimumBooks - itemsToPackGroup.Count)
                    {
                        itemsToPackGroup.AddRange(items[currentPackGroup - 1]); //Add 
                        items[currentPackGroup - 1].Clear(); //Remove all items from the list because they are now in the pack list
                    }
                    else //If there are more items in the next group, take just enough to make the minimum
                    {
                        int numberToTake = minimumBooks - itemsToPackGroup.Count;
                        for (int i = numberToTake - 1; i >= 0; i--)
                        {
                            itemsToPackGroup.Add(items[currentPackGroup - 1][i]);
                            items[currentPackGroup - 1].RemoveAt(i);
                        }
                    }
                }

                //Create CromulentBisgetti Container
                Container con = ContainerToCB(container);
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