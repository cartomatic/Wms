using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        protected internal Dictionary<string, Action<Wms2WmtsProxy>> HandleGetMapValidationRulesDriverSpecific = new Dictionary
            <string, Action<Wms2WmtsProxy>>()
        {
            {
                "styles_valid", (drv) =>
                {
                    var msg = "Invalid parameter STYLES: ";
                    var ec = WmsExceptionCode.NotApplicable;

                    var pValue = drv.GetParam("styles");

                    var styles = pValue.Split(',');

                    //just check for the non-empty styles
                    foreach (var style in styles)
                    {
                        if(!string.IsNullOrEmpty(style))
                            throw new WmsDriverException(msg + pValue, ec);
                    }
                }
            },
            {
                "layers_only_one_requested", (drv) =>
                {
                    var msg = "Too many layers requested. Layer limit is {0}.";
                    var ec = WmsExceptionCode.LayerNotDefined;

                    //Extract layers
                    string[] inLayers = drv.GetParam("LAYERS").Split(',');

                    if(inLayers.Length > (drv.ServiceDescription.LayerLimit ?? 1))
                         throw new WmsDriverException(string.Format(msg, drv.ServiceDescription.LayerLimit ?? 1), ec);
                }
            },
            {
                "layers_defined", (drv) =>
                {
                    var msg = "Layer '{0}' is not defined.";
                    var ec = WmsExceptionCode.LayerNotDefined;

                    //Extract layers
                    string[] inLayers = drv.GetParam("LAYERS").Split(',');


                    //extract all the current map layer names
                    var wmtsCache = drv.GetCachedWmtsCaps(drv.GetBaseUrl());

                    List<string> mapLayers = wmtsCache.Contents.LayerSet.Aggregate(new List<string>(), (agg, l) =>
                    {
                        agg.Add(l.Title.FirstOrDefault()?.Value);
                        return agg;
                    });

                    //verify if the layers are valid
                    foreach (var layer in inLayers)
                    {
                        if(!mapLayers.Contains(layer))
                            throw new WmsDriverException(string.Format(msg, layer), ec);
                    }
                }
            },
            {
                "crs_supported", (drv) =>
                {
                    var msg = "CRS not supported.";
                    var ec = WmsExceptionCode.InvalidCRS;

                    //basically can call only one layer in wmts (or the themes), but since layers are to be cached somehow can request one layer at a time
                    string inLayer = drv.GetParam("LAYERS").Split(',').First();

                    //crs support is done per layer, so need to extract a layer firs and a list of supported crss for that layer 
                    //basically if got here there should be a wmts layer available
                    var wmtsCache = drv.GetCachedWmtsCaps(drv.GetBaseUrl());
                    var supportedCrs = wmtsCache.Contents.LayerSet
                        .FirstOrDefault(l => l.Title.FirstOrDefault()?.Value == inLayer)?.Items.Aggregate(new List<string>(), (agg, bbox) =>
                        {
                            agg.Add("EPSG:" + bbox.crs.Split(':').Last());
                            return agg;
                        });

                    if (!supportedCrs.Any(crs => string.Compare(drv.GetParam<string>("crs"), crs, drv.GetIgnoreCase()) != 0))
                        throw new WmsDriverException(msg, ec);
                }
            }
        };
    }
}
