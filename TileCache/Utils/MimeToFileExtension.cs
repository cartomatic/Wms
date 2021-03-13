using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        private static ConcurrentDictionary<string, string> _mimeToFileExtensionCache { get; set; } = new ConcurrentDictionary<string, string>();

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
                if (!_mimeToFileExtensionCache.TryAdd(mime, mimeInfo.extension))
                {
                    return mimeInfo.extension;
                }
            }

            return _mimeToFileExtensionCache.TryGetValue(mime, out var extension) 
                ? extension 
                : Cartomatic.Utils.Web.ContentType.GetContentTypeInfo(mime).extension;
        }
    }
}
