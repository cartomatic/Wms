namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        /// <summary>
        /// Checks if wmsFormatMime is supported for caching
        /// </summary>
        /// <param name="wmsFormatMime"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool WmsImageFormatSupported(string wmsFormatMime, Settings s)
        {
            //check if the user specified tile cache settings allow for caching of the format
            if (s.SupportedFormats == null || s.SupportedFormats.Count == 0 || s.SupportedFormats.Exists(f => f == wmsFormatMime))
            {
                //now check if the file format is ok for saving - checks whether the mime can be translated to a file extension

                var ext = GetFileExtensionForMime(wmsFormatMime);

                return !string.IsNullOrEmpty(ext) && ext != "unknown";
            }
            else
            {
                return false;
            }
        }
    }
}
