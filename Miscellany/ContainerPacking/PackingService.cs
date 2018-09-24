using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;
using Autodesk.DesignScript.Runtime;

namespace Miscellany.ContainerPacking
{
    /// <summary>
    /// PackingService
    /// </summary>
    public static class PackingService
    {
        #region PackContainer

        /// <summary>
        /// Runs the chosen packing algorithm given a single container and a list of items to pack. The default algorithm (1) is EB-AFIT from David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <param name="algorithm">Algorithm ID</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <returns name="isCompletePack">Are all items packed?</returns>
        /// <returns name="packTimeInMilliseconds">Pack time</returns>
        /// <returns name="percentContainerVolumePacked">Percentage of the container that is packed</returns>
        /// <returns name="percentItemVolumePacked">Percentage of items packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems", "isCompletePack", "packTimeInMilliseconds", "percentContainerVolumePacked", "percentItemVolumePacked" })]
        public static Dictionary<string, object> PackContainer(Miscellany.ContainerPacking.Entities.Container container, List<Miscellany.ContainerPacking.Entities.Item> itemsToPack, int algorithm = 1)
        {
            //Create CromulentBisgetti Container
            decimal dLength = Miscellany.Math.Functions.ToDecimal(container.Length);
            decimal dWidth = Miscellany.Math.Functions.ToDecimal(container.Width);
            decimal dHeight = Miscellany.Math.Functions.ToDecimal(container.Height);
            Container con = new Container(container.ID, dLength, dWidth, dHeight);
            
            //Create CromulentBisgetti Items
            List<Item> items = new List<Item>();
            foreach (Miscellany.ContainerPacking.Entities.Item i in itemsToPack)
            {
                decimal ddim1 = Miscellany.Math.Functions.ToDecimal(i.Dim1);
                decimal ddim2 = Miscellany.Math.Functions.ToDecimal(i.Dim2);
                decimal ddim3 = Miscellany.Math.Functions.ToDecimal(i.Dim3);
                Item cbItem = new Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                items.Add(cbItem);
            }
            
            //Create list with single container
            List<Container> containers = new List<Container> { con };
            
            //Select algorithm using integer
            List<int> algorithms = new List<int> { algorithm };
            
            //Get container packing result
            ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(containers, items, algorithms).FirstOrDefault();
            
            //Get the single algorthim packing result from the container packing result
            AlgorithmPackingResult algorithmPackingResult = containerPackingResult.AlgorithmPackingResults.FirstOrDefault();
            bool IsCompletePack = algorithmPackingResult.IsCompletePack;
            int PackTimeInMilliseconds = Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds); //Max limit of int32 for milliseconds is596 hours
            double PercentContainerVolumePacked = Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentContainerVolumePacked);
            double PercentItemVolumePacked = Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentItemVolumePacked);
            
            //Convert CromulentBisgetti items to Miscellany Items
            //Packed Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsPacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (Item i in algorithmPackingResult.PackedItems)
            {
                Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                itemsPacked.Add(mItem);
            }
            //Unpacked Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (Item i in algorithmPackingResult.UnpackedItems)
            {
                Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                itemsUnpacked.Add(mItem);
            }
            
            //Return values
            var d = new Dictionary<string, object>();
            d.Add("packedItems", itemsPacked);
            d.Add("unpackedItems", itemsUnpacked);
            d.Add("isCompletePack", IsCompletePack);
            d.Add("packTimeInMilliseconds", PackTimeInMilliseconds);
            d.Add("percentContainerVolumePacked", PercentContainerVolumePacked);
            d.Add("percentItemVolumePacked", PercentItemVolumePacked);
            return d;
        }

        #endregion

        #region PackContainers

        /// <summary>
        /// Runs the chosen packing algorithm as a greedy strategy on a list of containers and a list of items to pack. It aims to solve optimally at each container in turn and is not globally optimal. The default algorithm (1) is EB-AFIT from David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="containers">Containers in order</param>
        /// <param name="itemsToPack">Items to pack</param>
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
        public static Dictionary<string, object> PackContainers(List<Miscellany.ContainerPacking.Entities.Container> containers, List<Miscellany.ContainerPacking.Entities.Item> itemsToPack, int algorithm = 1)
        {
            //Select algorithm using integer
            List<int> algorithms = new List<int> { algorithm };
            
            //Create CromulentBisgetti Items
            List<Item> items = new List<Item>();
            foreach (Miscellany.ContainerPacking.Entities.Item i in itemsToPack)
            {
                decimal ddim1 = Miscellany.Math.Functions.ToDecimal(i.Dim1);
                decimal ddim2 = Miscellany.Math.Functions.ToDecimal(i.Dim2);
                decimal ddim3 = Miscellany.Math.Functions.ToDecimal(i.Dim3);
                Item cbItem = new Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                items.Add(cbItem);
            }
            
            //Output lists
            List<List<Miscellany.ContainerPacking.Entities.Item>> itemsPacked = new List<List<Miscellany.ContainerPacking.Entities.Item>>();
            bool IsCompletePack = false;
            List<int> PackTimeInMilliseconds = new List<int>();
            int TotalPackTimeInMilliseconds = 0;
            List<double> PercentContainerVolumePacked = new List<double>();
            List<double> PercentItemVolumePacked = new List<double>();
            
            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                //Create CromulentBisgetti Container
                decimal dLength = Miscellany.Math.Functions.ToDecimal(container.Length);
                decimal dWidth = Miscellany.Math.Functions.ToDecimal(container.Width);
                decimal dHeight = Miscellany.Math.Functions.ToDecimal(container.Height);
                Container con = new Container(container.ID, dLength, dWidth, dHeight);
                List<Container> cons = new List<Container> { con };
                //Get container packing result
                ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(cons, items, algorithms).FirstOrDefault();
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
                items = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                IsCompletePack = algorithmPackingResult.IsCompletePack;
                PackTimeInMilliseconds.Add(Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds));
                TotalPackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
                PercentContainerVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentContainerVolumePacked));
                PercentItemVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentItemVolumePacked));
                if (algorithmPackingResult.IsCompletePack == true)
                {
                    break;
                }
            }

            //Convert CromulentBisgetti items to Miscellany Items for Unpacked Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (Item i in items)
            {
                Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                itemsUnpacked.Add(mItem);
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

        #region Methods

        //Convert a CromulentBisgetti Item to a Miscellany Item
        private static Miscellany.ContainerPacking.Entities.Item ItemToMiscellany(Item i)
        {
            double ddim1 = Miscellany.Math.Functions.ToDouble(i.Dim1);
            double ddim2 = Miscellany.Math.Functions.ToDouble(i.Dim2);
            double ddim3 = Miscellany.Math.Functions.ToDouble(i.Dim3);
            //Create Item
            Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
            //Add packed information
            mItem.IsPacked = i.IsPacked;
            if (i.IsPacked)
            {
                mItem.PackDimX = Miscellany.Math.Functions.ToDouble(i.PackDimX);
                mItem.PackDimY = Miscellany.Math.Functions.ToDouble(i.PackDimY);
                mItem.PackDimZ = Miscellany.Math.Functions.ToDouble(i.PackDimZ);
                mItem.CoordX = Miscellany.Math.Functions.ToDouble(i.CoordX);
                mItem.CoordY = Miscellany.Math.Functions.ToDouble(i.CoordY);
                mItem.CoordZ = Miscellany.Math.Functions.ToDouble(i.CoordZ);
            }
            return mItem;
        }

        #endregion
    }
}
