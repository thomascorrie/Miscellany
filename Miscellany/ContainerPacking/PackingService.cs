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
        public static Dictionary<string, ContainerPackingResult> Packs(Container container, List<Item> itemsToPack)
        {
            List<Container> containers = new List<Container> { container };
            List<int> algorithms = new List<int> { 1 };
            ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(containers, itemsToPack, algorithms).FirstOrDefault();
            AlgorithmPackingResult algorithmPackingResult = containerPackingResult.AlgorithmPackingResults.FirstOrDefault();

            //Return values
            var d = new Dictionary<string, ContainerPackingResult>();
            //d.Add("packedItems", display);
            //d.Add("unpackedItems", pt);
            return d;
        }
    }
}
