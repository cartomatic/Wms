namespace Cartomatic.Wms.TileCache
{
    public class Bounds
    {
        public double? MinX { get; set; }
        public double? MinY { get; set; }
        public double? MaxX { get; set; }
        public double? MaxY { get; set; }

        public Bounds() { }

        public Bounds(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public bool IsValid()
        {
            if (!MinX.HasValue || !MinY.HasValue || !MaxX.HasValue || !MaxY.HasValue)
            {
                return false;
            }

            return !(MinX > MaxX) && !(MinY > MaxY);
        }
        
    }
}
