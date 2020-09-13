namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {

        /// <summary>
        /// Gets cached file response_binary for specified cache folder, web request and tile address
        /// Returns file response_binary or empty byte[] if file does not exist
        /// </summary>
        /// <param name="cacheFolder">Cache folder</param>
        /// <param name="request">Request for which the cached file may exist</param>
        /// <param name="tile">Tile with the tile address in the tileScheme</param>
        /// <returns></returns>
        public static byte[] GetCachedFile(string cacheFolder, System.Net.HttpWebRequest request, Tile tile)
        {
            return GetCachedFile(
                cacheFolder,
                System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query),
                tile
            );
        }

        /// <summary>
        /// Gets cached file response_binary for specified cache folder, url params and tile address
        /// Returns file response_binary or empty byte[] if file does not exist
        /// </summary>
        /// <param name="cacheFolder"></param>
        /// <param name="requestParams"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static byte[] GetCachedFile(string cacheFolder, System.Collections.Specialized.NameValueCollection requestParams, Tile tile)
        {
            return GetCachedFile(
                GetCachedFilePath(cacheFolder, requestParams, tile)
            );
        }


        /// <summary>
        /// Checks if a cached file exists and returns its response_binary if so; empty byte[] is returned otherwise
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetCachedFile(string filePath)
        {
            var output = new byte[0];
            if (System.IO.File.Exists(filePath))
            {
                output = System.IO.File.ReadAllBytes(filePath);
            }
            return output;
        }
    }
}
