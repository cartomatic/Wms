using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class HandleGetCapabilitiesTests
    {
        //Note:
        //GetCaps tests are really simplistic, just to make sure output xml contains some expected data

        [Test]
        [Category("ManifoldMapFileDependant")]
        public async Task HandleGetCapabilities_WhenParamsOk_GeneratesLayersSectionForAllTheLayers()
        {
            var drv = MakeWmsDriver();

            var drvOutput = await drv.HandleRequestAsync("http://some.url/?request=GetCapabilities&service=WMS");

            drvOutput.ResponseText.Should().Contain("<Name>Natural Earth 50m</Name>");
            drvOutput.ResponseText.Should().Contain("<Name>Ne_50m_coastline Drawing</Name>");
            drvOutput.ResponseText.Should().Contain("<Name>NE2_50M_SR_W</Name>");
        }


        [Test]
        [Category("ManifoldMapFileDependant")]
        public async Task HandleGetCapabilities_WhenParamsOkAndCombineLayers_GeneratesLayersSectionForTheMapLevelOnly()
        {
            var drv = MakeWmsDriver();
            drv.CombineLayers = true;

            var drvOutput = await drv.HandleRequestAsync("http://some.url/?request=GetCapabilities&service=WMS");

            drvOutput.ResponseText.Should().Contain("<Name>Natural Earth 50m</Name>");
            drvOutput.ResponseText.Should().NotContain("<Name>Ne_50m_coastline Drawing</Name>");
            drvOutput.ResponseText.Should().NotContain("<Name>NE2_50M_SR_W</Name>");
        }


        private FakeManifoldWmsDriver MakeWmsDriver()
        {
            return new FakeManifoldWmsDriver(Utils.GetMapFilePath());
        }

        class FakeManifoldWmsDriver : ManifoldWmsDriver
        {
            public bool CombineLayers { get; set; }

            public FakeManifoldWmsDriver(string mapFile, string mapComp = "Map", WmsServiceDescription serviceDescription = null)
                : base(mapFile, mapComp, serviceDescription)
            {
            }

            protected override async Task<IWmsDriverResponse> HandleGetCapabilitiesAsync()
            {

                //adjust the settings 
                if (CombineLayers)
                {
                    MSettings.CombineLayers = true;
                }

                return await base.HandleGetCapabilitiesAsync();
            }
        }
    }
}
