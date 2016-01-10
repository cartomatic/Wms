using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;
using WmsDriver = Cartomatic.Manifold.WmsDriver;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class HandleGetCapabilitiesTests
    {
        //Note:
        //GetCaps tests are really simplistic, just to make sure output xml contains some expected data

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleGetCapabilities_WhenParamsOk_GeneratesLayersSectionForAllTheLayers()
        {
            var drv = MakeWmsDriver();

            var drvOutput = drv.HandleRequest("http://some.url/?request=GetCapabilities&service=WMS");

            drvOutput.ResponseText.Should().Contain("<Name>Natural Earth 50m</Name>");
            drvOutput.ResponseText.Should().Contain("<Name>Ne_50m_coastline Drawing</Name>");
            drvOutput.ResponseText.Should().Contain("<Name>NE2_50M_SR_W</Name>");
        }


        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleGetCapabilities_WhenParamsOkAndCombineLayers_GeneratesLayersSectionForTheMapLevelOnly()
        {
            var drv = MakeWmsDriver();
            drv.CombineLayers = true;

            var drvOutput = drv.HandleRequest("http://some.url/?request=GetCapabilities&service=WMS");

            drvOutput.ResponseText.Should().Contain("<Name>Natural Earth 50m</Name>");
            drvOutput.ResponseText.Should().NotContain("<Name>Ne_50m_coastline Drawing</Name>");
            drvOutput.ResponseText.Should().NotContain("<Name>NE2_50M_SR_W</Name>");
        }


        private FakeWmsDriver MakeWmsDriver()
        {
            return new FakeWmsDriver(Utils.GetMapFilePath());
        }

        class FakeWmsDriver : WmsDriver
        {
            public bool CombineLayers { get; set; }

            public FakeWmsDriver(string mapFile, string mapComp = "Map", WmsServiceDescription serviceDescription = null)
                : base(mapFile, mapComp, serviceDescription)
            {
            }

            protected override IWmsDriverResponse HandleGetCapabilities()
            {

                //adjust the settings 
                if (CombineLayers)
                {
                    MSettings.CombineLayers = true;
                }

                return base.HandleGetCapabilities();
            }
        }
    }
}
