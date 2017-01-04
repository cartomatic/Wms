using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wmts.Wmts_101;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        protected override IWmsDriverResponse HandleGetMapDriverSpecific()
        {
            Validate(HandleGetMapValidationRulesDriverSpecific);

            return RenderTile();
        }

        /// <summary>
        /// Extracts all the stuff needed for pulling the data from the wmts, pulls the data, stiches tiles together and extracts the part requested by the wmst 
        /// </summary>
        /// <returns></returns>
        protected internal IWmsDriverResponse RenderTile()
        {
            IWmsDriverResponse output = new WmsDriverResponse();


            //tile back color
            var backColor = Color.White;
            if (GetParam<bool>("transparent"))
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


            //wmts caps cache
            var wmtsCache = GetCachedWmtsCaps(GetBaseUrl());

            //extract layer - layer param validated before, so can go straight to the point
            var layer = GetParam<string>("layers"); //this should be a single layer
            var wmtsLayer = wmtsCache.Contents.LayerSet.FirstOrDefault(l => l.Title.FirstOrDefault()?.Value == layer);

            //extract style
            //Note: currently not allowing any styles passed by the cllients, so need to extract a default one
            var style = (wmtsLayer.Style.FirstOrDefault(s => s.isDefault) ?? wmtsLayer.Style.FirstOrDefault()).Identifier.Value;

            //extract resource url - replacement tokens are: {style}, {TileMatrixSet}, {TileMatrix}, {TileCol}, {TileRow}
            var imageFormat = GetParam<string>("format");
            var resourceUrl =
                wmtsLayer.ResourceURL.FirstOrDefault(
                    ru =>
                        ru.resourceType == URLTemplateTypeResourceType.tile &&
                        string.Compare(ru.format, imageFormat, false, CultureInfo.InvariantCulture) == 0).template;


            //extract epsg and the tilematrixset for given layer and epsg - epsg validated before so can go straight to the point
            var epsg = GetParam<string>("crs");
            var tileMatrixSetLink =
                wmtsLayer.TileMatrixSetLink.FirstOrDefault(tml => string.Compare(tml.TileMatrixSet, epsg, true,CultureInfo.InvariantCulture) == 0).TileMatrixSet;
            var tileMatrixSet =
                wmtsCache.Contents.TileMatrixSet.FirstOrDefault(tms => tms.Identifier.Value == tileMatrixSetLink);


            //extract width / height
            var width = GetParam<int>("width");
            var height = GetParam<int>("height");

            //and bbox

            //Note:
            //by default manifold 8 does not reverse coords when calling wms 1.3.0.
            //some other clients do
            //this param is basically used to force flip coords of bbox for the clients that do have problems with valid wms 130 bbox requests
            //make miny,minx,maxy,maxx from minx,miny,maxx,maxy

            //see, if an extra param has been passed in to reverse the bbox coord pairs
            var rawBbox = GetParam("bbox");
            var version = GetParam("version");
            if (version == "1.3.0" && FlipBbox)
            {
                var bboxparts = rawBbox.Split(',');
                //make it miny,minx,maxy,maxx
                rawBbox = $"{bboxparts[1]},{bboxparts[0]},{bboxparts[3]},{bboxparts[2]}";
            }

            //get srid - will need it later to check ccords flip too
            var srid = Int32.Parse(epsg.Substring(epsg.LastIndexOf(':') + 1));

            //get the bbox
            var bbox = ParseBBOX(rawBbox, version, srid);


            //calculate request resolution in order to find the most suitable tile matrix for the request - this is based on scale comparison
            //assume the horizontal and vertical resolutions are the same
            //var wmsRequestRes = bbox.Width/width;


            //Note: assuming unit to be 1m. This is really naive approach though, as there are obviously projections that use different units..
            //for the time being though it will do, at some point will have to (if there is a need of course... ;) plug in some ref epsg db and calculate unit ratios instead of
            //hardcoding them
            //TODO - potentially can just accept a double param that defines the ratio of a unit to cm; this way it woud be pluggable and customisable
            //TODO - calculate proper scales / res based ond CRS unit, instead of defaulting to metre

            //Note: WMTS spec assumes a pixel size of 0.28mm
            var pixelsPerCm = 10/0.28;
            var unitToCmRatio = 100; //hardcoding the m here as a unit - see comments above.
            var pixelsPerUnit = pixelsPerCm*unitToCmRatio;

            //what is the size of bbox in scale - expressed in map units
            var requestedTileSizeInMapUnits = width/pixelsPerUnit;

            //requestBboxWidth / scale = requestedTileSizeInMapUnits ==> scale = requestBboxWidth / requestedTileSizeInMapUnits
            var requestScale = bbox.Width/requestedTileSizeInMapUnits;


            //since scale is now known need to find a tilematrix with the most appropriate scale - with the smallest diff between scales
            var minDiff = double.MaxValue;
            int minIdx = 0;
            for (var i = 0; i < tileMatrixSet.TileMatrix.Length; i++)
            {
                var scaleDiff = Math.Abs(tileMatrixSet.TileMatrix[i].ScaleDenominator - requestScale);
                if (scaleDiff < minDiff)
                {
                    minDiff = scaleDiff;
                    minIdx = i;
                }
            }

            //grab the tile matrix that best suits the request
            var tm = tileMatrixSet.TileMatrix[minIdx];

            //work out the tm pixel res
            //1/scale = 1unitScaled -> 1unitScaled * pixelsPerUnit = size in pixels -> 1/size in pixels = resolution -> resolution = scale/pixelsPerUnit
            var tmPixelResolution = tm.ScaleDenominator / pixelsPerUnit;

            //tile matrix top left corner
            var tlParts = tm.TopLeftCorner.Split(' ');

            double tmTop, tmLeft;
            //check if the srid flips coords before putting together a bbox!
            var flips = GetCoordFlip(srid);
            if (flips)
            {
                tmLeft = double.Parse(tlParts[1]);
                tmTop = double.Parse(tlParts[0]);
            }
            else
            {
                tmLeft = double.Parse(tlParts[0]);
                tmTop = double.Parse(tlParts[1]);
            }
            

            //tilematrix tilesizes in pixels
            var tmTileWidth = int.Parse(tm.TileWidth);
            var tmTileHeight = int.Parse(tm.TileHeight);
            
            //tilematrix width / height in tiles
            var tmMatrixWidth = int.Parse(tm.MatrixWidth);
            var tmMatrixHeight = int.Parse(tm.MatrixHeight);

            //now can work out which tiles to pull
            var leftTempTileIdx = (int) ((bbox.MinX - tmLeft)/tmPixelResolution/tmTileWidth);
            var rightTempTileIdx = (int) ((bbox.MaxX - tmLeft)/tmPixelResolution/tmTileWidth);
            //row idx grows from top to botom!
            var topTempTileIdx = (int) ((tmTop - bbox.MaxY)/tmPixelResolution/tmTileHeight);
            var bottomTempTileIdx = (int) ((tmTop - bbox.MinY)/tmPixelResolution/tmTileHeight);



            //how many tiles need to pull
            var localTilesetWidth = Math.Abs(rightTempTileIdx - leftTempTileIdx) + 1;
            var localTilesetHeight = Math.Abs(bottomTempTileIdx - topTempTileIdx) + 1;


            //create a bitmap that will be used to stitch the wmts tiles
            var tempBmp = new Bitmap(localTilesetWidth * tmTileWidth, localTilesetHeight * tmTileHeight, PixelFormat.Format32bppArgb);

            int tempXPixShift = 0;
            int tempYPixShift = 0;

            var tasks = new List<Task<dynamic>>();

            //prepare calls for the tiles
            for (int x = 0; x < localTilesetWidth; x++)
            {
                var colIdx = leftTempTileIdx + x;
                if (colIdx < 0 || colIdx > tmMatrixWidth) continue;

                tempXPixShift = x * tmTileWidth;
                var localTempXPixShift = tempXPixShift; //local copy so works in closure
                for (int y = 0; y < localTilesetHeight; y++)
                {
                    var rowIdx = topTempTileIdx + y;
                    if (rowIdx < 0 || rowIdx > tmMatrixHeight) continue;

                    tempYPixShift = y * tmTileHeight;

                    var localTempYPixShift = tempYPixShift; //local copy so works in closure

                    //work out the tile url
                    //If present in the ResourceURL - its a restful 
                    //template="https://tiles-a.data-cdn.linz.govt.nz/services;key=3d56d9b5bb5a46fd9cef81ce077f3173/tiles/v4/set=2,{style}/{TileMatrixSet}/{TileMatrix}/{TileCol}/{TileRow}.png"/>
                    var tileUtl =
                        resourceUrl.Replace("{style}", style)
                            .Replace("{TileMatrixSet}", tileMatrixSetLink)
                            .Replace("{TileMatrix}", tm.Identifier.Value)
                            .Replace("{TileCol}", colIdx.ToString())
                            .Replace("{TileRow}", rowIdx.ToString());

                    //KVP requests
                    //https://data.linz.govt.nz/services;key=3d56d9b5bb5a46fd9cef81ce077f3173/wmts/?service=WMTS&version=1.0.0&request=GetFeatureInfo&style=auto&format=image/png&layer=set-2&tilematrixset=EPSG:2193&tilematrix=0&tilecol=1&tilerow=3&infoformat=application/json&i=1&j=1
                    //https://data.linz.govt.nz/services;key=3d56d9b5bb5a46fd9cef81ce077f3173/wmts/?service=WMTS&version=1.0.0&request=GetTile&style=auto&format=image/png&layer=set-2&tilematrixset=EPSG:2193&tilematrix=0&tilecol=1&tilerow=3&infoformat=application/json&i=1&j=1

                    Task<dynamic> f = Task<dynamic>.Factory.StartNew(() =>
                    {
                        //pull the image
                        var request = tileUtl.CreateHttpWebRequest();
                        var response = request.ExecuteRequest();

                        //TODO - potentially could do some caching too... so no need to call the wmts service so many times... need to test how this will work without caching first; also in memmory caching could be painful, on the other hand though it's easier to wipe such cache ;)

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            //read the output and make an image out of it
                            return new
                            {
                                tile = new Bitmap(response.GetResponseStream()),
                                tempXPixShift = localTempXPixShift,
                                tempYPixShift = localTempYPixShift
                            };
                        }
                        //ignore failed tiles
                        else
                        {
                            //retry...
                            request = tileUtl.CreateHttpWebRequest();
                            response = request.ExecuteRequest();
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                //read the output and make an image out of it
                                return new
                                {
                                    tile = new Bitmap(response.GetResponseStream()),
                                    tempXPixShift = localTempXPixShift,
                                    tempYPixShift = localTempYPixShift
                                };
                            }
                        }

                        return null;
                    });

                    tasks.Add(f);
                }
            }

            //all the tasks have been already fired up so need to await the
            

            //and await all
            Task.WaitAll(tasks.ToArray());

            
            using (var g = Graphics.FromImage(tempBmp))
            {
                foreach (var task in tasks)
                {
                    if (task.Result == null) continue;

                    var tile = task.Result.tile as Bitmap;

                    g.DrawImage(
                        tile,
                        new Rectangle(task.Result.tempXPixShift, task.Result.tempYPixShift, tmTileWidth, tmTileHeight),
                        new Rectangle(0, 0, tile.Width, tile.Height),
                        GraphicsUnit.Pixel
                    );
                }
            }

            

            //at this stage the whole temp tile should be present so need to extract the needed part and paint it onto an output tile.

            //basically temp tileset / temp image should be larger than the requested bbox - always as it is made sure all the corners fit in

            //work out the top left of the temp tileset in map units
            var tempTilesetMinX = tmLeft + (leftTempTileIdx)*tmTileWidth*tmPixelResolution;
            var tempTilesetMaxY = tmTop - (topTempTileIdx)*tmTileHeight*tmPixelResolution;

            //work out the shift from left and top
            var xPixShift = (bbox.MinX - tempTilesetMinX)/tmPixelResolution;
            var yPixShift = (tempTilesetMaxY - bbox.MaxY)/tmPixelResolution;

            //and the rect width and height
            var pixWidth = bbox.Width/tmPixelResolution;
            var pixHeight = bbox.Height/tmPixelResolution;

            //finally create an output bitmap and paint the cut part of the temp tileset
            var outBmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(outBmp))
            {
                //if back color different than transparent paint it
                if (backColor != Color.Transparent)
                {
                    g.FillRectangle(
                        new SolidBrush(backColor),
                        new Rectangle(0, 0, outBmp.Width, outBmp.Height)
                        );
                }

                g.DrawImage(
                    tempBmp,
                    new Rectangle(0, 0, outBmp.Width, outBmp.Height),
                    new RectangleF((float) xPixShift, (float) yPixShift, (float) pixWidth, (float) pixHeight),
                    GraphicsUnit.Pixel
                    );
            }

            //Get the image format requested
            //Note: this should have been checked in the base driver against formats supported
            ImageCodecInfo imageEncoder = GetEncoderInfo(GetParam("FORMAT"));

            //and finally write the bmp to mem str so can get bytes
            using (var ms = new MemoryStream())
            {
                outBmp.Save(ms, imageEncoder, null);
                output.ResponseContentType = imageEncoder.MimeType;
                output.ResponseBinary = ms.ToArray();
            }

            return output;
        }
    }
}
