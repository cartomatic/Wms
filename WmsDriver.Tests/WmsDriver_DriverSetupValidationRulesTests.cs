using System;
using System.Collections.Generic;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriver_DriverSetupValdationRulesTests
    {
        [Test]
        public void DriverSetup_IfInternalDataNotInitialised_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver();
            var testedValidationRule = drv.DriverSetupValidationRules["service_params_containers_instantiated"];

            drv.InvalidateInternalDataContainers();
            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        //need to have at least one supported version
        [Test]
        public void DriverSetup_WhenNoSupportedVersionsDefined_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addVersion: false);
            var testedValidationRule = drv.DriverSetupValidationRules["service_versions_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenNoGetCapabilitiesFormatsDefined_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addCapsFormat: false);
            var testedValidationRule = drv.DriverSetupValidationRules["getcaps_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenGetCapabilitiesFormatsDefinedPartially_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addCapsFormat: false, addCapsFormatWrong: true);
            var testedValidationRule = drv.DriverSetupValidationRules["getcaps_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenDefaultGetCapabilitiesFormatNotSpecifiedOrNotSupported_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addDefaultCapsFormat: false);
            var testedValidationRule = drv.DriverSetupValidationRules["default_getcaps_format_specified_and_supported"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenNoGetMapFormatsDefined_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addMapFormat: false);
            var testedValidationRule = drv.DriverSetupValidationRules["getmap_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenGetMapFormatsDefinedPartially_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addMapFormat: false, addMapFormatWrong: true);
            var testedValidationRule = drv.DriverSetupValidationRules["getmap_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenNoExceptionFormatsDefined_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addExceptionFormat: false);
            var testedValidationRule = drv.DriverSetupValidationRules["exception_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenExceptionFormatsDefinedPartially_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addExceptionFormat: false, addExceptionFormatWrong: true);
            var testedValidationRule = drv.DriverSetupValidationRules["exception_formats_specified"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void DriverSetup_WhenDefaultExceptionFormatNotSpecifiedOrNotSupported_DriverSetupValidationRuleThrows()
        {
            var drv = MakeWmsDriver(addDefaultExceptionFormat: false);
            var testedValidationRule = drv.DriverSetupValidationRules["default_exception_format_specified_and_supported"];

            Action action = () => testedValidationRule(drv);

            action.ShouldThrow<WmsDriverException>();
        }

        private FakeWmsDriver MakeWmsDriver(bool addVersion = true, bool addCapsFormat = true, bool addCapsFormatWrong = false, bool addDefaultCapsFormat = true, bool addMapFormat = true, bool addMapFormatWrong = false, bool addExceptionFormat = true, bool addExceptionFormatWrong = false, bool addDefaultExceptionFormat = true)
        {
            var drv = new FakeWmsDriver();

            if (addVersion)
                drv.SupportedVersions.Add("1.3.0");

            if (addCapsFormat)
                drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() { "XML" });

            if (addCapsFormatWrong)
                drv.SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>());

            if (addDefaultCapsFormat)
                drv.DefaultGetCapabilitiesFormats.Add("1.3.0", "XML");

            if (addMapFormat)
                drv.SupportedGetMapFormats.Add("1.3.0", new List<string>() { "image/png" });

            if (addMapFormatWrong)
                drv.SupportedGetMapFormats.Add("1.3.0", new List<string>());

            if (addExceptionFormat)
                drv.SupportedExceptionFormats.Add("1.3.0", new List<string>() { "XML" });

            if (addExceptionFormatWrong)
                drv.SupportedExceptionFormats.Add("1.3.0", new List<string>());

            if (addDefaultExceptionFormat)
                drv.DefaultExceptionFormats.Add("1.3.0", "XML");

            return drv;
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public void InvalidateInternalDataContainers()
            {
                SupportedVersions = null;
            }
        }
    }
}
