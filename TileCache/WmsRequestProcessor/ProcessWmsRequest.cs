using Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Utils;
using Cartomatic.Utils.Web;
using Cartomatic.Wms.TileCache.DataModel;
using Cartomatic.Wms.WmsDriverExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cartomatic.Wms.TileCache
{
    public partial class WmsRequestProcessor
    {
        /// <summary>
        /// Processes the request that is supposed to be a wms request; All invalid requests - non getmap requests are just proxied
        /// If a request is a getmap request more checkups are done - for example if requested image 'fits' into a tile scheme used for caching;
        ///
        /// this method is a convenience wrapper for proxying and caching a remote WMS
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="cfg">tile cache configuration</param>
        /// <param name="endpoints">a collection of maps to look up a remote url for a map</param>
        /// <returns></returns>
        public static async Task<IActionResult> ProcessRequestAsync(
            HttpRequest request,
            HttpResponse response,
            Configuration cfg,
            Dictionary<string, string> endpoints
        )
        {
            var query = request.Query;

            var endpoint = query["endpoint"];

            if (string.IsNullOrWhiteSpace(endpoint) || !endpoints.ContainsKey(endpoint))
                return WmsException(response, $"Unrecognized endpoint: {endpoint}");


            var urlBase = endpoints[endpoint];

            //discard map param - manifold wms driver uses it
            var destUrl = $"{urlBase}?{string.Join("&", query.Select(x => $"{x.Key}={x.Value}"))}";

            var tcOut = await Cartomatic.Wms.TileCache.WmsRequestProcessor.ProcessRequestAsync(
                cfg.Settings,
                cfg.TileScheme,
                destUrl
            );

            //rewrite whatever content has been returned by the remote server
            response.ContentType = tcOut.ResponseContentType;



            if (tcOut.HasFile)
            {
                var fs = new FileStream(tcOut.FilePath, FileMode.Open);
                response.RegisterForDispose(fs);
                return new FileStreamResult(fs, tcOut.ResponseContentType);
            }

            if (tcOut.HasData)
            {
                //rewrite response if required
                if (tcOut.ResponseContentType == "application/xml" &&
                    query["request"].ToString().ToLower() == "getcapabilities")
                {
                    using var ms = new System.IO.MemoryStream(tcOut.ResponseBinary);
                    using var sw = new StreamReader(ms);
                    var responseStr = await sw.ReadToEndAsync();

                    var replacementUrl = $"{request.Scheme}://{request.Host}{request.Path}?map={endpoint}";

                    responseStr = responseStr
                        .Replace($"{urlBase}/?", replacementUrl)
                        .Replace($"{urlBase}?", replacementUrl)
                        .Replace(urlBase, replacementUrl);

                    return new ObjectResult(responseStr)
                    {
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    var ms = new MemoryStream(tcOut.ResponseBinary);
                    response.RegisterForDispose(ms);

                    return new FileStreamResult(ms, tcOut.ResponseContentType);
                }
            }

            return new ObjectResult(tcOut)
            {
                StatusCode = (int)tcOut.StatusCode
            };
        }

        /// <summary>
        /// Custom wms exception generator
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static IActionResult WmsException(HttpResponse response, string message)
        {
            var wmsEx = new Cartomatic.OgcSchemas.Wms.Wms_1302.ServiceExceptionReport
            {
                ServiceException = new ServiceExceptionType[]
                {
                    new ServiceExceptionType
                    {
                        Value = message
                    }
                }
            };

            response.ContentType = "text/xml";

            return new ObjectResult(wmsEx)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

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
                return await HandleWmsRequestAsync(request, settings,  wmsDrv);
            }

            var requestParams = System.Web.HttpUtility.ParseQueryString(request.Address.Query);

            //getmap request?
            var wmsRequest = requestParams["request"];
            if (string.IsNullOrEmpty(wmsRequest) || wmsRequest.ToLower() != "getmap")
            {
                return await HandleWmsRequestAsync(request, settings,  wmsDrv);
            }



            //format compatible?
            var requestedFormatMime = requestParams["format"].ToLower();
            if (!Utils.WmsImageFormatSupported(requestedFormatMime, settings))
            {
                return await HandleWmsRequestAsync(request, settings,  wmsDrv);
            }


            //tile size compatible?
            int.TryParse(requestParams["height"], out var h);
            int.TryParse(requestParams["width"], out var w);

            if (w != ts.TileSize || h != ts.TileSize)
            {
                output = await HandleWmsRequestAsync(request, settings,  wmsDrv);

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
                return await HandleWmsRequestAsync(request, settings,  wmsDrv);
            }


            //get ready with tileset def
            ts.Prepare();

            if (!ts.Ready)
            {
                output = await HandleWmsRequestAsync(request, settings,  wmsDrv);

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
                output = await HandleWmsRequestAsync(request, settings,  wmsDrv);

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
            output = await HandleWmsRequestAsync(request, settings,  wmsDrv);


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
        /// <param name="settings"></param>
        /// <param name="wmsDrv"></param>
        private static async Task<Output> HandleWmsRequestAsync<T>(System.Net.HttpWebRequest request, Settings settings, T wmsDrv) where T : WmsDriver
        {
            return wmsDrv != null
                ? await HandleWmsDriverRequestAsync(request, wmsDrv)
                : await HandleStandardWebRequestAsync(request, settings);
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
        private static async Task<Output> HandleStandardWebRequestAsync(System.Net.HttpWebRequest request, Settings settings)
        {
            var output = new Output();


            var result = await GetHttpClient(settings).GetAsync(request.RequestUri);

            if (result.IsSuccessStatusCode)
            {
                output.ResponseContentType = result.Content.Headers.ContentType.MediaType;
                output.StatusCode = result.StatusCode;
                output.ResponseBinary = await result.Content.ReadAsByteArrayAsync();
                output.HasData = true;
            }
            else
            {
                output.StatusCode = result.StatusCode;
                output.ResponseText = result.ReasonPhrase;
            }

            return output;
        }

        /// <summary>
        /// default pool size
        /// </summary>
        private static int _dfltClientPoolSize = 4;

        /// <summary>
        /// http client use count
        /// </summary>
        private static long _clientUseCount;

        /// <summary>
        /// http client pool
        /// </summary>
        private static HttpClient[] _httpClientPool;

        /// <summary>
        /// returns a next http client from a pool
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static HttpClient GetHttpClient(Settings settings)
        {
            var nextClientIdx = Interlocked.Increment(ref _clientUseCount)  % (settings.HttpClientPoolSize ?? _dfltClientPoolSize);

            _httpClientPool ??= new HttpClient[settings.HttpClientPoolSize ?? _dfltClientPoolSize];

            _httpClientPool[nextClientIdx] ??= new HttpClient();

            return _httpClientPool[nextClientIdx];
        }


    }
}
