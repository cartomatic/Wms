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

                    //Extract layers - this may be a single layer or a list of layers
                    string[] inLayers = drv.GetParam("layer").Split(',');

                    //first extract all the current map layer names
                    List<string> mapLayers = drv.ExtractMapLayers();

                    //verify if the layer are valid
                    foreach (var l in inLayers)
                    {
                        if (!mapLayers.Exists(ml => ml == l))
                            throw new WmsDriverException(string.Format(msg, l), ec);
                    }
                    
                }
            }
        };

    }
}
