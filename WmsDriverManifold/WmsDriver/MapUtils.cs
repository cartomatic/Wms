using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    /// <summary>
    /// Extracts map layer names off the map component.
    /// </summary>
    public partial class WmsDriver
    {
        /// <summary>
        /// Extracts names of the layers currently available in the served map
        /// </summary>
        /// <returns></returns>
        private List<string> ExtractMapLayers()
        {
            var mapLayers = new List<string>();

            var map = MapServer.Component as M.Map;

            if (MSettings.CombineLayers)
            {
                mapLayers.Add(map.Name);
            }
            else
            {
                mapLayers.AddRange(from M.Layer l in map.LayerSet select l.Component.Name);
            }

            return mapLayers;
        }

        /// <summary>
        /// Manages map layers visibility according to the value of the layers param
        /// </summary>
        private void ManageMapLayersVisibility(double mapResolution)
        {
            var map = MapServer.Component as M.Map;
            
            var inLayers = GetParam("layers").Split(',');

            //get the map scale
            double scale = CheckScale(mapResolution); //bbox.Width / width is just the current pixel size 

            foreach (var l in inLayers)
            {
                //if all layers were requested (*), this is a map (combine layers) or a root layer
                if (l == "*" || l == map.Name || l == ServiceDescription.Title)
                {
                    //this is a map / root layer
                    //so ojust need to turn on all the layers
                    foreach (M.Layer ll in map.LayerSet)
                    {
                        MapServer.TurnLayer(ll.Component.Name, LayerVisible(ll.Component.Name, scale));
                    }
                    break; //all the layers are now on anyways
                }
                else //normal layer
                {
                    MapServer.TurnLayer(l, LayerVisible(l, scale));
                }
            }
        }


        /// <summary>
        /// checks if a layer is visible in scale; method taken without changes from the first version - should be reviewed at some point
        /// </summary>
        /// <param name="compName"></param>
        /// <returns></returns>
        private bool LayerVisible(string compName, double? scale = null)
        {

            bool visible = false;

            //component zoom
            double compZoomMax = 0;
            double compZoomMin = 0;

            //current scale; 
            if (!scale.HasValue)
            {
                //if not provided, grab it directly off map server; if mapserver has not yet been properly set, then the readout is gonna be baaad ;)
                scale = CheckScale();
            }

            M.ComponentType compType = MapServer.Document.ComponentSet[compName].Type;

            switch (compType)
            {
                case M.ComponentType.ComponentDrawing:

                    M.Drawing drawing = (M.Drawing)MapServer.Document.ComponentSet[compName];
                    compZoomMax = drawing.ZoomMax;
                    compZoomMin = drawing.ZoomMin;

                    break;

                case M.ComponentType.ComponentImage:

                    M.Image image = (M.Image)MapServer.Document.ComponentSet[compName];
                    compZoomMax = image.ZoomMax;
                    compZoomMin = image.ZoomMin;

                    break;

                case M.ComponentType.ComponentSurface:

                    M.Surface surface = (M.Surface)MapServer.Document.ComponentSet[compName];
                    compZoomMax = surface.ZoomMax;
                    compZoomMin = surface.ZoomMin;

                    break;

                case M.ComponentType.ComponentLabels:

                    M.Labels labels = (M.Labels)MapServer.Document.ComponentSet[compName];
                    compZoomMax = labels.ZoomMax;
                    compZoomMin = labels.ZoomMin;

                    break;
            }

            //check whether a layer is visible in scale
            if (compZoomMax == 0 && compZoomMin == 0)
            {
                //Scale range not set so layer is visible
                visible = true;
            }
            else
            {
                //at least one scale restriction is set so test whether the layer is visible
                if (compZoomMax >= scale && compZoomMin <= scale)
                {
                    visible = true;
                }
                else
                {
                    //also test if the zoom max is not 0
                    //This ensures setting layer visible to true if the raster layer has no max zoom set
                    if (compZoomMax == 0 && compZoomMin <= scale)
                    {
                        visible = true;
                    }
                    else
                    {
                        visible = false;
                    }
                }
            }

            return visible;
        }





        /// <summary>
        /// Returns current zoom scale of served component
        /// </summary>
        /// <returns></returns>
        private double CheckScale(double? viewScaleX = null)
        {
            //grab map units
            M.Map map = (M.Map)MapServer.Component;

            //map units expressed in metres
            double mapUnit = map.CoordinateSystem.Unit.Scale;

            //grab the viewScaleX from mapServer if not provided
            if (!viewScaleX.HasValue)
            {
                viewScaleX = MapServer.ViewScaleX;
            }

            //express viewscale in metres
            double pixelSizeMapServer = (double)viewScaleX * mapUnit;

            //calculate scale when the resolution is assumed to be 96ppi -> 0.0254 = 2.54 cm in metres
            //double scale = Math.Round(pixelSizeMapServer / pixelSize);
            double scale = pixelSizeMapServer / PIXEL_SIZE;


            return scale;
        }
    }
}
