using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Utils.Web;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriver_HandleCapabilitiesTests
    {
        //need to have at least one supported version
        [Test]
        public void HandleGetCapabilities_WhenNoServiceParamProvided_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetCapabilitiesValidationRules["service_param_presence"];
            var request = "http://some.url/?request=GetCapabilities".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }
        
        //service must be WMS
        [Test]
        public void HandleGetCapabilities_WhenServiceOtherThanWms_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetCapabilitiesValidationRules["service_must_be_wms"];
            var request = "http://some.url/?request=GetCapabilities&service=NotWMS".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleGetCapabilities_WhenServiceParamIsWMS_ValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetCapabilitiesValidationRules["service_param_presence"];
            var request = "http://some.url/?request=GetCapabilities&service=WMS".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        //format if provided must match get capabilties format!
        [Test]
        public void HandleGetCapabilities_WhenFormatProvidedDoesNotMatchSupported_ValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetCapabilitiesValidationRules["format_must_be_valid"];
            var request = "http://some.url/?request=GetCapabilities&service=WMS&format=NotValid".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleGetCapabilities_WhenFormatProvidedASupported_ValidationShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleGetCapabilitiesValidationRules["format_must_be_valid"];
            var request = "http://some.url/?request=GetCapabilities&service=WMS&format=XML".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.ShouldNotThrow();
        }

        [Test]
        public void HandleGetCapabilities_WhenParamsOk_CallsTheSubsequentMethodsStack()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?request=GetCapabilities&service=WMS");

            drv.GenerateWmsCapabilitiesDocument130Called.Should().BeTrue();
            drv.GenerateCapsServiceSection130Called.Should().BeTrue();
            drv.GenerateCapsCapabilitiesSection130Called.Should().BeTrue();
            drv.GenerateCapsLayersSection130Called.Should().BeTrue();
        }

        [Test]
        public void HandleGetCapabilities_WhenParamsOkAndGenerateLayersSectionNotImplemented_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var expectedMsg = "IMPLEMENTATION ERROR: GetCapabilities is a mandatory operation for WMS 1.3.0.";

            var response = drv.HandleRequest("http://some.url/?request=GetCapabilities&service=WMS");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(expectedMsg);
        }

        [Test]
        public void GetPublicUrl_WhenPublicUrlSpecifiedInWmsServiceDescription_UsesIt()
        {
            var drv = MakeWmsDriver();
            drv.ServiceDescription = new WmsServiceDescription()
            {
                PublicAccessURL = "http://my.test.url"
            };

            drv.GetPublicAccessURL().Should().Be(drv.ServiceDescription.PublicAccessURL);
        }

        [Test]
        public void GetPublicUrl_WhenPublicUrlIsNotDefinedByInWmsServiceDescription_WorksOutUrlBasedOnRequestUrl()
        {
            var drv = MakeWmsDriver();
            var requestUrl = "http://request.url:8888/using/a/nested/path";
            var request = requestUrl.CreateHttpWebRequest();

            drv.ServiceDescription = new WmsServiceDescription();
            drv.ExtractRequestParams(request);

            drv.GetPublicAccessURL().Should().Be(requestUrl);
        }

        //get caps must be created, response type must be same as requested
        [Test]
        public void HandleGetCapabilities_WhenParamsOk_GeneratesCapabilitiesXml()
        {
            var drv = MakeWmsDriver();

            //avoid calling base, as it throws on purpose in the default abstract class implementation
            drv.CallBaseInGenerateCapsLayersSection130 = false;

            var response = drv.HandleRequest("http://some.url/?request=GetCapabilities&service=WMS");

            response.ResponseContentType.Should().Be("text/xml");
            response.ResponseText.Should().NotBeNullOrWhiteSpace();
            response.ResponseText.Should().Contain("<WMS_Capabilities");
        }

        

        private FakeWmsDriver MakeWmsDriver()
        {
            var drv = new FakeWmsDriver();

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
            public bool GenerateWmsCapabilitiesDocument130Called { get; set; }
        
            public bool GenerateCapsServiceSection130Called { get; set; }

            public bool GenerateCapsCapabilitiesSection130Called { get; set; }

            public bool GenerateCapsLayersSection130Called { get; set; }


            /// <summary>
            /// by default call base in GenerateCapsLayersSection130 so it properly throws a WmsException;
            /// flag makes it possible to not call the base, so can avoid exception
            /// </summary>
            public bool CallBaseInGenerateCapsLayersSection130 = true;

            protected override WMS_Capabilities GenerateWmsCapabilitiesDocument130()
            {
                GenerateWmsCapabilitiesDocument130Called = true;
                return base.GenerateWmsCapabilitiesDocument130();
            }

            protected override WMS_Capabilities GenerateCapsServiceSection130(WMS_Capabilities capsDoc)
            {
                GenerateCapsServiceSection130Called = true;
                return base.GenerateCapsServiceSection130(capsDoc);
            }

            protected override WMS_Capabilities GenerateCapsCapabilitiesSection130(WMS_Capabilities capsDoc)
            {
                GenerateCapsCapabilitiesSection130Called = true;
                return base.GenerateCapsCapabilitiesSection130(capsDoc);
            }

            protected override WMS_Capabilities GenerateCapsLayersSection130(WMS_Capabilities capsDoc)
            {
                GenerateCapsLayersSection130Called = true;
                if (CallBaseInGenerateCapsLayersSection130)
                {
                    return base.GenerateCapsLayersSection130(capsDoc);
                }
                else
                {
                    return capsDoc;
                }
            }
        }
    }
}
