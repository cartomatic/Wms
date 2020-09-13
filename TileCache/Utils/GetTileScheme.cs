namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        /// <summary>
        /// A convenience method of getting TileScheme from json;
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TileScheme GetTileScheme(string json)
        {
            return TileScheme.TileSchemeFromJson(json);
        }

    }
}
