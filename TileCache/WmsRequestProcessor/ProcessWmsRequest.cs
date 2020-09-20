using System.Net;
using System.Threading.Tasks;
using Cartomatic.Utils;
using Cartomatic.Utils.Web;
using Cartomatic.Wms.WmsDriverExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cartomatic.Wms.TileCache
{
    public partial class WmsRequestProcessor
    {
        /// <summary>
        /// Processes the request that is supposed to be a wms request; All invalid requests - non getmap requests are just proxied
        /// If a request is a getmap request more checkups are done - for example if requested image 'fits' into a tile scheme used for caching;
        /// 
        /// This method performs the actual web request against the provided url. This makes it possible to cache both local or remote services
        /// </summary>
        /// <param name="settings">Tile cache settings object TileCache.Settings</param>
        /// <param name="ts">Tile Scheme used for caching given reques</param>
        /// <param name="url">URL of the request</param>
        public static async Task<Output> ProcessRequestAsync(Settings settings, TileScheme ts, string url)
        {
            //Note: need to pass null casted to base wms driver type so the param is accepted;
            //otherwise the method could not work out the type!
            return await ProcessRequestAsync<WmsDriver>(settings, ts, url, null);
        }


        /// <summary>
        /// Processes the request that is supposed to be a wms request; All invalid requests - non getmap requests are just proxied
        /// If a request is a getmap request more checkups are done - for example if requested image 'fits' into a tile scheme used for caching;
        /// 
        /// If wms driver is not provided, the actual web request is performed; This makes it possible to cache output of wms drivers as well as
        /// local and remote wms services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings">Tile cache settings object TileCache.Settings</param>
        /// <param name="ts">Tile Scheme used for caching given request</param>
        /// <param name="url">The actual request URL</param>
        /// <param name="wmsDrv">WmsDriver if the request should be handled by a driver</param>
        public static async Task<Output> ProcessRequestAsync<T>(Settings settings, TileScheme ts, string url, T wmsDrv) where T : WmsDriver
        {
            var output = new Output();


            //System.Net.HttpWebRequest object for easy request params retrieval
            var request = url.CreateHttpWebRequest();

            if (request == null)
            {
                return output;
            }

            //if there is not tileset provided, just pass through as it will not be able to neither lookup for the cache nor save file
            if (ts == null)
            {
                return await HandleWmsRequestAsync(request, wmsDrv);
            }

            var requestParams = System.Web.HttpUtility.ParseQueryString(request.Address.Query);

            //getmap request?
            var wmsRequest = requestParams["request"];
            if (string.IsNullOrEmpty(wmsRequest) || wmsRequest.ToLower() != "getmap")
            {
                return await HandleWmsRequestAsync(request, wmsDrv);
            }



            //format compatible?
            var requestedFormatMime = requestParams["format"].ToLower();
            if (!Utils.WmsImageFormatSupported(requestedFormatMime, settings))
            {
                return await HandleWmsRequestAsync(request, wmsDrv);
            }


            //tile size compatible?
            int.TryParse(requestParams["height"], out var h);
            int.TryParse(requestParams["width"], out var w);

            if (w != ts.TileSize || h != ts.TileSize)
            {
                output = await HandleWmsRequestAsync(request, wmsDrv);

                //dev opts
                output.ResponseBinary = Utils.ApplyDevOptions(
                    settings: settings,
                    bmpData: output.ResponseBinary,
                    outMime: requestedFormatMime,
                    failureMsg: "invalid tile size"
                );

                return output;
            }

            //layers present?
            if (string.IsNullOrEmpty(requestParams["layers"]))
            {
                return await HandleWmsRequestAsync(request, wmsDrv);
            }


            //get ready with tileset def
            ts.Prepare();

            if (!ts.Ready)
            {
                output = await HandleWmsRequestAsync(request, wmsDrv);

                //dev opts
                output.ResponseBinary = Utils.ApplyDevOptions(
                settings: settings,
                bmpData: output.ResponseBinary,
                outMime: requestedFormatMime,
                failureMsg: "invalid tset def"
            );

                return output;
            }


            //calculate the tile address and check if it fits into the tileset
            var crs = requestParams["crs"];
            if (crs == null)
            {
                crs = requestParams["srs"];
            }

            //bbox is dependent on the wms version & epsg (starting with 1.3.0, bbox axis order depends on the projection definition)
            var bbox = WmsDriver.ParseBBOX(requestParams["bbox"], requestParams["version"], crs);

            var t = ts.CalculateTileAddress(new Bounds(bbox.MinX, bbox.MinY, bbox.MaxX, bbox.MaxY));

            if (!t.Valid)
            {
                output = await HandleWmsRequestAsync(request, wmsDrv);

                //dev opts
                output.ResponseBinary = Utils.ApplyDevOptions(
                    settings: settings,
                    bmpData: output.ResponseBinary,
                    outMime: requestedFormatMime,
                    tile: t
                );

                return output;
            }

            //At this stage the tile is valid - it 'belongs' to the tile scheme and its address is known
            //so need to check if it has already been cached

            //depending on whether the dev mode is on or not cached file is retrieved as either only a path or file binary data
            //the default behavior is to return ony a file path so reading the file to memory can be avoided
            if (settings.DevModeStampTileAddress)
            {
                //dev mode - file data retrieved as binary so can quickly stamp some data on it

                var cachedData = Utils.GetCachedFile(settings.CacheFolder, requestParams, t);
                if (cachedData.Length > 0)
                {
                    output.HasData = true;
                    output.ResponseContentType = requestedFormatMime;

                    //dev opts
                    output.ResponseBinary = Utils.ApplyDevOptions(
                        settings: settings,
                        bmpData: cachedData,
                        outMime: requestedFormatMime,
                        tile: t,
                        cached: true
                    );

                    return output;
                }
            }
            else
            {
                //this is the default mode - only file path is returned if the file exists

                var fPath = Utils.GetCachedFilePath(settings.CacheFolder, requestParams, t);


                if (System.IO.File.Exists(fPath))
                {
                    output.HasFile = true;
                    output.FilePath = fPath;
                    output.ResponseContentType = requestedFormatMime;

                    return output;
                }
            }



            //Since we are here it means no cache was found so need to execute a web request or make the wms driver do the job
            output = await HandleWmsRequestAsync(request, wmsDrv);


            //check the content type of the format returned by the server or wms driver
            //if there were any exceptions, response content type will be different than the content requested by the client
            //In such case avoid saving the cache and just return the output
            if (requestedFormatMime != output.ResponseContentType)
            {
                return output;
            }


            //Note:
            //TODO
            //Here we could do some bbox checks if needed - there is no reason to save cache for areas that should not be cached;
            //So far limited bbox params are not available in both - cache settings and tile scheme, so this will have to be
            //rethought at some stage


            //TODO:
            //Need to think of something for the scenarios where the response returns inimage or image exception
            //it would be not good to cache such stuff...
            //The problem is that in such cases it may be difficult to know if the service returned wrong data
            //perhaps response status code would give some info

            //save file on independent thread so we can return immeadiately as saving file is delegated
#if NETFULL
            var sf = new Utils.DelegateSaveFileBasedOnRequestParams(Utils.SaveFile);
            sf.BeginInvoke(
                output.ResponseBinary,
                settings.CacheFolder,
                requestParams,
                t,
                null,
                null
            );
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_1
            Task.Run(() => Utils.SaveFile(output.ResponseBinary, settings.CacheFolder, requestParams, t));
#endif


            //dev opts
            output.ResponseBinary = Utils.ApplyDevOptions(
                settings: settings,
                bmpData: output.ResponseBinary,
                outMime: requestedFormatMime,
                tile: t
            );

            return output;
        }

        /// <summary>
        /// Handles the Wms request using a wms driver or by performing a standard web request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="wmsDrv"></param>
        private static async Task <Output> HandleWmsRequestAsync<T>(System.Net.HttpWebRequest request, T wmsDrv) where T : WmsDriver
        {
            return wmsDrv != null
                ? await HandleWmsDriverRequestAsync(request, wmsDrv)
                : await HandleStandardWebRequestAsync(request);
        }

        /// <summary>
        /// Makes a wmsdrv process the wms request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="wmsDrv"></param>
        private static async Task<Output> HandleWmsDriverRequestAsync<T>(System.Net.HttpWebRequest request, T wmsDrv) where T : WmsDriver
        {
            var output = new Output();

            var wmsDrvOut = await wmsDrv.HandleRequestAsync(request);

            output.ResponseContentType = wmsDrvOut.ResponseContentType;
            output.ResponseBinary = wmsDrvOut.ResponseBinary;
            output.ResponseText = wmsDrvOut.ResponseText;
            output.StatusCode = wmsDrvOut.WmsDriverException == null
                ? HttpStatusCode.OK
                : wmsDrvOut.WmsDriverException.WmsExceptionCode == WmsExceptionCode.NotApplicable
                    ? HttpStatusCode.InternalServerError
                    : HttpStatusCode.BadRequest;
            output.HasData = wmsDrvOut.HasData();

            return output;
        }

        /// <summary>
        /// Handles a standard web request
        /// </summary>
        /// <param name="request"></param>
        private static async Task<Output> HandleStandardWebRequestAsync(System.Net.HttpWebRequest request)
        {
            var output = new Output();

            var response = await request.ExecuteRequestAsync();

            output.ResponseBinary = response.GetResponseStream().ReadStream();

            //response could have returned exception, therefore the content type may be different than the requested one
            output.ResponseContentType = response.ContentType;

            response.Close();

            output.HasData = true;

            return output;
        }
    }
}
