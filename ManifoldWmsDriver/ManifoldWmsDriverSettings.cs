using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Wms
{
    /// <summary>
    /// Manifold specific driver settings
    /// </summary>
    public class ManifoldWmsDriverSettings
    {
        public ManifoldWmsDriverSettings() { }

        /// <summary>
        /// Name of the map component this settings apply to; One project file can contain multiple map comps that can be served by the driver
        /// </summary>
        public string MapComponent { get; set; }

        /// <summary>
        /// Spatial Reference System Identifier - EPSG int SRID identifier; manifold does not operate in epsg notation, so this needs to be specified manually
        /// </summary>
        public int SRID { get; set; }

        //TODO - SRID could be a list of available SRIDs for given component; component names could then be compnameSRID1, compnameSRID2, and so on...

        /// <summary>
        /// Whether or not all the layers should be combined and served as one layer; In such case map component becomes the only layer
        /// </summary>
        public bool CombineLayers { get; set; }

        /// <summary>
        /// Basic collar size to be used when rendering the image; used to improve the rendering on edges when rendering vectors
        /// </summary>
        public int CollarSize { get; set; }

        /// <summary>
        /// Label collar size to be used when rendering maps with labels; used to improve the rendering of labels so they do not get cut
        /// </summary>
        public int LabelCollarSize { get; set; }

        /// <summary>
        /// Whether or not the layer opacity should be respected; if true, layers are rendered one by one and assembled back as one image;
        /// This of course makes things slower, but it is not possible to set manifold's map component background to transparent for rendering (still true in 8.0.29.0)
        /// </summary>
        public bool RespectLayerOpacity { get; set; }

        /// <summary>
        /// render quality of the mapserver; if not provided a default render quality of 100 is used; applicability of this params varies between formats
        /// </summary>
        public int? RenderQuality { get; set; }

        /// <summary>
        /// Whether or not map component configured with this option ON should make its layers refresh the data for given bounding box or not.
        /// Only linked components are refreshed, other components are ignored. When using this option it is important to set manifold not to refresh linked
        /// components on file open.
        /// Note: Due to manifold inefficiences with linked data refresh for postgis and further problems
        /// with manifold 8 freezing on iis7/winServ2008 (not sure if the iis / os is the reason though)
        /// customised fake linking is implemented
        /// </summary>
        public bool AutoAoi { get; set; }

        /// <summary>
        /// AOI data collar size expressed in pixels - by how many pixels should the AOI be extended when refreshing the data
        /// The size is translated to the map units
        /// May improve the rendering, especially when objects are labeled, and the automatic label overlap resolve is on
        /// </summary>
        public int? AoiCollar { get; set; }


        //TODO:
        //AOI could be controlled per layer.
        //this is to be investigated further, as an empty layer is likely to not report proper bounding box when outputting capabilities
        //less likely scenario is that manifold will pull all the data, and then just kill the performance.
        //Note: since M9 is on the way it can wait a bit longer I guess
        
        /// <summary>
        /// Per component aoi settings; applicable only for the non-linked (fake linked) drawing components
        /// </summary>
        public List<ManifoldWmsDriverAoiSettings> AoiSettings { get; set; }


        /// <summary>
        /// The base WmsDriverSettings object; when used it overrides the one passed through a WmsDriver constructor
        /// </summary>
        public WmsServiceDescription WmsServiceDescription { get; set; }
    }
}
