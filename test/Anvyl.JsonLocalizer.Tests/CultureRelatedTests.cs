using System.Globalization;
using Xunit;

namespace Anvyl.JsonLocalizer.Tests
{
    public class CultureRelatedTests : BaseTest
    {
        [Fact(DisplayName = "Creates a new json file with null values copied from default culture")]
        public void Creates_New_File_Copying_From_Default()
        {
            var localizerRO = _localizer.WithCulture(new CultureInfo("ro-RO"));
            const string locKey = "Hello";
            Assert.NotNull(localizerRO);
            Assert.True(localizerRO[locKey].ResourceNotFound);
            Assert.Equal($"[{locKey}]", localizerRO[locKey]);
            
            // Cleanup
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }
    }
}