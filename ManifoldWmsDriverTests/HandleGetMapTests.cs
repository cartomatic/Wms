using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;
using Cartomatic.Utils.Web;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class HandleGetMapTests
    {
        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleGetMap_WhenCrsInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            drv.CreateMapServer();
            drv.ExtractWmsDriverSettings();
            var testedValidationRule = drv.HandleGetMapValidationRulesDriverSpecific["crs_supported"];
            var request = "http://some.url/?crs=EPSG:2180".CreateHttpWebRequest();
            drv.ExtractRequestParamsPublic(request);
            
            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleGetMap_WhenCrsOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            drv.CreateMapServer();
            drv.ExtractWmsDriverSettings();
            var testedValidationRule = drv.HandleGetMapValidationRulesDriverSpecific["crs_supported"];
            var request = "http://some.url/?crs=EPSG:4326".CreateHttpWebRequest();
            drv.ExtractRequestParamsPublic(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [TestCase("whatever")]
        [TestCase("3,5,1,4")]
        [TestCase(" , ")]
        public void HandleGetMap_WhenStylesParamInvalid_ValidationRuleThrows(string styles)
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRulesDriverSpecific["styles_valid"];
            var request = string.Format("http://some.url/?styles={0}", styles).CreateHttpWebRequest();
            drv.ExtractRequestParamsPublic(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [TestCase("")]
        [TestCase(",")]
        public void HandleGetMap_WhenStylesParamValid_ValidationRuleShouldNotThrow(string styles)
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRulesDriverSpecific["styles_valid"];
            var request = string.Format("http://some.url/?styles={0}", styles).CreateHttpWebRequest();
            drv.ExtractRequestParamsPublic(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }




        private FakeManifoldWmsDriver MakeWmsDriver()
        {
            return new FakeManifoldWmsDriver(Utils.GetMapFilePath());
        }

        class FakeManifoldWmsDriver : ManifoldWmsDriver
        {
            public FakeManifoldWmsDriver(string mapFile, string mapComp = "Map", WmsServiceDescription serviceDescription = null)
                : base(mapFile, mapComp, serviceDescription)
            {
            }

            public void ExtractRequestParamsPublic(HttpWebRequest request)
            {
                base.ExtractRequestParams(request);
            }
        }
    }
}
