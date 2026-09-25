using System.Collections.Generic;
using Xunit;

namespace Miscellany.Tests
{
    public class MathsTests
    {
        [Fact]
        public void RunningTotal() => Assert.Equal(new[] { 1.0, 3.0, 6.0, 10.0 }, Maths.RunningTotal(new List<double> { 1, 2, 3, 4 }));

        [Fact]
        public void ToDecimalRoundTrips() => Assert.Equal(12.5, Maths.ToDouble(Maths.ToDecimal(12.5)));

        [Fact]
        public void ToDecimalClampsOutOfRangeValues()
        {
            Assert.Equal(decimal.MaxValue, Maths.ToDecimal(1e30));
            Assert.Equal(decimal.MinValue, Maths.ToDecimal(-1e30));
        }
    }

    public class ListTests
    {
        [Fact]
        public void PairItemsPairsAdjacentItems()
        {
            var pairs = List.Modifies.PairItems(new List<object> { "a", "b", "c" });
            Assert.Equal(2, pairs.Count);
            Assert.Equal(new object[] { "a", "b" }, (IEnumerable<object>)pairs[0]);
            Assert.Equal(new object[] { "b", "c" }, (IEnumerable<object>)pairs[1]);
        }

        [Fact]
        public void PairItemsOfASingleItemIsEmpty() => Assert.Empty(List.Modifies.PairItems(new List<object> { "a" }));
    }
}
