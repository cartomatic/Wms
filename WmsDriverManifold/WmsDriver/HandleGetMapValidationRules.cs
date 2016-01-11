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

                    //also add the root layer name to the layers list
                    //as the map (root layer) can also be requested
                    mapLayers.Add(drv.ServiceDescription.Title);

                    //Note:
                    //This is actually against the specs... The proper way of requesting all layers
                    //is to use the top level layer in the layers param.
                    //To do so though one needs to know the particular layer names.
                    //
                    //So in order to enable rquesting * layers without knowing their names and the name
                    //of the map / root layer add *
                    //
                    //And DO REMEMBER this is not the WMS allowed way and on some occassions may make dependant apps fail
                    mapLayers.Add("*");


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
