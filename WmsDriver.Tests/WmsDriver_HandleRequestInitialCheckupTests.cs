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

            var response = drv.HandleRequest(string.Empty);

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

            var response = drv.HandleRequest(string.Empty);

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenGetCapabilitiesFormatsDefinedPartially_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addCapsFormat: false, addCapsFormatWrong: true);
            var testedValidationRule = drv.HandleRequestValidationRules["getcaps_format_specified"];

            var response = drv.HandleRequest(string.Empty);

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

            var response = drv.HandleRequest(string.Empty);

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        [Test]
        public void HandleRequest_WhenGetMapFormatsDefinedPartially_GeneratesWmsDriverException()
        {
            var drv = MakeWmsDriver(addMapFormat: false, addMapFormatWrong: true);
            var testedValidationRule = drv.HandleRequestValidationRules["getmap_format_specified"];

            var response = drv.HandleRequest(string.Empty);

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(testedValidationRule.WmsEcExceptionCode);
            response.WmsDriverException.Message.Should().Be(testedValidationRule.Message);
        }

        
        //need to verify if the request param has been provided
        //if the version param has been provided, then must match supported versions
        //if request is getcaps, then does not need the version but need to obtain the highest version supported

        //properly works out the highest version based on the supported versions

        //properly delegates get caps handling
        //properly delegates get map handling
        //properly delegates get feature info handling
        //properly delegates get legend graphics handling
        //properly delegates vendor ops handling
        //propery fails on a nont supported operation

        private Cartomatic.Wms.WmsDriver MakeWmsDriver(bool addVersion = true, bool addCapsFormat = true, bool addCapsFormatWrong = false, bool addMapFormat = true, bool addMapFormatWrong = false)
        {
            var drv = new FakeWmsDriverWithMissingData();

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

        private class FakeWmsDriverWithMissingData : Cartomatic.Wms.WmsDriver
        {
        }
    }
}
