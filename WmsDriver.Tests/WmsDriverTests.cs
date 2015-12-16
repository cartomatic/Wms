using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriverTests
    {
        [Test]
        public void Constructor_WhenCalled_ShouldAlwaysInitDataContainers()
        {
            var drv = MakeWmsDriver() as TestWmsDriver;

            drv.SupportedGetFeatureInfoFormats.Should().NotBeNull();
            drv.SupportedGetCapabilitiesFormats.Should().NotBeNull();
            drv.SupportedGetMapFormats.Should().NotBeNull();
            drv.SupportedExceptionFormats.Should().NotBeNull();
            drv.SupportedVersions.Should().NotBeNull();
        }

        [Test]
        public void Prepare_WhenServiceDescriptionNotProvided_CreatesDefaultWmsServiceDescription()
        {
            var drv = MakeWmsDriver() as TestWmsDriver;
            var defaultSd = MakeWmsServiceDescription().ApplyDefaults();
            var defaultSd1 = MakeWmsServiceDescription().ApplyDefaults();

            drv.ServiceDescription.Should().BeNull();

            drv.Prepare();

            drv.ServiceDescription.Should().NotBeNull();
        }

        private IWmsServiceDescription MakeWmsServiceDescription()
        {
            return new WmsServiceDescription();
        }

        private IWmsDriver MakeWmsDriver()
        {
            return new TestWmsDriver();
        }

        private class TestWmsDriver : Cartomatic.Wms.WmsDriver
        {
        }
    }
}