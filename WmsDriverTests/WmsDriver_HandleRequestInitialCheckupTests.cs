using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Utils.Web;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    public class WmsDriver_HandleRequestInitialCheckupTests
    {

        [Test]
        public void HandleRequest_WhenRequestParamNotFound_InitialRequestValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["request_param_present"];
            var request = "http://some.url/".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenRequestParamPresent_InitialRequestValidationShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["request_param_present"];
            var request = "http://some.url/?request=SomeRequest".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleRequest_WhenVersionParamDoesNotMatchServiceVersionsDeclared_InitialRequestValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["version_param_matches_supported_versions"];
            var request = "http://some.url/?request=GetWhatever&version=0.0.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenVersionParamMatchesServiceVersionsDeclared_InitialRequestValidationRuleShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["version_param_matches_supported_versions"];
            var request = "http://some.url/?request=GetWhatever&version=1.3.0".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        [Test]
        public void HandleRequest_WhenVersionNotProvidedAndRequestIsNotGetCapabilities_InitialRequestValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["no_version_request_must_be_getcaps"];
            var request = "http://some.url/?request=GetWhatever".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenVersionNotProvidedAndRequestIsGetCapabilities_InitialRequestShouldNotThrow()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.HandleRequestValidationRules["no_version_request_must_be_getcaps"];
            var request = "http://some.url/?request=GetCapabilities".CreateHttpWebRequest();
            drv.ExtractRequestParams(request);

            Action action = () => testedValidationRule(drv);

            action.Should().NotThrow();
        }

        //properly delegates get caps handling
        [Test]
        public async Task HandleRequest_WhenRequestIsValidGetCapabilities_CallsHandleGetCapabilities()
        {
            var drv = MakeWmsDriver();

            await drv.HandleRequestAsync("http://some.url/?version=1.3.0&request=GetCapabilities");

            drv.HandleGetCapabilitiesCalled.Should().BeTrue();
        }

        //properly delegates get map handling
        [Test]
        public async Task HandleRequest_WhenRequestIsGetMap_CallsHandleGetMap()
        {
            var drv = MakeWmsDriver();

            await drv.HandleRequestAsync("http://some.url/?version=1.3.0&request=GetMap");

            drv.HandleGetMapCalled.Should().BeTrue();
        }

        //properly delegates get feature info handling
        [Test]
        public async Task HandleRequest_WhenRequestIsGetFeatureInfo_CallsHandleGetFeatureInfo()
        {
            var drv = MakeWmsDriver();

            await drv.HandleRequestAsync("http://some.url/?version=1.3.0&request=GetFeatureInfo");

            drv.HandleGetFeatureInfoCalled.Should().BeTrue();
        }

        
        [Test]
        public async Task HandleRequest_WhenRequestIsUnsupported_CallsHandleUnsupported()
        {
            var drv = MakeWmsDriver();
            var op = "MyUnsupportedOp";

            await drv.HandleRequestAsync(string.Format("http://some.url/?version=1.3.0&request={0}", op));

            drv.HandleUnsupportedCalled.Should().BeTrue();
            drv.UnsupportedOp.Should().Be(op);
        }

        [Test]
        public async Task HandleRequest_WhenRequestIsVendorOp_CallsHandleVendorOp()
        {
            var drv = MakeWmsDriver();

            await drv.HandleRequestAsync("http://some.url/?version=1.3.0&request=SomeVendorOp");

            drv.HandleVendorOpCalled.Should().BeTrue();
        }

        
        [Test]
        public async Task HandleRequest_WhenRequestIsVendorOpButOpIsNotSupported_GenratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var request = "ThisOpIsNotSupported"; //note: see supported vendor op in drv constructor below.

            var response = await drv.HandleRequestAsync(string.Format("http://some.url/?version=1.3.0&request={0}", request));

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(string.Format("Operation '{0}' is not supported.", request));
        }

        [Test]
        public async Task HandleRequest_WhenRequestIsVendorOpAndIsSupportedButDoesNotHaveHandler_GenratesWmsDriverException()
        {
            var drv = MakeWmsDriver();
            var request = "ThisOpIsSupportedButDoesNotHaveHandler"; //note: see supported vendor op in drv constructor below.

            var response = await drv.HandleRequestAsync(string.Format("http://some.url/?version=1.3.0&request={0}", request));

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(string.Format("IMPLEMENTATION ERROR: Operation '{0}' is marked as supported but it is not implemented.", request));
        }

        //properly delegates vendor ops handling
        [Test]
        public async Task HandleRequest_WhenRequestIsVendorOpAndOpIsSupportedAndAHandlerIsDefined_CallsAppropriateHandleVendorOp()
        {
            var drv = MakeWmsDriver();
            var op = "SomeVendorOp"; //note: see supported vendor op in drv constructor below.

            await drv.HandleRequestAsync(string.Format("http://some.url/?version=1.3.0&request={0}", op));

            drv.HandleCustomVendorOpCalled.Should().BeTrue();
        }

        private FakeWmsDriver MakeWmsDriver()
        {
            var drv = new FakeWmsDriver();

            drv.SupportedVersions.Add("1.3.0");
            drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() { "XML" });
            drv.DefaultGetCapabilitiesFormats.Add("1.3.0", "XML");

            drv.SupportedGetMapFormats.Add("1.3.0", new List<string>() { "image/png" });

            drv.SupportedExceptionFormats.Add("1.3.0", new List<string>() { "XML" });
            drv.DefaultExceptionFormats.Add("1.3.0", "XML");

            drv.SupportedVendorOperations.Add("1.3.0", new List<string>() { "SomeVendorOp", "ThisOpIsSupportedButDoesNotHaveHandler" });

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


            protected override async Task<IWmsDriverResponse> HandleGetCapabilitiesAsync()
            {
                HandleGetCapabilitiesCalled = true;
                return new WmsDriverResponse();
            }

            protected override async Task<IWmsDriverResponse> HandleGetMapAsync()
            {
                HandleGetMapCalled = true;
                return new WmsDriverResponse();
            }

            protected override async Task<IWmsDriverResponse> HandleGetFeatureInfoAsync()
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
