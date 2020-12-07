using System;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms.TileCache
{
    public partial class TileScheme
    {

        /// <summary>
        /// Creates TileScheme object off the json definition
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TileScheme TileSchemeFromJson(string json)
        {
            TileScheme output = null;

            output = json.DeserializeFromJson<TileScheme>();

            return output;
        }

        public int GetTileSetWidthForLvl(int lvl)
        {
            return TileSetBaseWidth * (int)Math.Pow(2, lvl);
        }

        public int GetTileSetHeightForLvl(int lvl)
        {
            return TileSetBaseHeight * (int)Math.Pow(2, lvl);
        }

        /// <summary>
        /// Returns x tile address (x column) at given zoom level
        /// </summary>
        /// <param name="x">X coordinate in map units</param>
        /// <param name="zoomLvl"></param>
        /// <returns></returns>
        public int? GetXColumn(double x, int zoomLvl)
        {
            if (!Ready || zoomLvl >= Resolutions.Length)
            {
                return null;
            }

            return (int)Math.Floor((Math.Abs((double)TileSetBounds.MinX - x) / (double)Resolutions[zoomLvl]) / TileSize); 
        }

        /// <summary>
        /// Returns y tile address (y row) at given zoom level
        /// </summary>
        /// <param name="y"></param>
        /// <param name="zoomLvl"></param>
        /// <returns></returns>
        public int? GetYColumn(double y, int zoomLvl)
        {
            if (!Ready || zoomLvl >= Resolutions.Length)
            {
                return null;
            }


            if (ReverseY)
            {
                return (int)Math.Floor((Math.Abs((double)TileSetBounds.MaxY - y) / (double)Resolutions[zoomLvl]) / TileSize);
            }

            return (int)Math.Floor((Math.Abs((double)TileSetBounds.MinY - y) / (double)Resolutions[zoomLvl]) / TileSize);

        }

        /// <summary>
        /// Gets the total tile count for the tile set 
        /// </summary>
        /// <returns></returns>
        public long GetTileCount()
        {
            return GetTileCount(TileSetBounds);
        }


        /// <summary>
        /// Gets the total tile count for the passed bounds
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public long GetTileCount(Bounds b)
        {
            long tileCount = 0;

            for (int i = 0; i < NumZoomLevels; i++)
            {
                tileCount += GetTileCountForZoomLvl(b, i);
            }

            return tileCount;
        }

        /// <summary>
        /// Gets the tile count for the bounds and specified zoom levels
        /// </summary>
        /// <param name="b"></param>
        /// <param name="startLvl"></param>
        /// <param name="endLvl"></param>
        /// <returns></returns>
        public long GetTileCount(Bounds b, int startLvl, int endLvl)
        {
            long count = 0;

            for (var i = startLvl; i <= endLvl; i++)
            {
                count += GetTileCountForZoomLvl(b, i);
            }
            return count;
        }


        /// <summary>
        /// Gets tile count for a specified zoom level
        /// </summary>
        /// <param name="zoomLvl"></param>
        /// <returns></returns>
        public long GetTileCountForZoomLvl(int zoomLvl)
        {
            return GetTileCountForZoomLvl(TileSetBounds, zoomLvl);
        }

        /// <summary>
        /// Gets the tile count for the bounds and a specified zoom level
        /// </summary>
        /// <param name="b"></param>
        /// <param name="zoomLvl"></param>
        /// <returns></returns>
        public long GetTileCountForZoomLvl(Bounds b, int zoomLvl)
        {
            var minTileX = (int)GetXColumn((double)b.MinX, zoomLvl);
            var maxTileX = (int)GetXColumn((double)b.MaxX, zoomLvl);

            var minTileY = ReverseY ? (int)GetYColumn((double)b.MaxY, zoomLvl) : (int)GetYColumn((double)b.MinY, zoomLvl);
            var maxTileY = ReverseY ? (int)GetYColumn((double)b.MinY, zoomLvl) : (int)GetYColumn((double)b.MaxY, zoomLvl);

            var xTileCount = maxTileX - minTileX + 1; //+1 adds a tile as simple subtraction removes it
            var yTileCount = maxTileY - minTileY + 1;

            return (xTileCount) * (yTileCount);
        }
    }
}
