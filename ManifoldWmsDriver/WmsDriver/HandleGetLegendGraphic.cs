using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Drawing;
using Cartomatic.Wms;

using M = Manifold.Interop;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        protected override IWmsDriverResponse HandleGetLegendGraphicDriverSpecific()
        {
            Validate(HandleGetLegendGraphicValidationRulesDriverSpecific);

            var output = new WmsDriverResponse();

            //Note:
            //Basically, the driver for this functionality is GeoServer

            //An example GeoServer Request
            //http://localhost:8080/geoserver/wms?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=topp:states

            //GetLegendGraphics should return an image for a requested layer. With manifold though this is a bit cumbersome.
            //as mapserver renders a bit more than just an image of a legend entry
            //TODO - verify the above. check if possible to render just the graphics, or to crop it. Maybe a setting in the driver cfg could help in order to just return images for single layers
            
            var map = MapServer.Component as M.Map;

            //extract layer(s) - can be a single layer or a set of layers
            string[] inLayers = GetParam("layer").Split(',');



            //turn layers on / off
            //Note: legend is generated from the map and takes into account the visibility of map layers, not the 
            //map server layers. hence a bit different way of setting layer visibility than in GetMap


            //handle layers visibility
            if (inLayers.Contains("*") || inLayers.Contains(map.Name) || inLayers.Contains(ServiceDescription.Title))
            {
                //this is a request to turn all the layers on
                //all layers were requested (*), this is a map (combine layers) or a root layer
                foreach (M.Layer l in map.LayerSet)
                {
                    //turn all the layers but labels
                    l.Visible = l.Component.Type != M.ComponentType.ComponentLabels;
                }
            }
            else
            {
                //not a request to turn all the layers on, so need to review them one by one
                //if layer requested then turn it on otherwise turn it off
                foreach (M.Layer l in map.LayerSet)
                {
                    l.Visible = inLayers.Contains(l.Component.Name);
                }
            }

            ImageCodecInfo imageEncoder = GetParam("FORMAT").GetEncoderInfo();
            MapServer.RenderFormat = GetMapServerRenderFormat(imageEncoder.MimeType);

            //Note:
            //map server just renders a legend, so although the GetLegendGraphic does require width and height they are ignored at this stage
            //not perfect, but it can wait a bit.

            var legendBinary = MapServer.RenderLegend() as byte[];

            //TODO - after rendering make sure to cut the specified portions of image if required - will need a cfg property for this


            output.ResponseContentType = imageEncoder.MimeType;
            output.ResponseBinary = legendBinary;

            return output;
        }
    }
}
