namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        /// <summary>
        /// File save delegate used to do the saving in a separate thread
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="cacheFolder"></param>
        /// <param name="request"></param>
        /// <param name="tile"></param>
        public delegate void DelegateSaveFileBasedOnRequest(byte[] fileData, string cacheFolder, System.Net.HttpWebRequest request, Tile tile);

        /// <summary>
        /// File save delegate used to do the saving in a separate thread
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="cacheFolder"></param>
        /// <param name="requestParams"></param>
        /// <param name="tile"></param>
        public delegate void DelegateSaveFileBasedOnRequestParams(byte[] fileData, string cacheFolder, System.Collections.Specialized.NameValueCollection requestParams, Tile tile);

        /// <summary>
        /// Saves the tile to cache
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="cacheFolder"></param>
        /// <param name="request"></param>
        /// <param name="tile"></param>
        public static void SaveFile(byte[] fileData, string cacheFolder, System.Net.HttpWebRequest request, Tile tile)
        {
            SaveFile(
                fileData,
                cacheFolder,
                System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query),
                tile
            );
        }

        /// <summary>
        /// Saves the tile to cache
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="cacheFolder"></param>
        /// <param name="requestParams"></param>
        /// <param name="tile"></param>
        public static void SaveFile(byte[] fileData, string cacheFolder, System.Collections.Specialized.NameValueCollection requestParams, Tile tile)
        {
            if (!System.IO.Directory.Exists(cacheFolder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(cacheFolder);
                }
                catch
                {
                    return;
                }
            }

            var internalCachePath = GetInternalCachePath(requestParams);

            var fileDestinationDir = $@"{cacheFolder}\{internalCachePath}\{GetFileDestinationDirectory(tile)}";


            var fileExt = GetFileExtensionForMime(requestParams["format"]);

            var filePath = $@"{cacheFolder}\{internalCachePath}\{GetFilePath(tile, fileExt)}";

            try
            {
                if (!System.IO.Directory.Exists(fileDestinationDir))
                {
                    System.IO.Directory.CreateDirectory(fileDestinationDir);
                }

                System.IO.File.WriteAllBytes(filePath, fileData);
            }
            catch
            {
                //ignore
            }

        }

    }
}
