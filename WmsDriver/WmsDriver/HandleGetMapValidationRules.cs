using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> HandleGetMapValidationRules = new Dictionary
            <string, Action<WmsDriver>>()
        {
            //layers param is mandatory
            {
                "layers_param_presence", (drv) =>
                {
                    var msg = "Required parameter LAYERS not specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(string.IsNullOrEmpty(drv.GetParam("layers")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //styles param is mandatory
            {
                "styles_param_presence", (drv) =>
                {
                    var msg = "Required parameter STYLES not specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(drv.GetParam("styles") == null)
                        throw new WmsDriverException(msg, ec);
                }
            },

            //crs / srs param is mandatory - depends on version!
            {
                "crssrs_param_presence", (drv) =>
                {
                    //version should have been tested in the initial request validation procedure
                    int version = int.Parse(drv.GetParam("version").Replace(".", ""));

                    if (version >= 130)
                    {
                        if (string.IsNullOrEmpty(drv.GetParam("crs")))
                            throw new WmsDriverException("Required parameter CRS not specified.", WmsExceptionCode.NotApplicable);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(drv.GetParam("srs")))
                            throw new WmsDriverException("Required parameter SRS not specified.", WmsExceptionCode.NotApplicable);
                    }
                }
            },

            //bbox param is mandatory
            {
                "bbox_param_presence", (drv) =>
                {
                    var msg = "Required parameter BBOX not specified.";
                    var ec = WmsExceptionCode.MissingDimensionValue;
            
                    if(string.IsNullOrEmpty(drv.GetParam("bbox")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //width param is mandatory
            {
                "width_param_presence", (drv) =>
                {
                    var msg = "Required parameter WIDTH not specified.";
                    var ec = WmsExceptionCode.MissingDimensionValue;
            
                    if(string.IsNullOrEmpty(drv.GetParam("width")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //height param is mandatory
            {
                "height_param_presence", (drv) =>
                {
                    var msg = "Required parameter HEIGHT not specified.";
                    var ec = WmsExceptionCode.MissingDimensionValue;
            
                    if(string.IsNullOrEmpty(drv.GetParam("height")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //format param is mandatory
            {
                "format_param_presence", (drv) =>
                {
                    var msg = "Required parameter FORMAT not specified.";
                    var ec = WmsExceptionCode.InvalidFormat;
            
                    if(string.IsNullOrEmpty(drv.GetParam("format")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //width should be int parsable more than 0 and less or equal to max width allowed
            {
                "width_param_valid", (drv) =>
                {
                    int width;
                    if (!int.TryParse(drv.GetParam("width"), out width))
                        throw new WmsDriverException("Invalid parameter WIDTH.", WmsExceptionCode.InvalidDimensionValue);

                    if (width <= 0)
                        throw new WmsDriverException("Parameter WIDTH must be larger than 0.", WmsExceptionCode.InvalidDimensionValue);

                    if(drv.ServiceDescription.MaxWidth.HasValue && width > drv.ServiceDescription.MaxWidth)
                        throw new WmsDriverException(string.Format("Parameter WIDTH too large. Max allowed WIDTH is {0}.", drv.ServiceDescription.MaxWidth), WmsExceptionCode.InvalidDimensionValue);
                }
            },

            //width should be int parsable more than 0 and less or equal to max width allowed
            {
                "height_param_valid", (drv) =>
                {
                    int height;
                    if (!int.TryParse(drv.GetParam("height"), out height))
                        throw new WmsDriverException("Invalid parameter HEIGHT.", WmsExceptionCode.InvalidDimensionValue);

                    if (height <= 0)
                        throw new WmsDriverException("Parameter HEIGHT must be larger than 0.", WmsExceptionCode.InvalidDimensionValue);

                    if(drv.ServiceDescription.MaxHeight.HasValue && height > drv.ServiceDescription.MaxHeight)
                        throw new WmsDriverException(string.Format("Parameter HEIGHT too large. Max allowed WIDTH is {0}.", drv.ServiceDescription.MaxHeight), WmsExceptionCode.InvalidDimensionValue);
                }
            },

            {
                "format_param_valid", (drv) =>
                {
                    var version = drv.GetParam("version");
                    var format = drv.GetParam("format");

                    if(!drv.SupportedGetMapFormats[version].Exists(f =>f == format))
                        throw new WmsDriverException(
                            string.Format("Invalid FORMAT parameter. Supported formats for version {0} of the service are: {1}.", version, string.Join(", ", drv.SupportedGetMapFormats[version])), WmsExceptionCode.InvalidFormat
                        );
                }
            },
        };
    }
}
