namespace Cartomatic.Wms.TileCache
{
    public class Tile
    {
        public Tile() { }

        /// <summary>
        /// whether or not a tile fits within a tile scheme flag used by the tile set checkup procedures
        /// </summary>
        public bool Valid { get; set; }

        public Bounds Bounds { get; set; }
        public TileAddress Address { get; set; }
    }
}
