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
        /// Runs the EB-AFIT packing algorithm given a single container and a list of items to pack
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems"})]
        public static Dictionary<string, List<Miscellany.ContainerPacking.Entities.Item>> Pack(Miscellany.ContainerPacking.Entities.Container container, List<Miscellany.ContainerPacking.Entities.Item> itemsToPack)
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
            var d = new Dictionary<string, List<Miscellany.ContainerPacking.Entities.Item>>();
            d.Add("packedItems", itemsPacked);
            d.Add("unpackedItems", itemsUnpacked);
            return d;
        }
    }
}
