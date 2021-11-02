using Cartomatic.Utils;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        /// <summary>
        /// Calculates file path based of the tile address and requested file extension
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetFilePath(Tile tile, string ext)
        {
            return $@"{tile.Address.Z}\{tile.Address.X}\{tile.Address.Y}{ext}";
        }

        /// <summary>
        /// Gets the cached file destination directory based on tile address
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static string GetFileDestinationDirectory(Tile tile)
        {
            return $@"{tile.Address.Z}\{tile.Address.X}";
        }

        /// <summary>
        /// Gets the internal cache path based on the requested wms url
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetInternalCachePath(System.Net.HttpWebRequest request)
        {
            return GetInternalCachePath(System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query));
        }


        /// <summary>
        /// Gets the internal cache path based on the requested wms url params
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public static string GetInternalCachePath(System.Collections.Specialized.NameValueCollection requestParams)
        {
            var output = string.Empty;


            //extract the service url
            //this will generate independent paths for subdomains so in fact will make it impossible
            //to use the common cache for different subdomains and therefore there is no point in using it here
            //if there is a need to separate cache for different wms services in one storage a decision process should be made at the handler level
            //rather than here as playing with such separation here effectively makes the cache less flexible
            //string serviceUrl = request.Address.Authority;
            //if (!string.IsNullOrEmpty(request.Address.AbsolutePath))
            //{
            //    serviceUrl += "_" + request.Address.AbsolutePath;
            //}
            //serviceUrl = httpUtils.EncodeUrl(serviceUrl);


            var endpoint = requestParams["endpoint"];
            if (!string.IsNullOrEmpty(endpoint))
            {
                if (!string.IsNullOrEmpty(output)) output += "\\";
                output += endpoint;
            }


            //extract the map param if any - some wms use map (qgis for example or our implementation of wms for manifold)
            //this is needed if more than one map is served by wms; otherwise cache would have random other stuff comming from different maps
            var map = requestParams["map"];

            //In a case this is a qgis wms or sharpmap wms, map param specifies a file path
            //need to extract the actual file name from there

            if (!string.IsNullOrWhiteSpace(map) && map.IsAbsolute()) //check if this is an absolute path - for services that need to read a file it should be
            //if (map.IndexOf(".qgs") > -1)
            {
                map = System.IO.Path.GetFileName(map);
            }

            //this is manifold wms driver specific - actually usually the driver will use any combination of both and sometimes will not use any of them
            var source = requestParams["source"];

            //merge source with map and make it one param - start with source
            if (!string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(map))
                {
                    map = source;
                }
                else
                {
                    map = source + "_" + map;
                }
            }

            //extract the epsg code - use epsg code as a subfolder too, not only the map
            var epsg = requestParams["srs"];
            if (string.IsNullOrEmpty(epsg))
            {
                epsg = requestParams["crs"];
            }


            //extract the layers string
            var layers = requestParams["layers"].EncodeUrl();

            //some wms may allow for using * to request all layers
            //In particular this applies to the Cartomatic.Manifold.WmsDriver
            //This is of course not allowed in the WMS specs and will make the cache fail to create
            //a folder as * is not an allowed path caracter.
            //Therefore it is necessary to remove the * and substitute it with a replacement string
            //for now this is gonna bee simply '__all__' as this is kinda unlikely one will have such layer name
            layers = layers.Replace("*", "__all__");


            //output = serviceUrl;

            if (!string.IsNullOrEmpty(map))
            {
                if (!string.IsNullOrEmpty(output)) output += "\\";
                output += map;
            }

            if (!string.IsNullOrEmpty(epsg))
            {
                if (!string.IsNullOrEmpty(output)) output += "\\";
                output += epsg.ToLower().Replace(":", "_");
            }

            output += "\\" + layers;

            //Add style, transparency and bgcolor as well
            //make sure to add none or somthing to the empty style param!
            //can styles be comma separated or something??? as layers can?
            //just in a case encode the style string too.
            var styles = requestParams["styles"];
            var transparent = requestParams["transparent"];
            var bgcolor = ""; //initially empty as when transparent, bg color is always transparent

            if (string.IsNullOrEmpty(styles))
            {
                styles = "style_default";
            }
            else
            {
                styles = "style_" + styles.EncodeUrl();
            }


            if (string.Compare(requestParams["TRANSPARENT"], "TRUE", true) == 0)
            {
                transparent = ";transparent_true";
            }
            else
            {
                transparent = ";transparent_false";

                bgcolor = requestParams["bgcolor"];

                if (!string.IsNullOrEmpty(bgcolor))
                {
                    bgcolor = ";bgcolor_" + bgcolor.EncodeUrl();
                }
                else
                {
                    bgcolor = ";bgcolor_default";
                }
            }

            output += "\\" + styles + transparent + bgcolor;



            //finally make sure it is possible to create separate caches for different file formats
            var format = requestParams["format"].Replace("/", "_");
            output += "\\" + format;


            return output;
        }


        /// <summary>
        /// Returns a path of a file as it should appear in the cache
        /// </summary>
        /// <param name="cacheFolder"></param>
        /// <param name="request"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static string GetCachedFilePath(string cacheFolder, System.Net.HttpWebRequest request, Tile tile)
        {
            return GetCachedFilePath(
                cacheFolder,
                System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query),
                tile
            );
        }

        /// <summary>
        /// Returns a path of a file as it should appear in the cache
        /// </summary>
        /// <param name="cacheFolder"></param>
        /// <param name="requestParams"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static string GetCachedFilePath(string cacheFolder, System.Collections.Specialized.NameValueCollection requestParams, Tile tile)
        {
            var internalCachePath = GetInternalCachePath(requestParams);

            var fileExt = GetFileExtensionForMime(requestParams["format"]);

            var filePath = $@"{cacheFolder}\{internalCachePath}\{GetFilePath(tile, fileExt)}";

            return filePath;
        }

    }
}
