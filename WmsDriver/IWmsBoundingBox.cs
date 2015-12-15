namespace Cartomatic.Wms
{
    public interface IWmsBoundingBox
    {
        double MinX { get; set; }

        double MaxX { get; set; }

        double MinY { get; set; }

        double MaxY { get; set; }

        double Width { get; }
        
        double Height { get; }
        
        double CenterX { get; }
        
        double CenterY { get; }
    }
}