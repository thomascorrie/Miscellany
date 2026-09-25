using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using static Miscellany.Tests.PackingTestHelpers;
using M = Miscellany.ContainerPacking.Entities;
using PS = Miscellany.ContainerPacking.PackingService;

namespace Miscellany.Tests
{
    public class PackContainerTests
    {
        [Fact]
        public void PacksItemsThatFit()
        {
            var result = PS.PackContainer(Containers(3)[0], Cubes(3));
            AssertConsistent(result, 3);
            Assert.True(IsComplete(result));
            Assert.Equal(3, Packed(result).Count);
        }

        [Fact]
        public void ReportsItemsThatDoNotFit()
        {
            var result = PS.PackContainer(Containers(2)[0], Cubes(5));
            AssertConsistent(result, 5);
            Assert.Equal(2, Packed(result).Count);
            Assert.Equal(3, Unpacked(result).Count);
        }

        [Fact]
        public void SplitsQuantityIntoSingleUnits()
        {
            var result = PS.PackContainer(Containers(2)[0], new List<M.Item> { Cube(3) });
            AssertConsistent(result, 3);
            Assert.Equal(2, Units(Packed(result)));
            Assert.Equal(1, Units(Unpacked(result)));
        }

        [Fact]
        public void HandlesAnEmptyListOfItems()
        {
            var result = PS.PackContainer(Containers(2)[0], new List<M.Item>());
            AssertConsistent(result, 0);
            Assert.True(IsComplete(result));
        }

        [Fact]
        public void HasNoOrientationOutput()
        {
            Assert.False(PS.PackContainer(Containers(1)[0], Cubes(1)).ContainsKey("orientation"));
        }
    }

    public class PackContainersTests
    {
        [Fact]
        public void CarriesLeftoversIntoLaterContainers()
        {
            var result = PS.PackContainers(Containers(3, 3), Cubes(5));
            AssertConsistent(result, 5);
            Assert.True(IsComplete(result));
        }

        [Fact]
        public void DoesNotDuplicateQuantityWhenRepacking()
        {
            // Bug #16: two leftover units of a quantity-3 Item used to be expanded to six
            var result = PS.PackContainers(Containers(1, 1, 1, 1), new List<M.Item> { Cube(3) });
            AssertConsistent(result, 3);
            Assert.True(IsComplete(result));
        }

        [Fact]
        public void ReportsLeftoversWhenContainersRunOut()
        {
            var result = PS.PackContainers(Containers(1), new List<M.Item> { Cube(3) });
            AssertConsistent(result, 3);
            Assert.Equal(2, Units(Unpacked(result)));
        }
    }

    public class PackContainersWithGroupsTests
    {
        [Fact]
        public void IsIncompleteWhenGroupsRemain()
        {
            var result = PS.PackContainersWithGroups(Containers(3), Groups(3, 3));
            AssertConsistent(result, 6);
            Assert.False(IsComplete(result));
        }

        [Fact]
        public void KeepsGroupsInSeparateContainers()
        {
            var groups = Groups(2, 2);
            var firstGroupIds = groups[0].Select(i => i.ID).ToList();
            var result = PS.PackContainersWithGroups(Containers(5, 5), groups);
            var perContainer = (List<List<M.Item>>)result["packedItems"];
            AssertConsistent(result, 4);
            Assert.Equal(firstGroupIds.OrderBy(x => x), perContainer[0].Select(i => i.ID).OrderBy(x => x));
        }

        [Fact]
        public void HandlesASingleEmptyGroup()
        {
            var result = PS.PackContainersWithGroups(Containers(5), Groups(0));
            AssertConsistent(result, 0);
        }

        [Fact]
        public void SkipsEmptyGroupsWithoutUsingAContainer()
        {
            var result = PS.PackContainersWithGroups(Containers(2), Groups(0, 0, 2));
            AssertConsistent(result, 2);
            Assert.True(IsComplete(result));
        }

