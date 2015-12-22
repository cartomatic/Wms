using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Parses WMS bounding box - takes in value of the bbox param
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="version"></param>
        /// <param name="epsg"></param>
        /// <returns></returns>
        protected internal WmsBoundingBox ParseBBOX(string bbox, string version, string epsg)
        {
            int srid;
            if (!string.IsNullOrEmpty(epsg) && Int32.TryParse(epsg.ToLower().Replace("epsg:", ""), out srid))
            {
                return ParseBBOX(bbox, version, srid);
            }
            else
            {
                return ParseBBOX(bbox, version, (int?)null);
            }
        }

        /// <summary>
        /// Parses WMS bounding box - takes in value of the bbox param
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="version"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        protected internal WmsBoundingBox ParseBBOX(string bbox, string version, int? srid)
        {
            var strVals = bbox.Split(new[] { ',' });

            var wmsex = new WmsDriverException("Value of the BBOX param is not valid.", WmsExceptionCode.InvalidDimensionValue);

            if (strVals.Length != 4)
                throw wmsex;

            double minx, miny, maxx, maxy;

            if (!double.TryParse(strVals[0], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out minx))
                throw wmsex;

            if (!double.TryParse(strVals[2], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out maxx))
                throw wmsex;

            if (!double.TryParse(strVals[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out miny))
                throw wmsex;

            if (!double.TryParse(strVals[3], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out maxy))
                throw wmsex;


            //TODO: need to recognise bounds based on the WMS version and epsg code!!!!

            try
            {
                return GetCoordFlip(version, srid)
                    ? new WmsBoundingBox(miny, minx, maxy, maxx)

                    : new WmsBoundingBox(minx, miny, maxx, maxy);
            }
            catch (Exception ex)
            {
                throw new WmsDriverException(ex.Message, WmsExceptionCode.InvalidDimensionValue);
            }
        }

        /// <summary>
        /// Whether or not srid specified flips bbox coords
        /// </summary>
        /// <param name="version"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        protected internal virtual bool GetCoordFlip(string version, int? srid)
        {
            bool flip = false;

            if (version == "1.3.0")
            {

                //list of srids that flip coords
                //Note:
                //Need something smarter than manual srid list
                //DotSpatial????
                //Or an extract from the epsg db in a form of an embeded sqllite or an object
                //
                int[] srids = {
                    4326,
                    2180
                };

                if (Array.Exists(srids, element => element == srid))
                {
                    flip = true;
                }

            }

            return flip;
        }
    }
}
