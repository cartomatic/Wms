using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{

    /// <summary>
    /// AOI settings used when refreshing 'fake' linked components
    /// </summary>
    public class ManifoldWmsDriverAoiSettings
    {
        public ManifoldWmsDriverAoiSettings() { }

        /// <summary>
        /// Name of the component being fake linked
        /// </summary>
        public string Comp { get; set; }

        /// <summary>
        /// db credentials
        /// </summary>
        public Cartomatic.Utils.Data.DataSourceCredentials DataSourceCredentials { get; set; }

        /// <summary>
        /// Whether or not the geometry is transferred as binary
        /// </summary>
        public bool UseBinaryGeom { get; set; }

        /// <summary>
        /// A query used to retrieve the data with appropriate (per data source type) replacement strings used to customise the AOI
        /// the replacement strings are:
        /// {t} - top edge of the bbox
        /// {b} - bottom edge of the bbox
        /// {l} - left edge of the bbox
        /// {r} - right edge of the bbox
        /// Examples:
        /// pgsql - select label, st_astext(geometry) as geom_wkt from public.velden where 'BOX({l} {b},{r} {t})'::box2d && "geometry";
        /// sqlserver - select label, [geometry].STAsText() as geom_wkt from dbo.velden where Geometry::STGeomFromText('POLYGON({l} {b},{l} {t},{r} {t}, {r} {b}, {l} {b})', 28992).STIntersects([Geometry]) = 1;
        /// </summary>
        public string Query { get; set; }


        //TODO collar to be used when refreshing the data for given component
        //public int? aoiCollar { get; set; }
    }
}
