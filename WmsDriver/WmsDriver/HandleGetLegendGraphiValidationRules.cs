using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> HandleGetLegendGraphicValidationRules = new Dictionary
            <string, Action<WmsDriver>>()
        {
            //An example GeoServer Request
            //http://localhost:8080/geoserver/wms?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=topp:states

            //the initial checks are performed based on the GeoServer Request

            //version and Request checked in the HandleRequest method
            //so need to chek format, size and layer param

            //layers param is mandatory
            {
                "layer_param_presence", (drv) =>
                {
                    var msg = "Required parameter LAYER not specified.";
                    var ec = WmsExceptionCode.NotApplicable;

                    if (string.IsNullOrEmpty(drv.GetParam<string>("layer")))
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
            
                    if(string.IsNullOrEmpty(drv.GetParam<string>("format")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //width should be int parsable more than 0 and less or equal to max width allowed
            {
                "width_param_valid", (drv) =>
                {
                    var width = drv.GetParam<int?>("width");
                    if (!width.HasValue)
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
                    var height = drv.GetParam<int?>("height");
                    if (!height.HasValue)
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
                    var op = "GetLegendGraphic";
                    var version = drv.GetParam<string>("version");
                    var format = drv.GetParam<string>("format");



                    if (!drv.SupportedVendorOperationFormats.ContainsKey(op) || drv.SupportedVendorOperationFormats[op] == null || !drv.SupportedVendorOperationFormats[op].ContainsKey(version) || !drv.SupportedVendorOperationFormats[op][version].Exists(f => f == format))
                        throw new WmsDriverException(
                            string.Format("Invalid FORMAT parameter. Supported {0} formats for version {1} of the service are: {2}.", op, version, string.Join(", ", drv.SupportedVendorOperationFormats[op][version])), WmsExceptionCode.InvalidFormat
                        );
                }
            }
        };
    }
}
