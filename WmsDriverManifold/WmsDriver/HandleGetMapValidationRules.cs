using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Wms;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> HandleGetMapValidationRulesDriverSpecific = new Dictionary
            <string, Action<WmsDriver>>()
        {
            {
                "crs_supported", (drv) =>
                {
                    var msg = "CRS not supported.";
                    var ec = WmsExceptionCode.InvalidCRS;

                    if (string.Compare(drv.GetParam<string>("crs"), "EPSG:" + drv.MSettings.SRID, drv.GetIgnoreCase()) != 0)
                        throw new WmsDriverException(msg, ec);
                }
            },
            {
                "styles_valid", (drv) =>
                {
                    var msg = "Invalid parameter STYLES: ";
                    var ec = WmsExceptionCode.NotApplicable;

                    var pValue = drv.GetParam("styles");

                    var styles = pValue.Split(',');

                    foreach (var style in styles)
                    {
                        if(!string.IsNullOrEmpty(style))
                            throw new WmsDriverException(msg + pValue, ec);
                    }
                }
            },
            {
                "layers_defined", (drv) =>
                {
                    var msg = "Layer '{0}' is not defined.";
                    var ec = WmsExceptionCode.LayerNotDefined;

                    //Extract layers
                    string[] inLayers = drv.GetParam("LAYERS").Split(',');


                    //first extract all the current map layer names
                    List<string> mapLayers = drv.ExtractMapLayers();


                    //verify if the layers are valid
                    foreach (var layer in inLayers)
                    {
                        if(!mapLayers.Contains(layer))
                            throw new WmsDriverException(string.Format(msg, layer), ec);
                    }
                }
            }

        };

    }
}
