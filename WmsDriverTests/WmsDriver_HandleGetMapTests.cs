﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cartomatic.Utils.Web;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    public class WmsDriver_HandleGetMapTests
    {
        [Test]
        public void HandleGetMap_WhenNoLayersParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["layers_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenLayersParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["layers_param_presence"];
            var request = "http://some.url/?request=GetMap&layers=some,layers".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoStylesParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["styles_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenStylesParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["styles_param_presence"];
            var request = "http://some.url/?request=GetMap&styles=some,styles".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoCrsParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["crssrs_param_presence"];
            var request = "http://some.url/?request=GetMap&version=1.3.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenCrsParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["crssrs_param_presence"];
            var request = "http://some.url/?request=GetMap&version=1.3.0&crs=someCrs".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoSrsParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["crssrs_param_presence"];
            var request = "http://some.url/?request=GetMap&version=1.1.1".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenSrsParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["crssrs_param_presence"];
            var request = "http://some.url/?request=GetMap&version=1.1.1&srs=someCrs".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoBboxParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bbox_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenBboxParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bbox_param_presence"];
            var request = "http://some.url/?request=GetMap&bbox=someBbox".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoWidthParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_presence"];
            var request = "http://some.url/?request=GetMap&width=someWidth".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoHeightParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenHeightParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_presence"];
            var request = "http://some.url/?request=GetMap&height=someHeight".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenNoFormatParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["format_param_presence"];
            var request = "http://some.url/?request=GetMap".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenFormatParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["format_param_presence"];
            var request = "http://some.url/?request=GetMap&format=someFormat".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetMap&width=invalid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamIsZero_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetMap&width=0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamTooLarge_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetMap&width=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamLargeButNoRestrictionsSpecified_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetMap&width=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenWidthParamOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetMap&width=256".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_HeightWidthParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetMap&height=invalid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenHeightParamIsZero_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetMap&height=0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenHeightParamTooLarge_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetMap&height=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxHeight = 1000;

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenHeightParamLargeButNoRestrictionsSpecified_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetMap&height=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_WhenHeightParamOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetMap&height=256".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }


        [Test]
        public void HandleGetMap_WhenFormatParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["format_param_valid"];
            var request = "http://some.url/?request=GetMap&version=1.3.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);
            
            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_WhenFormatParamValid_ValidationShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["format_param_valid"];
            var request = "http://some.url/?request=GetMap&version=1.3.0&format=image/png".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleGetMap_BgParamIfPresentAndInvalid_ValidationRuleShould_Throw()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bgcolor_valid"];
            var request = "http://some.url/?bgcolor=invalid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [TestCase("blue")]
        [TestCase("#FFF")]
        public void HandleGetMap_BgParamIfPresentAndValid_ValidationRuleShouldNotThrow(string bgColor)
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bgcolor_valid"];
            var request = string.Format("http://some.url/?bgcolor={0}", bgColor).CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }


        [TestCase("")]
        [TestCase("3,5,1,4")]
        public void HandleGetMap_WhenBboxInvalid_ValidationRuleThrows(string bbox)
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bbox_valid"];
            var request = string.Format("http://some.url/?version=1.3.0&crs=EPSG:2180&bbox={0}", bbox).CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleGetMap_BboxOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetMapValidationRules["bbox_valid"];
            var request = "http://some.url/?version=1.3.0&crs=EPSG:4326&bbox=1,2,3,4".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public async Task HandleGetMap_WhenPassesValidation_CallsHandleGetMapDriverSpecific()
        {
            var drv = MakeWmsDriver();
            var request = "http://some.url/?request=GetMap&service=WMS&version=1.3.0&layers=someLayers&styles=&crs=someCrs&bbox=1,2,3,4&width=256&height=256&format=image/png".CreateHttpWebRequest();

            await drv.HandleRequestAsync(request);

            drv.HandleGetMapDriverSpecificCalled.Should().BeTrue();
        }

        [Test]
        public async Task HandleGetMap_WhenParamsOkDriverSpecificGetMapNotImplemented_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            drv.CallBaseHandleGetMapDriverSpecific = true;
            var expectedMsg = "IMPLEMENTATION ERROR: GetMap is a mandatory operation for WMS 1.3.0.";

            var response = await drv.HandleRequestAsync("http://some.url/?request=GetMap&service=WMS&version=1.3.0&layers=someLayers&styles=&crs=someCrs&bbox=1,2,3,4&width=256&height=256&format=image/png");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(expectedMsg);
        }


        private FakeWmsDriver MakeWmsDriver()
        {
            var drv = new FakeWmsDriver();

            drv.ServiceDescription = new WmsServiceDescription();

            drv.SupportedVersions.Add("1.3.0");

            drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>(){"XML"});
            drv.DefaultGetCapabilitiesFormats.Add("1.3.0", "XML");

            drv.SupportedGetMapFormats.Add("1.3.0", new List<string>(){"image/png"});

            drv.SupportedExceptionFormats.Add("1.3.0", new List<string>(){ "XML"});
            drv.DefaultExceptionFormats.Add("1.3.0", "XML");


            return drv;
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public bool HandleGetMapDriverSpecificCalled { get; set; }

            public bool CallBaseHandleGetMapDriverSpecific { get; set; }
        
            protected override async Task<IWmsDriverResponse> HandleGetMapDriverSpecificAsync()
            {
                HandleGetMapDriverSpecificCalled = true;

                if (CallBaseHandleGetMapDriverSpecific)
                {
                    return await base.HandleGetMapDriverSpecificAsync();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
