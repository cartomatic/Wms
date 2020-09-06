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
        /// Generates a layers section of the Capabilities document
        /// </summary>
        /// <param name="capsDoc"></param>
        /// <returns></returns>
        protected override WMS_Capabilities GenerateCapsLayersSection130(WMS_Capabilities capsDoc)
        {
            var rootL = new Layer();

            //Root layer name
            rootL.Name = ServiceDescription.Title;

            //Root layer crs
            var rootCrs = new List<string>();
            rootCrs.Add("EPSG:" + MSettings.SRID);
            rootL.CRS = rootCrs.ToArray();


            //get the map component - map component of the mapserver object; only maps are allowed!
            M.Map map = MapServer.Component as M.Map;


            //Get the map's bounding box
            var mapbbox = GetMapBbox(map);

            //convert it to geographic and add to output
            rootL.EX_GeographicBoundingBox = ConvertToGeographicBoundingBox(mapbbox, map.CoordinateSystem);


            //bounding box for crs
            var bboxes = new List<BoundingBox>();
            bboxes.Add(ConvertToBoundingBox(mapbbox, MSettings.SRID));
            rootL.BoundingBox = bboxes.ToArray();


            //extract layers layers
            var layers = new List<Layer>();


            //check if the layers should be combined
            if (MSettings.CombineLayers)
            {
                layers.Add(GenerateWmsLayerDescription(map));
            }
            else
            {
                foreach (M.Layer l in map.LayerSet)
                {
                    layers.Add(GenerateWmsLayerDescription(l, map));
                }
            }

            //add layers to root layer
            rootL.Layer1 = layers.ToArray();

            //and pass it back to the caps doc
            capsDoc.Capability.Layer = rootL;

            return capsDoc;
        }

        /// <summary>
        /// generates a WmsLayer description for a map; used when combineLayers setting is set to true
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected Layer GenerateWmsLayerDescription(M.Map map)
        {
            Layer L = new Layer();

            L.Name = map.Name;
            L.Title = map.Name;

            //l (map) bbox
            var lbbox = GetMapBbox(map);
            L.BoundingBox = new BoundingBox[] { ConvertToBoundingBox(lbbox, MSettings.SRID) };
            L.EX_GeographicBoundingBox = ConvertToGeographicBoundingBox(lbbox, map.CoordinateSystem);

            //layer not queryable and stuff


            return L;
        }


        /// <summary>
        /// Generates a WmsLayer description for manifold map layer
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        protected Layer GenerateWmsLayerDescription(M.Layer l, M.Map map)
        {
            Layer L = new Layer();


            L.Name = l.Component.Name;
            L.Title = l.Component.Name;

            //No styles for manifold layers so far

            //layer bounding box
            var lbbox = GetLayerBoundingBox(l);
            L.BoundingBox = new BoundingBox[] { ConvertToBoundingBox(lbbox, MSettings.SRID) };
            L.EX_GeographicBoundingBox = ConvertToGeographicBoundingBox(lbbox, map.CoordinateSystem);


            //queryable
            L.queryable = GetQueryable(l);


            return L;
        }
    }
}
