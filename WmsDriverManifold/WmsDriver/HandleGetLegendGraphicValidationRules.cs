using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> HandleGetLegendGraphicValidationRulesDriverSpecific = new Dictionary
            <string, Action<WmsDriver>>()
        {
            {
                "layer_defined", (drv) =>
                {
                    var msg = "Layer '{0}' is not defined.";
                    var ec = WmsExceptionCode.LayerNotDefined;

                    //Extract layers
                    var inLayer = drv.GetParam("LAYER");


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

                    //verify if the layer are valid
                    if(!mapLayers.Exists(ml => ml == inLayer))
                        throw new WmsDriverException(string.Format(msg, inLayer), ec);
                }
            }
        };

    }
}
