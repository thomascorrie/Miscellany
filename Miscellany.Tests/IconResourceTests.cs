using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Xunit;

namespace Miscellany.Tests
{
    /// <summary>
    /// Loads Miscellany.customization.dll the way Dynamo's IconServices does:
    /// new ResourceManager(assemblyName + "Images", customizationAssembly).GetObject(iconKey)
    /// </summary>
    public class IconResourceTests
    {
        private static ResourceManager IconResources()
        {
            // Not Assembly.Location: on .NET Framework the test runner shadow-copies test assemblies to a temp folder
            var assembly = Assembly.LoadFrom(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Miscellany.customization.dll"));
            return new ResourceManager("MiscellanyImages", assembly);
        }

        public static TheoryData<string> IconKeys => new TheoryData<string>
        {
            "Miscellany.About.Miscellany.Large",
            "Miscellany.About.Miscellany.Small",
            "Miscellany.ContainerPacking.PackingService.PackContainer.Large",
            "Miscellany.ContainerPacking.Entities.Item.Sequence.Small",
            "Miscellany.Geometry.Abstract.CoordinateSystemDisplay.Large",
            "Miscellany.List.Modifies.PairItems.Small",
            "Miscellany.Maths.RunningTotal.Large",
        };

        [Theory]
        [MemberData(nameof(IconKeys))]
        public void IconIsInTheFormatThisDynamoGenerationReads(string key)
        {
            object icon = IconResources().GetObject(key);
            Assert.NotNull(icon);
#if NETFRAMEWORK
            // Dynamo 2.x: (Bitmap)rm.GetObject(iconKey)
            var bitmap = Assert.IsType<System.Drawing.Bitmap>(icon);
            Assert.Equal(key.EndsWith(".Large") ? 128 : 32, bitmap.Width);
#else
            // Dynamo 3.x+: new MemoryStream(rm.GetObject(iconKey) as byte[])
            var bytes = Assert.IsType<byte[]>(icon);
            Assert.Equal(new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G' }, bytes.Take(4));
#endif
        }

        [Fact]
        public void EveryIconPngIsIncluded()
        {
            var set = IconResources().GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, false);
            int count = set.Cast<System.Collections.DictionaryEntry>().Count();
            Assert.Equal(66, count);
        }
    }
}
