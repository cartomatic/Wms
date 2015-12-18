using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriver_HandleRequestInitialCheckupTests
    {
        //need to have at least one supported version
        [Test]
        public void HandleRequest_WhenNoSupportedVersionsDefined_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addVersion: false);
            var testedValidationRule = drv.HandleRequestValidationRules["service_version_specified"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        //need to have at least one get capabilities format
        [Test]
        public void HandleRequest_WhenNoGetCapabilitiesFormatsDefined_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addCapsFormat: false);
            var testedValidationRule = drv.HandleRequestValidationRules["getcaps_format_specified"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenGetCapabilitiesFormatsDefinedPartially_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addCapsFormat: false, addCapsFormatWrong: true);
            var testedValidationRule = drv.HandleRequestValidationRules["getcaps_format_specified"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        //need to have at least one get map format
        //need to have at least one get capabilities format
        [Test]
        public void HandleRequest_WhenNoGetMapFormatsDefined_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addMapFormat: false);
            var testedValidationRule = drv.HandleRequestValidationRules["getmap_format_specified"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenGetMapFormatsDefinedPartially_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addMapFormat: false, addMapFormatWrong: true);
            var testedValidationRule = drv.HandleRequestValidationRules["getmap_format_specified"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenRequestParamNotFound_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["request_param_present"];

            var response = drv.HandleRequest("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenVersionParamDoesNotMatchServiceVersionsDeclared_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["version_param_matches_supported_versions"];

            var response = drv.HandleRequest("http://some.url/?request=GetWhatever&version=0.0.0");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(string.Format(testedValidationRule.Message, string.Concat(", ", drv.SupportedVersions)));
        }


        
        [Test]
        public void HandleRequest_WhenVersionNotProvidedAndRequestIsNotGetCapabilities_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["no_version_request_must_be_getcaps"];

            var response = drv.HandleRequest("http://some.url/?request=GetWhatever");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(string.Format(testedValidationRule.Message, string.Concat(", ", drv.SupportedVersions)));
        }

        //properly delegates get caps handling
        [Test]
        public void HandleRequest_WhenRequestIsGetCapabilities_CallsHandleGetCapabilities()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=GetCapabilities");

            drv.HandleGetCapabilitiesCalled.Should().BeTrue();
        }

        //properly delegates get map handling
        [Test]
        public void HandleRequest_WhenRequestIsGetMap_CallsHandleGetMap()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=GetMap");

            drv.HandleGetMapCalled.Should().BeTrue();
        }

        //properly delegates get feature info handling
        [Test]
        public void HandleRequest_WhenRequestIsGetFeatureInfo_CallsHandleGetFeatureInfo()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=GetFeatureInfo");

            drv.HandleGetFeatureInfoCalled.Should().BeTrue();
        }

        //properly delegates get legend graphics handling
        [Test]
        public void HandleRequest_WhenRequestIsGetLegendGraphics_CallsHandleGetLegendGraphics()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=GetLegendGraphic");

            drv.HandleGetLegendGraphicCalled.Should().BeTrue();
        }

        //propery fails on a nont supported operation
        [Test]
        public void HandleRequest_WhenRequestIsUnsupported_CallsHandleUnsupported()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=MyUnsupportedOp");

            drv.HandleUnsupportedCalled.Should().BeTrue();
            drv.UnsupportedOp.Should().Be("MyUnsupportedOp");
        }

        [Test]
        public void HandleRequest_WhenRequestIsNotSupportedAndNoCustomVendorHandlerIsDefined_GenratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var request = "ThisOpDoesNotHaveAHandler"; 

            var response = drv.HandleRequest(string.Format("http://some.url/?version=1.3.0&request={0}", request));

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(string.Format("Operation '{0}' is not supported.", request));
        }

        [Test]
        public void HandleRequest_WhenRequestIsVendorOp_CallsHandleVendorOp()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=SomeVendorOp");

            drv.HandleVendorOpCalled.Should().BeTrue();
        }

        //properly delegates vendor ops handling
        [Test]
        public void HandleRequest_WhenRequestIsVendorOp_CallsAppropriateHandleVendorOp()
        {
            var drv = MakeWmsDriver();

            drv.HandleRequest("http://some.url/?version=1.3.0&request=SomeVendorOp");

            drv.HandleCustomVendorOpCalled.Should().BeTrue();
        }

        

        private FakeWmsDriver MakeWmsDriver(bool addVersion = true, bool addCapsFormat = true, bool addCapsFormatWrong = false, bool addMapFormat = true, bool addMapFormatWrong = false)
        {
            var drv = new FakeWmsDriver();

            if(addVersion)
                drv.SupportedVersions.Add("1.3.0");

            if(addCapsFormat)
                drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>(){"XML"});

            if(addCapsFormatWrong)
                drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() { });

            if(addMapFormat)
                drv.SupportedGetMapFormats.Add("1.3.0", new List<string>(){"image/png"});

            if (addMapFormatWrong)
                drv.SupportedGetMapFormats.Add("1.3.0", new List<string>() { });

            return drv;
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public bool HandleGetCapabilitiesCalled { get; set; }

            public bool HandleGetMapCalled { get; set; }

            public bool HandleGetFeatureInfoCalled { get; set; }

            public bool HandleGetLegendGraphicCalled { get; set; }

            public bool HandleVendorOpCalled { get; set; }

            public bool HandleCustomVendorOpCalled { get; set; }

            public bool HandleUnsupportedCalled { get; set; }

            public string UnsupportedOp { get; set; }


            protected override IWmsDriverResponse HandleGetCapabilities()
            {
                HandleGetCapabilitiesCalled = true;
                return new WmsDriverResponse();
            }

            protected override IWmsDriverResponse HandleGetMap()
            {
                HandleGetMapCalled = true;
                return new WmsDriverResponse();
            }

            protected override IWmsDriverResponse HandleGetFeatureInfo()
            {
                HandleGetFeatureInfoCalled = true;
                return new WmsDriverResponse();
            }

            protected override IWmsDriverResponse HandleGetLegendGraphic()
            {
                HandleGetLegendGraphicCalled = true;
                return new WmsDriverResponse();
            }

            protected override IWmsDriverResponse HandleUnsupported(string operation, bool tryVendorOp = true)
            {
                HandleUnsupportedCalled = true;
                UnsupportedOp = operation;

                return base.HandleUnsupported(operation, tryVendorOp);
            }


            protected override IWmsDriverResponse HandleVendorOp(string op)
            {
                HandleVendorOpCalled = true;
                return base.HandleVendorOp(op);
            }

            /// <summary>
            /// a custom vendor op handler; resolved by the driver through reflection
            /// </summary>
            /// <returns></returns>
            protected IWmsDriverResponse HandleSomeVendorOp()
            {
                HandleCustomVendorOpCalled = true;
                return new WmsDriverResponse();
            }
        }
    }
}
