using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using M = Manifold.Interop;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        /// <summary>
        /// Converts m.Rect to EX_GeographicBoundingBox
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        protected EX_GeographicBoundingBox ConvertToGeographicBoundingBox(M.Rect rect, M.CoordinateSystem cs)
        {
            //output object
            EX_GeographicBoundingBox bbox = new EX_GeographicBoundingBox();

            //prepare the converter
            M.CoordinateConverter converter = MapServer.Application.NewCoordinateConverter();
            converter.Prepare((M.Base)cs, (M.Base)MapServer.Application.NewCoordinateSystem("Latitude / Longitude"));


            //bbox corner points
            var bl = MapServer.Application.NewPoint(rect.XMin, rect.YMin);
            var tr = MapServer.Application.NewPoint(rect.XMax, rect.YMax);

            //convert the bbox
            converter.Convert((M.Base)bl, null);
            converter.Convert((M.Base)tr, null);

            //transfer values
            bbox.westBoundLongitude = bl.X;
            bbox.southBoundLatitude = bl.Y;
            bbox.eastBoundLongitude = tr.X;
            bbox.northBoundLatitude = tr.Y;

            //and return the object
            return bbox;
        }


        /// <summary>
        /// Converts m.Rect to BoundingBox
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        protected BoundingBox ConvertToBoundingBox(M.Rect rect, int srid)
        {
            BoundingBox bb = new BoundingBox();

            bb.CRS = "EPSG:" + srid;

            bb.minx = rect.XMin;
            bb.miny = rect.YMin;
            bb.maxx = rect.XMax;
            bb.maxy = rect.YMax;

            return bb;
        }



        /// <summary>
        /// Gets map's bounding box
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected M.Rect GetMapBbox(M.Map map)
        {


            //get the map's bounding box
            /*
             * Note:
             * When there are only linked images on the map manifold is not able to get the bbox properly
             * if there is at least one vector layer though it seems to get it right
             * 
             * Therefore the linked images in the map have to be checked and the bounding
             * box calculated 'manually'
             */


            //now try to grab a bbox off the map - if 
            M.Rect projectedBbox;
            try
            {
                //this will throw an error if there are only linked images on the map
                projectedBbox = map.Box;
            }
            catch
            {

                //Looks like there are only linked images

                //Note:
                //This actually may also apply to linked but not refreshed vector comps....


                //bbox extent
                projectedBbox = MapServer.Application.NewRect();

                //a flag indicating whether the bbox parts have already been read from the component
                bool hasValue = false;

                //first check if there are linked images in the map
                foreach (M.Layer lay in map.LayerSet)
                {

                    //get the component of the layer
                    M.Component cmp = lay.Component;

                    var lbbox = GetLayerBoundingBox(lay);

                    if (!hasValue)
                    {
                        projectedBbox.XMin = lbbox.XMin;
                        projectedBbox.YMin = lbbox.YMin;
                        projectedBbox.XMax = lbbox.XMax;
                        projectedBbox.YMax = lbbox.YMax;

                        hasValue = true;
                    }
                    else //not the first layer so expand bbox
                    {
                        projectedBbox.XMin = Math.Min(projectedBbox.XMin, lbbox.XMin);
                        projectedBbox.YMin = Math.Min(projectedBbox.YMin, lbbox.YMin);
                        projectedBbox.XMax = Math.Max(projectedBbox.XMax, lbbox.XMax);
                        projectedBbox.YMax = Math.Max(projectedBbox.YMax, lbbox.YMax);
                    }
                }

            }

            return projectedBbox;
        }

        /// <summary>
        /// Gets a bounding box of a layer
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        protected M.Rect GetLayerBoundingBox(M.Layer l)
        {
            return GetComponentBoundingBox(l.Component);
        }

        /// <summary>
        /// Gets a bounding box of a component
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected M.Rect GetComponentBoundingBox(M.Component c)
        {
            M.Rect bbox = null;

            switch (c.Type)
            {
                case M.ComponentType.ComponentImage:

                    var img = (M.Image)c;

                    if (img.IsLinked())
                    {
                        //grab the image coordsys
                        M.CoordinateSystem imgCoordSys = img.CoordinateSystem;

                        var xMin = imgCoordSys.ParameterSet["localOffsetX"].Value;
                        var yMin = imgCoordSys.ParameterSet["localOffsetY"].Value;
                        var xMax = xMin + img.Width * imgCoordSys.ParameterSet["localScaleX"].Value;
                        var yMax = yMin + img.Height * imgCoordSys.ParameterSet["localScaleY"].Value;

                        bbox = MapServer.Application.NewRect(xMin, yMin, xMax, yMax);

                    }
                    else
                    {
                        bbox = img.Box;
                    }

                    break;

                case M.ComponentType.ComponentDrawing:
                    bbox = ((M.Drawing)c).Box;
                    break;

                case M.ComponentType.ComponentLabels:
                    bbox = ((M.Labels)c).Box;
                    break;

                case M.ComponentType.ComponentSurface:
                    bbox = ((M.Surface)c).Box;
                    break;

                case M.ComponentType.ComponentTheme:
                    bbox = (((M.Theme)c).OwnerDrawing as M.Drawing).Box;
                    break;

                default:
                    break;
            }

            return bbox;

        }
    }
}
