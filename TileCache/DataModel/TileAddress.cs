namespace Cartomatic.Wms.TileCache
{
    public class TileAddress
    {
        public TileAddress() { }

        public TileAddress(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }

        public bool IsValid()
        {
            return Z.HasValue && X.HasValue && Y.HasValue;
        }
    }
}
