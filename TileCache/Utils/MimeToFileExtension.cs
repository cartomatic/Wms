using System.Collections.Generic;

namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        private static Dictionary<string, string> _mimeToFileExtensionCache { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Returns a file extension for given mime
        /// </summary>
        /// <param name="mime"></param>
        /// <returns></returns>
        private static string GetFileExtensionForMime(string mime)
        {
            if (!_mimeToFileExtensionCache.ContainsKey(mime))
            {
                var mimeInfo = Cartomatic.Utils.Web.ContentType.GetContentTypeInfo(mime);
                _mimeToFileExtensionCache[mime] = mimeInfo.extension;
            }
            return _mimeToFileExtensionCache[mime];
        }
    }
}
