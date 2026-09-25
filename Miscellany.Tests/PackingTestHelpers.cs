using System.Collections.Generic;
using System.Linq;
using Xunit;
using M = Miscellany.ContainerPacking.Entities;

namespace Miscellany.Tests
{
    /// <summary>
    /// Builds unit-cube Items and 1 x 1 x n Containers, so a Container of length n holds exactly n Items
    /// and results from the real EB-AFIT algorithm are predictable.
    /// </summary>
    internal static class PackingTestHelpers
    {
        private static int nextId = 1;

        public static M.Item Cube(int quantity = 1) => new M.Item(nextId++, 1, 1, 1, quantity);

        public static List<M.Item> Cubes(int count) => Enumerable.Range(0, count).Select(_ => Cube()).ToList();

        public static List<List<M.Item>> Groups(params int[] sizes) => sizes.Select(Cubes).ToList();

        public static List<M.Container> Containers(params int[] lengths) =>
            lengths.Select((length, i) => new M.Container(i, length, 1, 1)).ToList();

        public static List<M.Item> Packed(Dictionary<string, object> result)
        {
            var packed = result["packedItems"];
            return packed is List<List<M.Item>> perContainer ? perContainer.SelectMany(l => l).ToList() : (List<M.Item>)packed;
        }

        public static List<M.Item> Unpacked(Dictionary<string, object> result) => (List<M.Item>)result["unpackedItems"];

        public static bool IsComplete(Dictionary<string, object> result) => (bool)result["isCompletePack"];

        public static int Units(IEnumerable<M.Item> items) => items.Sum(i => i.Quantity);

        /// <summary>
        /// Every unit is either packed or unpacked, isCompletePack agrees with the unpacked list,
        /// packed Items are single units numbered 1..n in packing order.
        /// </summary>
        public static void AssertConsistent(Dictionary<string, object> result, int expectedUnits)
        {
            var packed = Packed(result);
            var unpacked = Unpacked(result);
            Assert.Equal(expectedUnits, Units(packed) + Units(unpacked));
            Assert.Equal(unpacked.Count == 0, IsComplete(result));
            Assert.All(packed, i => Assert.Equal(1, i.Quantity));
            Assert.All(packed, i => Assert.True(i.IsPacked));
            Assert.Equal(Enumerable.Range(1, packed.Count), packed.Select(i => i.Sequence));
        }
    }
}
