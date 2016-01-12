using System;
using System.Collections.Generic;
using Cartomatic.Utils.Web;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriver_HandleGetLegendGraphicTests
    {
        [Test]
        public void HandleLegendGraphic_WhenNoLayersParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["layer_param_presence"];
            var request = "http://some.url/?VERSION=1.3.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenLayersParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["layer_param_presence"];
            var request = "http://some.url/?LAYER=topp:states".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenNoWidthParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic&width=someWidth".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenNoHeightParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenHeightParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic&height=someHeight".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenNoFormatParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["format_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenFormatParamProvided_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["format_param_presence"];
            var request = "http://some.url/?request=GetLegendGraphic&format=someFormat".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&width=invalid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamIsZero_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&width=0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamTooLarge_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&width=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamLargeButNoRestrictionsSpecified_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&width=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenWidthParamOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["width_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&width=256".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_HeightWidthParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&height=invalid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenHeightParamIsZero_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&height=0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenHeightParamTooLarge_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&height=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxHeight = 1000;

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenHeightParamLargeButNoRestrictionsSpecified_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&height=6000".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenHeightParamOk_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["height_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&height=256".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);
            drv.ServiceDescription.MaxWidth = 1000;

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }


        [Test]
        public void HandleLegendGraphic_WhenFormatParamInvalid_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["format_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&version=1.3.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleLegendGraphic_WhenFormatParamValid_ValidationShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetLegendGraphicValidationRules["format_param_valid"];
            var request = "http://some.url/?request=GetLegendGraphic&version=1.3.0&format=image/png".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleLegendGraphic_WhenRequestPassesValidation_CallsHandleGetLegendGraphicDriverSpecific()
        {
            var drv = MakeWmsDriver();
            var request = "http://some.url/?request=GetLegendGraphic&service=WMS&version=1.3.0&layer=someLayer&width=256&height=256&format=image/png".CreateHttpWebRequest();

            drv.HandleRequest(request);

            drv.HandleGetLegendGraphicDriverSpecificCalled.Should().BeTrue();
        }

        [Test]
        public void HandleLegendGraphic_WhenParamsOkButDriverSpecificGetLegendGraphicNotImplemented_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            drv.CallBaseHandleGetLegendGraphicDriverSpecific = true;
            var expectedMsg = "IMPLEMENTATION ERROR: Operation 'GetLegendGraphic' is marked as supported but it is not implemented.";

            var response = drv.HandleRequest("http://some.url/?request=GetLegendGraphic&service=WMS&version=1.3.0&layer=someLayer&width=256&height=256&format=image/png");

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

            drv.SupportedVendorOperations.Add("1.3.0", new List<string>() { "GetLegendGraphic" });
            drv.SupportedVendorOperationFormats.Add(
                "GetLegendGraphic", new Dictionary<string, List<string>>() { { "1.3.0", new List<string>() { "image/png", "image/jpeg", "image/gif" } } }
            );

            return drv;
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public bool HandleGetLegendGraphicDriverSpecificCalled { get; set; }

            public bool CallBaseHandleGetLegendGraphicDriverSpecific { get; set; }

            protected internal override IWmsDriverResponse HandleGetLegendGraphicDriverSpecific()
            {
                HandleGetLegendGraphicDriverSpecificCalled = true;

                if (CallBaseHandleGetLegendGraphicDriverSpecific)
                {
                    return base.HandleGetLegendGraphicDriverSpecific();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