        [Fact]
        public void DoesNotDuplicateQuantityWhenRepacking()
        {
            var groups = new List<List<M.Item>> { new List<M.Item> { Cube(4) } };
            var result = PS.PackContainersWithGroups(Containers(1, 1, 1, 1, 1, 1), groups);
            AssertConsistent(result, 4);
            Assert.True(IsComplete(result));
        }
    }

    public class PackContainersWithGroupsContinuouslyTests
    {
        [Fact]
        public void ManySmallGroupsDoNotStall()
        {
            // Used to spin ~4 billion times (about 1.7 s per container) once more than 10 passes were needed
            var stopwatch = Stopwatch.StartNew();
            var result = PS.PackContainersWithGroupsContinuously(Containers(100), Groups(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), 1, 20);
            stopwatch.Stop();
            AssertConsistent(result, 11);
            Assert.True(IsComplete(result));
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Took {stopwatch.ElapsedMilliseconds} ms");
        }

        [Fact]
        public void CarriesLeftoversAfterTheLastGroup()
        {
            var result = PS.PackContainersWithGroupsContinuously(Containers(6, 6), Groups(10), 1, 20);
            AssertConsistent(result, 10);
            Assert.True(IsComplete(result));
        }

        [Fact]
        public void ReportsLeftoversWhenContainersRunOut()
        {
            var result = PS.PackContainersWithGroupsContinuously(Containers(6), Groups(10), 1, 20);
            AssertConsistent(result, 10);
            Assert.Equal(4, Unpacked(result).Count);
        }

        [Fact]
        public void IsIncompleteWhenGroupsRemain()
        {
            var result = PS.PackContainersWithGroupsContinuously(Containers(3), Groups(3, 3, 3), 1, 3);
            AssertConsistent(result, 9);
            Assert.False(IsComplete(result));
        }

        [Fact]
        public void UnpackedItemsKeepInputOrder()
        {
            var groups = Groups(2, 2, 2);
            var expected = groups.SelectMany(g => g).Skip(2).Select(i => i.ID).ToList();
            var result = PS.PackContainersWithGroupsContinuously(Containers(2), groups, 1, 2);
            Assert.Equal(expected, Unpacked(result).Select(i => i.ID));
        }

        [Fact]
        public void UntouchedGroupsKeepTheirQuantity()
        {
            var groups = new List<List<M.Item>> { new List<M.Item> { Cube() }, new List<M.Item> { Cube(7) } };
            var result = PS.PackContainersWithGroupsContinuously(Containers(1), groups, 1, 1);
            AssertConsistent(result, 8);
            Assert.Equal(7, Unpacked(result).Single().Quantity);
        }

        [Fact]
        public void TreatsMinimumItemsBelowOneAsOne()
        {
            var result = PS.PackContainersWithGroupsContinuously(Containers(5), Groups(0, 2, 0, 3), 1, 0);
            AssertConsistent(result, 5);
            Assert.Single(Packed(result));
        }
    }

    public class PackingRandomisedTests
    {
        [Fact]
        public void EveryUnitIsAccountedForInRandomCases()
        {
            var random = new Random(42);
            for (int n = 0; n < 200; n++)
            {
                var sizes = Enumerable.Range(0, random.Next(0, 6)).Select(_ => random.Next(0, 8)).ToArray();
                var lengths = Enumerable.Range(0, random.Next(0, 5)).Select(_ => random.Next(1, 10)).ToArray();
                int minimum = random.Next(-1, 15);
                int total = sizes.Sum();
                string context = $"groups=[{string.Join(",", sizes)}] containers=[{string.Join(",", lengths)}] min={minimum}";

                try
                {
                    AssertConsistent(PS.PackContainersWithGroupsContinuously(Containers(lengths), Groups(sizes), 1, minimum), total);
                    AssertConsistent(PS.PackContainersWithGroups(Containers(lengths), Groups(sizes)), total);
                    AssertConsistent(PS.PackContainers(Containers(lengths), Cubes(total)), total);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed for " + context, e);
                }
            }
        }
    }
}
