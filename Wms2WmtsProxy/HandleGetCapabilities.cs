using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wmts_101 = Cartomatic.OgcSchemas.Wmts.Wmts_101;
using Wms_1302 = Cartomatic.OgcSchemas.Wms.Wms_1302;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Adds some customisations to the generated wms caps doc, so the clients can then call the service through this very proxy
        /// </summary>
        /// <returns></returns>
        protected override Wms_1302.WMS_Capabilities GenerateWmsCapabilitiesDocument130()
        {
            var capsDoc = base.GenerateWmsCapabilitiesDocument130();

            //this could be potentially taken off the initial request, but is simply reassembled here
            var urlWithProxy = $"{ProxyUrl}?{ProxyUrlParam}={GetBaseUrl()}";

            capsDoc.Service.OnlineResource.href = urlWithProxy;

            //Note:
            //only mandatory ops for the time being

            foreach (var dcpType in capsDoc.Capability.Request.GetCapabilities.DCPType)
            {
                ModifyDcpTypeUrl(dcpType, urlWithProxy);
            }
            foreach (var dcpType in capsDoc.Capability.Request.GetMap.DCPType)
            {
                ModifyDcpTypeUrl(dcpType, urlWithProxy);
            }

            return capsDoc;
        }

        /// <summary>
        /// Replaces the passed dcpType urls with the provided one
        /// </summary>
        /// <param name="dcpType"></param>
        /// <param name="url"></param>
        protected void ModifyDcpTypeUrl(Wms_1302.DCPType dcpType, string url)
        {
            if (dcpType?.HTTP?.Get != null)
            {
                dcpType.HTTP.Get.OnlineResource.href = url;
            }
            if (dcpType?.HTTP?.Post != null)
            {
                dcpType.HTTP.Post.OnlineResource.href = url;
            }
        }

        //get map url - this is important as need to route the wms requests through this very proxy
        //bounding boxes and alike
        //allowed epsg

        //GetMap formats - where resource url resource type == tile
        //same place also gives a request url - per format of course; url has some replacement tokens;  - this can be perhaps extracted by some utils - get by format- some nice extension methods!

        //need to extract available formats of get map
        //ignore get feature info for the time being

        //tile matrix set - this can be perhaps extracted by some utils - get by epsg - some nice extension methods!

        protected override Wms_1302.WMS_Capabilities GenerateCapsLayersSection130(Wms_1302.WMS_Capabilities capsDoc)
        {
            var baseUrl = GetBaseUrl();

            var wmtsCaps = GetCachedWmtsCaps(baseUrl);

            var rootL = new Wms_1302.Layer();

            rootL.Name = wmtsCaps.Contents.LayerSet.Aggregate(string.Empty,
                (agg, ls) => ls.Title.FirstOrDefault()?.Value);


            //this is for manifold8 - it will always default to wgs84 or whatever is first on the list i think
            //so in order to make the service be flexible usable with manifold, users can specify the crs they want to use
            var enforcedCrs = GetParam<string>("enforcecrs");


            //extract bboxes as defined int wmts
            var wmtsBboxes =
                wmtsCaps.Contents.LayerSet.Aggregate(new List<Wmts_101.BoundingBoxType>(),
                    (agg, ls) =>
                    {
                        agg.AddRange(ls.Items);
                        return agg;
                    })
                    .Where(bb => string.IsNullOrEmpty(enforcedCrs) || "EPSG:" + bb.crs.Split(':').Last() == enforcedCrs);

            
            var wmtsWgs84Bboxes = wmtsCaps.Contents.LayerSet.Aggregate(new List<Wmts_101.WGS84BoundingBoxType>(),
                (agg, ls) =>
                {
                    agg.AddRange(ls.WGS84BoundingBox);
                    return agg;
                });

            //list of supported CRSs
            var rootCrs = new List<string>();
            rootCrs.AddRange(
                wmtsBboxes.Select(bb => "EPSG:" + bb.crs.Split(':').Last()).Distinct()
            );
            rootL.CRS = rootCrs.ToArray();


            //crs bboxes
            rootL.BoundingBox = PrepareWmsBoundingBoxes(wmtsBboxes).ToArray();


            //geographic bbox
            //Note: wmts can specify an array of wgs84 bboxes. guess this is when each layer define a different extent
            //So basically need to make one bbox out of an arr
            rootL.EX_GeographicBoundingBox = PrepareEx_GeographicBoundingBox(wmtsWgs84Bboxes);


            //and finally do the layers
            var layers = new List<Wms_1302.Layer>();
            foreach (var wmtsL in wmtsCaps.Contents.LayerSet)
            {
                var l = new Wms_1302.Layer();

                //for the time being make the layers non-queryable - need to review the wmts specs and the way layers are marked as queryable
                //as well as implement the GetFeatureInfo op
                l.queryable = false;

                //Note it should really be aggregated, or be passesd lang specific. the wms specs though does not seem to bother with different langs,
                //so for now, just using the first one
                l.Name = wmtsL.Title.FirstOrDefault()?.Value;
                l.Title = wmtsL.Title.FirstOrDefault()?.Value;
                l.Abstract = wmtsL.Abstract.FirstOrDefault()?.Value;

                l.BoundingBox = PrepareWmsBoundingBoxes(wmtsL.Items).ToArray();

                l.EX_GeographicBoundingBox = PrepareEx_GeographicBoundingBox(wmtsL.WGS84BoundingBox);

                layers.Add(l);
            }

            rootL.Layer1 = layers.ToArray();

            //and pass it back to the caps doc
            capsDoc.Capability.Layer = rootL;

            return capsDoc;
        }

        /// <summary>
        /// Makes a wms EX_GeographicBoundingBox out of an ienumerable of wmts WGS84BoundingBoxType
        /// </summary>
        /// <param name="wGS84Bboxes"></param>
        /// <returns></returns>
        protected internal Wms_1302.EX_GeographicBoundingBox PrepareEx_GeographicBoundingBox(IEnumerable<Wmts_101.WGS84BoundingBoxType> wGS84Bboxes)
        {
            var outBb = new Wms_1302.EX_GeographicBoundingBox()
            {
                eastBoundLongitude = Double.MinValue,
                northBoundLatitude = Double.MinValue,
                westBoundLongitude = Double.MaxValue,
                southBoundLatitude = Double.MaxValue
            };

            foreach (var bbox in wGS84Bboxes)
            {
                var bottomLeft = bbox.LowerCorner.Split(' ');
                var topRight = bbox.UpperCorner.Split(' ');

                //Note min/max may give some weird errs if the longitudes span 180...
                //FIXME - make boxes that span 180 work as expected!

                outBb.eastBoundLongitude = Math.Max(outBb.eastBoundLongitude, double.Parse(topRight[0], CultureInfo.InvariantCulture));
                outBb.northBoundLatitude = Math.Max(outBb.northBoundLatitude, double.Parse(topRight[1], CultureInfo.InvariantCulture));
                outBb.westBoundLongitude = Math.Min(outBb.westBoundLongitude, double.Parse(bottomLeft[0], CultureInfo.InvariantCulture));
                outBb.southBoundLatitude = Math.Min(outBb.southBoundLatitude, double.Parse(bottomLeft[1], CultureInfo.InvariantCulture));
            }

            return outBb;
        }

        /// <summary>
        /// Processes a list of wmts bboxxes into wms bboxes
        /// </summary>
        /// <param name="wmtsBboxes"></param>
        /// <returns></returns>
        protected internal List<Wms_1302.BoundingBox> PrepareWmsBoundingBoxes(IEnumerable<Wmts_101.BoundingBoxType> wmtsBboxes)
        {
            var output = new List<Wms_1302.BoundingBox>();

            foreach (var wmtsBbox in wmtsBboxes)
            {
                //check if a list already contains a bbox for given crs
                var crs = "EPSG:" + wmtsBbox.crs.Split(':').Last();

                var bbox = output.Where(bb => bb.CRS == crs).FirstOrDefault();
                if (bbox == null)
                {
                    bbox = new Wms_1302.BoundingBox()
                    {
                        CRS = crs,
                        maxx = double.MinValue,
                        maxy = double.MinValue,
                        minx = double.MaxValue,
                        miny = double.MaxValue
                    };
                    output.Add(bbox);
                }

                //got the bbox, so can extend it as needed
                var bottomLeft = wmtsBbox.LowerCorner.Split(' ');
                var topRight = wmtsBbox.UpperCorner.Split(' ');

                bbox.miny = Math.Min(bbox.miny, double.Parse(bottomLeft[1], CultureInfo.InvariantCulture));
                bbox.minx = Math.Min(bbox.minx, double.Parse(bottomLeft[0], CultureInfo.InvariantCulture));
                bbox.maxy = Math.Max(bbox.maxy, double.Parse(topRight[1], CultureInfo.InvariantCulture));
                bbox.maxx = Math.Max(bbox.maxx, double.Parse(topRight[0], CultureInfo.InvariantCulture));
            }

            return output;
        }  
    }
}
