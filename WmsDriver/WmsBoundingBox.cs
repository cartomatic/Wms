using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.WmsDriver
{
    public class WmsBoundingBox : IWmsBoundingBox
    {
        public WmsBoundingBox(double minX, double minY, double maxX, double maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        private double _minX = double.MinValue;

        public double MinX
        {
            get { return _minX; }
            set
            {
                if (value >= MaxX)
                {
                    throw new ArgumentException("MinX should be less than MaxX");
                }
                _minX = value;
            }
        }

        private double _minY = double.MinValue;

        public double MinY
        {
            get { return _minY; }
            set
            {
                if (value >= MaxY)
                {
                    throw new ArgumentException("MinY should be less than MaxY");
                }
                _minY = value;
            }
        }

        private double _maxX = double.MaxValue;

        public double MaxX
        {
            get { return _maxX; }
            set
            {
                if(value <= MinX)
                {
                    throw new ArgumentException("MaxX should be greater than MinX");
                }
                _maxX = value;
            }
        }

        private double _maxY = double.MaxValue;

        public double MaxY
        {
            get { return _maxY; }
            set
            {
                if (value <= MinY)
                {
                    throw new ArgumentException("MaxY should be greater than MinY");
                }
                _maxY = value;
            }
        }

        public double Width
        {
            get { return MaxX - MinX; }
        }

        public double Height
        {
            get { return MaxY - MinY; }
        }

        public double CenterX
        {
            get { return MinX + Width/2; }
        }

        public double CenterY
        {
            get { return MinY + Height/2; }
        }


    }
}
