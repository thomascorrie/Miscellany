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
        /// <summary>
        /// Runs the EB-AFIT packing algorithm given a single container and a list of items to pack. Based on David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <returns name="isCompletePack">Are all items packed?</returns>
        /// <returns name="packTimeInMilliseconds">Pack time</returns>
        /// <returns name="percentContainerVolumePacked">Percentage of the container that is packed</returns>
        /// <returns name="percentItemVolumePacked">Percentage of items packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems", "isCompletePack", "packTimeInMilliseconds", "percentContainerVolumePacked", "percentItemVolumePacked"})]
        public static Dictionary<string, object> PackContainer(Miscellany.ContainerPacking.Entities.Container container, List<Miscellany.ContainerPacking.Entities.Item> itemsToPack)
        {
            //Create CromulentBisgetti Container
            decimal dLength = Miscellany.Math.Functions.ToDecimal(container.Length);
            decimal dWidth = Miscellany.Math.Functions.ToDecimal(container.Width);
            decimal dHeight = Miscellany.Math.Functions.ToDecimal(container.Height);
            Container con = new Container(container.ID,dLength,dWidth,dHeight);
            //Create CromulentBisgetti Items
            List<Item> items = new List<Item>();
            foreach (Miscellany.ContainerPacking.Entities.Item i in itemsToPack)
            {
                decimal ddim1 = Miscellany.Math.Functions.ToDecimal(i.Dim1);
                decimal ddim2 = Miscellany.Math.Functions.ToDecimal(i.Dim2);
                decimal ddim3 = Miscellany.Math.Functions.ToDecimal(i.Dim3);
                Item cbItem = new Item(i.ID,ddim1,ddim2,ddim3,i.Quantity);
                items.Add(cbItem);
            }
            //Create list with single container
            List<Container> containers = new List<Container> { con };
            //Select EB-AFIT algorithm using integer of 1
            List<int> algorithms = new List<int> { 1 };
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
                //Create item
                double ddim1 = Miscellany.Math.Functions.ToDouble(i.Dim1);
                double ddim2 = Miscellany.Math.Functions.ToDouble(i.Dim2);
                double ddim3 = Miscellany.Math.Functions.ToDouble(i.Dim3);
                Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                //Add packed information
                mItem.PackDimX = Miscellany.Math.Functions.ToDouble(i.PackDimX);
                mItem.PackDimY = Miscellany.Math.Functions.ToDouble(i.PackDimY);
                mItem.PackDimZ = Miscellany.Math.Functions.ToDouble(i.PackDimZ);
                mItem.CoordX =  Miscellany.Math.Functions.ToDouble(i.CoordX);
                mItem.CoordY =  Miscellany.Math.Functions.ToDouble(i.CoordY);
                mItem.CoordZ =  Miscellany.Math.Functions.ToDouble(i.CoordZ);
                mItem.IsPacked = i.IsPacked;
                itemsPacked.Add(mItem);
            }
            //Unpacked Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (Item i in algorithmPackingResult.UnpackedItems)
            {
                //Create item
                double ddim1 = Miscellany.Math.Functions.ToDouble(i.Dim1);
                double ddim2 = Miscellany.Math.Functions.ToDouble(i.Dim2);
                double ddim3 = Miscellany.Math.Functions.ToDouble(i.Dim3);
                Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                //Add packed information
                mItem.IsPacked = i.IsPacked;
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

        /// <summary>
        /// Runs the EB-AFIT packing algorithm as a greedy strategy on a list of containers and a list of items to pack. It aims to solve optimally at each container in turn and is not globally optimal. Extending David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="containers">Containers in order</param>
        /// <param name="itemsToPack">Items to pack</param>
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
        public static Dictionary<string, object> PackContainers(List<Miscellany.ContainerPacking.Entities.Container> containers, List<Miscellany.ContainerPacking.Entities.Item> itemsToPack)
        {
            //Select EB-AFIT algorithm using integer of 1
            List<int> algorithms = new List<int> { 1 };
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
            List<int> PackTimesInMilliseconds = new List<int>();
            int PackTimeInMilliseconds = 0;
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
                    //Create item
                    double ddim1 = Miscellany.Math.Functions.ToDouble(i.Dim1);
                    double ddim2 = Miscellany.Math.Functions.ToDouble(i.Dim2);
                    double ddim3 = Miscellany.Math.Functions.ToDouble(i.Dim3);
                    Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                    //Add packed information
                    mItem.PackDimX = Miscellany.Math.Functions.ToDouble(i.PackDimX);
                    mItem.PackDimY = Miscellany.Math.Functions.ToDouble(i.PackDimY);
                    mItem.PackDimZ = Miscellany.Math.Functions.ToDouble(i.PackDimZ);
                    mItem.CoordX = Miscellany.Math.Functions.ToDouble(i.CoordX);
                    mItem.CoordY = Miscellany.Math.Functions.ToDouble(i.CoordY);
                    mItem.CoordZ = Miscellany.Math.Functions.ToDouble(i.CoordZ);
                    mItem.IsPacked = i.IsPacked;
                    itemsPackedPass.Add(mItem);
                }
                itemsPacked.Add(itemsPackedPass);
                items = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                IsCompletePack = algorithmPackingResult.IsCompletePack;
                PackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
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
                //Create item
                double ddim1 = Miscellany.Math.Functions.ToDouble(i.Dim1);
                double ddim2 = Miscellany.Math.Functions.ToDouble(i.Dim2);
                double ddim3 = Miscellany.Math.Functions.ToDouble(i.Dim3);
                Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
                //Add packed information
                mItem.IsPacked = i.IsPacked;
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
    }
}
