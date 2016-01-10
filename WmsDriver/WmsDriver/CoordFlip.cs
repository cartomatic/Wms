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
        /// A set of coord systems that flip coords
        /// </summary>
        protected internal Dictionary<int, bool> CoordsFlippingSrids { get; set; }

        /// <summary>
        /// Populates the coords flipping srids holder with some predefined data
        /// Note:
        /// Need something smarter than manual srid list
        /// DotSpatial????
        /// Or an extract from the epsg db in a form of an embeded sqllite or an object
        /// </summary>
        protected void PopulateCoordFlippingSridsDict()
        {
            CoordsFlippingSrids = new Dictionary<int, bool>()
            {
                {4326, true},
                {2180, true}
            };
        }

        /// <summary>
        /// Adds a coord fliping srid so custom srid can be added in a case the defauld srid dataset does not contain it
        /// </summary>
        /// <param name="srid"></param>
        public void AddCoordFlippingSrid(int srid)
        {
            if (!CoordsFlippingSrids.ContainsKey((srid)))
            {
                CoordsFlippingSrids.Add(srid, true);
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
                if (srid.HasValue && CoordsFlippingSrids.ContainsKey(srid.Value))
                {
                    flip = true;
                }
            }

            return flip;
        }
    }
}
