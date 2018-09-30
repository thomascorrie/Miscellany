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
            Container con = ContainerToCB(container);

            //Create CromulentBisgetti Items
            List<Item> items = new List<Item>();
            foreach (Miscellany.ContainerPacking.Entities.Item i in itemsToPack)
            {
                Item cbItem = ItemToCB(i);
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
            //int BestFitOrientation = algorithmPackingResult.best

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
            d.Add("orientation", PercentItemVolumePacked);
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
                //Swap Y and Z values to shift from the CromulentBisgetti coordinate system and Dynamo
                mItem.PackDimX = Miscellany.Math.Functions.ToDouble(i.PackDimX);
                mItem.PackDimY = Miscellany.Math.Functions.ToDouble(i.PackDimZ);
                mItem.PackDimZ = Miscellany.Math.Functions.ToDouble(i.PackDimY);
                mItem.CoordX = Miscellany.Math.Functions.ToDouble(i.CoordX);
                mItem.CoordY = Miscellany.Math.Functions.ToDouble(i.CoordZ);
                mItem.CoordZ = Miscellany.Math.Functions.ToDouble(i.CoordY);
            }
            return mItem;
        }

        //Convert Miscellany Container to CromulentBisgetti Container
        private static Container ContainerToCB(Miscellany.ContainerPacking.Entities.Container c)
        {
            decimal dLength = Miscellany.Math.Functions.ToDecimal(c.Length);
            decimal dWidth = Miscellany.Math.Functions.ToDecimal(c.Width);
            decimal dHeight = Miscellany.Math.Functions.ToDecimal(c.Height);
            Container cbContainer = new Container(c.ID,dLength,dWidth,dHeight);
            return cbContainer;
        }

        //Convert Miscellany Item to CromulentBisgetti Item
        private static Item ItemToCB(Miscellany.ContainerPacking.Entities.Item i)
        {
            decimal ddim1 = Miscellany.Math.Functions.ToDecimal(i.Dim1);
            decimal ddim2 = Miscellany.Math.Functions.ToDecimal(i.Dim2);
            decimal ddim3 = Miscellany.Math.Functions.ToDecimal(i.Dim3);
            Item cbItem = new Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
            return cbItem;
        }

        #endregion
    }
}
