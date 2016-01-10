using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        protected override IWmsDriverResponse HandleGetMapDriverSpecific()
        {
            Validate(HandleGetMapValidationRulesDriverSpecific);


            IWmsDriverResponse output = new WmsDriverResponse();

            var width = GetParam<int?>("width");
            var height = GetParam<int?>("height");

            var backColor = Color.White;
            if (GetTransparent())
            {
                backColor = Color.Transparent;
            }
            else
            {
                var bgColor = GetParam("bgcolor");
                if (!string.IsNullOrEmpty(bgColor))
                {
                    //If got here, validation rule has already ensured the color is parsable!
                    backColor = ColorTranslator.FromHtml(GetParam("BGCOLOR"));
                }
            }

            //Get the image format requested
            //Note: this should have been checked in the base driver against formats supported
            ImageCodecInfo imageEncoder = GetEncoderInfo(GetParam("FORMAT"));


            var bbox = ParseBBOX(GetParam("bbox"), GetParam("version"), MSettings.SRID);


            //Note:
            //manifold WMS driver does not support styles so far as there is no such concept within manifold
            //Although themes linked with drawings could potentially be treated as styles but they would also have to be added as layers and that
            //kinda would makes styles useless again...
            //Could however add an option to hide themes and turn them on only if styles are requested. Not convinced though if its worth the effort... 


            //since got here, can start preparing for rendering
            M.Map map = (M.Map)MapServer.Component;


            //Extract layers
            string[] inLayers = GetParam("LAYERS").Split(',');


            //first extract all the current map layer names and at the same time turn them off
            List<string> mapLayers = new List<string>();

            if (MSettings.CombineLayers)
            {
                mapLayers.Add(map.Name);
            }
            else
            {
                foreach (M.Layer l in map.LayerSet)
                {
                    mapLayers.Add(l.Component.Name);
                    //this.mapServer.TurnLayer(l, false); //layer name vs layer; l is layer here
                    MapServer.TurnLayer(l.Component.Name, false);
                }
            }

            //also add the root layer name to the layers list
            //as the map (root layer) can also be requested
            mapLayers.Add(ServiceDescription.Title);


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


            //get the map scale
            double scale = CheckScale(bbox.Width / width); //bbox.Width / width is just the current pixel size 

            //layers that are to be turned on
            var visibleLayers = new List<string>();

            //now turn on the requested layers
            foreach (var l in inLayers)
            {
                if (!mapLayers.Exists(s => s == l))
                {
                    throw new WmsDriverException("Unknown layer '" + l + "'", WmsExceptionCode.LayerNotDefined);
                }
                else
                {
                    //if all layers were requested (*), this is a map (combine layers) or a root layer
                    if (l == "*" || l == map.Name || l == ServiceDescription.Title)
                    {
                        //this is a map / root layer
                        //so ojust need to turn on all the layers
                        foreach (M.Layer ll in map.LayerSet)
                        {
                            if (LayerVisible(ll.Component.Name, scale))
                            {
                                //this.mapServer.TurnLayer(ll, true); //ll is layer here
                                MapServer.TurnLayer(ll.Component.Name, true);
                                visibleLayers.Add(ll.Component.Name);
                            }
                        }
                        break; //all the layers are now on anyways
                    }
                    else //normal layer
                    {
                        if (LayerVisible(l, scale))
                        {
                            MapServer.TurnLayer(l, true); //l is string here
                            visibleLayers.Add(l);
                        }
                    }
                }
            }


            //TODO Layer reorder according to query


            if (MSettings.AutoAoi)
            {
                //note:
                //locking could occur prior to adjusting the layer visibility; to be adjusted if causes problems
                lock (_aoiRefreshLocker)
                {
                    //perform AOI updates for the linked components
                    AutoAOI(visibleLayers, bbox, width, height);


                    output.ResponseContentType = imageEncoder.MimeType;
                    output.ResponseBinary = Render(bbox, width, height, imageEncoder, backColor);
                }
            }
            else
            {
                //no aoi refreshing so just render

                output.ResponseContentType = imageEncoder.MimeType;
                output.ResponseBinary = Render(bbox, width, height, imageEncoder, backColor);
            }

            return output;
        }
    }
}
