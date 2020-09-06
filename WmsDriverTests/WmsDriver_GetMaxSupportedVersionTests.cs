using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    public class WmsDriver_GetMaxSupportedVersionTests
    {
        //properly works out the highest version based on the supported versions
        [Test]
        public void GetMaxSupportedVersion_AlwaysRegardlessTheOrder_ReturnsMaxSupportedVersion()
        {
            var drv = MakeWmsDriver() as FakeWmsDriver;

            //initially no versions there
            var mv = drv.GetMaxSupportedVersion();
            mv.Should().Be(null);

            drv.AddMoreVersions(new[] { "1.3.0" });
            mv = drv.GetMaxSupportedVersion();
            mv.Should().Be("1.3.0");

            drv.AddMoreVersions(new[] {"1.0.0", "1.1.1"});
            mv = drv.GetMaxSupportedVersion();
            mv.Should().Be("1.3.0");

            drv.AddMoreVersions(new[] { "1.3.9", "1.4.0", "1.1.2", "1.3.3"});
            mv = drv.GetMaxSupportedVersion();
            mv.Should().Be("1.4.0");

            drv.AddMoreVersions(new[] { "6.6.6" });
            mv = drv.GetMaxSupportedVersion();
            mv.Should().Be("6.6.6");
        }


        private Cartomatic.Wms.WmsDriver MakeWmsDriver()
        {
            return new FakeWmsDriver();
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public void AddMoreVersions(string[] range)
            {
                SupportedVersions.AddRange(range);
            }
        }
    }
}
