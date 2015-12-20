using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> DriverSetupValidationRules = new Dictionary
            <string, Action<WmsDriver>>()
        {
            {
                "service_params_containers_instantiated", (drv) =>
                {
                    var msg =
                        "SETUP ERROR: Some of the following objects were not initialised: SupportedGetCapabilitiesFormats, DefaultGetCapabilitiesFormats, SupportedGetMapFormats, SupportedGetFeatureInfoFormats, SupportedExceptionFormats, DefaultExceptionFormats, SupportedVersions.";
                    var ec = WmsExceptionCode.NotApplicable;

                    var containers = new List<object>()
                    {
                        drv.SupportedGetCapabilitiesFormats,
                        drv.DefaultGetCapabilitiesFormats,
                        drv.SupportedGetMapFormats,
                        drv.SupportedGetFeatureInfoFormats,
                        drv.SupportedExceptionFormats,
                        drv.DefaultExceptionFormats,
                        drv.SupportedVersions
                    };

                    if (containers.Any(c => c == null))
                        throw new WmsDriverException(msg, ec);
                }
            },
            //Driver must specify at least one service version it supports
            { "service_versions_specified", (drv) =>
                {
                    var msg = "SETUP ERROR: Driver must specify at least one service VERSION it supports.";
                    var ec = WmsExceptionCode.NotApplicable;
                    if(drv.SupportedVersions.Count == 0)
                        throw new WmsDriverException(msg, ec);
                }
            },

            //driver must define at least one GetCapabilities format for each supported service version
            {
                "getcaps_formats_specified", (drv) =>
                {
                    var msg = "SETUP ERROR: Driver must specify at least one GetCapabilities format for each supported service version.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if (drv.SupportedGetCapabilitiesFormats == null || drv.SupportedGetCapabilitiesFormats.Count == 0)
                        throw new WmsDriverException(msg, ec);

                    foreach (var version in drv.SupportedVersions)
                    {
                        if (!drv.SupportedGetCapabilitiesFormats.ContainsKey(version))
                            throw new WmsDriverException(msg, ec);

                        if (drv.SupportedGetCapabilitiesFormats[version] == null || drv.SupportedGetCapabilitiesFormats[version].Count == 0)
                            throw new WmsDriverException(msg, ec);
                    }
                }
            },
            { "default_getcaps_format_specified_and_supported", (drv) =>
                {
                    var msg = "SETUP ERROR: One of the default caps format does not seem to be supported.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    foreach (var version in drv.SupportedVersions)
                    {
                        if(!drv.DefaultGetCapabilitiesFormats.ContainsKey(version) || !drv.SupportedGetCapabilitiesFormats[version].Contains(drv.DefaultGetCapabilitiesFormats[version]))
                            throw new WmsDriverException(msg, ec);
                    }
                }
            },       

            //driver must define at least one GetMap format for each supported service version
            {
                "getmap_formats_specified", (drv) =>
                {
                    var msg = "SETUP ERROR: Driver must specify at least one GetMap format for each supported service version.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if (drv.SupportedGetMapFormats == null || drv.SupportedGetMapFormats.Count == 0)
                        throw new WmsDriverException(msg, ec);

                    foreach (var version in drv.SupportedVersions)
                    {
                        if (!drv.SupportedGetMapFormats.ContainsKey(version))
                            throw new WmsDriverException(msg, ec);

                        if (drv.SupportedGetMapFormats[version] == null || drv.SupportedGetMapFormats[version].Count == 0)
                            throw new WmsDriverException(msg, ec);
                    }
                }
            },


            { "exception_formats_specified", (drv) =>
                {
                    var msg = "SETUP ERROR: Driver must specify at least one Exception format for each supported version.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if (drv.SupportedExceptionFormats == null || drv.SupportedExceptionFormats.Count == 0)
                        throw new WmsDriverException(msg, ec);


                    foreach (var version in drv.SupportedVersions)
                    {
                        if (!drv.SupportedExceptionFormats.ContainsKey(version))
                            throw new WmsDriverException(msg, ec);

                        if (drv.SupportedExceptionFormats[version] == null || drv.SupportedExceptionFormats[version].Count == 0)
                            throw new WmsDriverException(msg, ec);
                    }
                }
            },    
            { "default_exception_format_specified_and_supported", (drv) =>
                {
                    var msg = "SETUP ERROR: Driver must specify a defualt exception format for each version specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    foreach (var version in drv.SupportedVersions)
                    {
                        if (!drv.DefaultExceptionFormats.ContainsKey(version) || !drv.SupportedExceptionFormats[version].Contains(drv.DefaultExceptionFormats[version]))
                            throw new WmsDriverException(msg, ec);
                    }
                }
            }
        };
    }
}
