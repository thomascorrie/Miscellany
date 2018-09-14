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
        /// Runs a packing algorithm given a container and a list of items to pack
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems"})]
        public static Dictionary<string, List<Item>> Pack(Container container, List<Item> itemsToPack)
        {
            //Create list with single container
            List <Container> containers = new List<Container> { container };
            //Select EB-AFIT algorithm using integer of 1
            List<int> algorithms = new List<int> { 1 };
            //Get container packing result
            ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(containers, itemsToPack, algorithms).FirstOrDefault();
            //Get the single algorthim packing result from the container packing result
            AlgorithmPackingResult algorithmPackingResult = containerPackingResult.AlgorithmPackingResults.FirstOrDefault();
            //Return values
            var d = new Dictionary<string, List<Item>>();
            d.Add("packedItems", algorithmPackingResult.PackedItems);
            d.Add("unpackedItems", algorithmPackingResult.UnpackedItems);
            return d;
        }
    }
}
