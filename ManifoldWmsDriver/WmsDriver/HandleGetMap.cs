using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Drawing;
using Cartomatic.Wms;
using M = Manifold.Interop;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        protected override async Task<IWmsDriverResponse> HandleGetMapDriverSpecificAsync()
        {
            Validate(HandleGetMapValidationRulesDriverSpecific);

            //refreshing data means the internal project file dataset changes between requests. hence a need for locking
            //as mapserver can be accessed by multiple threads and it is more than likely things get messed up the stuff otherwise
            if (MSettings.AutoAoi)
            {
                lock (_aoiRefreshLocker)
                {
                    return RenderMap();
                }
            }
            else
            {
                return RenderMap();
            }
        }

        /// <summary>
        /// Prepares the map for rendering and delegates render process
        /// </summary>
        /// <returns></returns>
        protected internal IWmsDriverResponse RenderMap()
        {
            IWmsDriverResponse output = new WmsDriverResponse();

            var width = GetParam<int>("width");
            var height = GetParam<int>("height");

            var backColor = Color.White;
            if (GetTransparent())
            {
                backColor = Color.Transparent;
            }
            else
            {
                var bgColor = GetParam("bgcolor");
                if (!string.IsNullOrEmpty(bgColor))
                {
                    //If got here, validation rule has already ensured the color is parsable!
                    backColor = ColorTranslator.FromHtml(GetParam("BGCOLOR"));
                }
            }

            //Get the image format requested
            //Note: this should have been checked in the base driver against formats supported
            var imageEncoder = GetParam("FORMAT").GetEncoderInfo();


            var bbox = ParseBBOX(GetParam("bbox"), GetParam("version"), MSettings.SRID);


            //Note:
            //manifold WMS driver does not support styles so far as there is no such concept within manifold
            //Although themes linked with drawings could potentially be treated as styles but they would also have to be added as layers and that
            //kinda would makes styles useless again...
            //Could however add an option to hide themes and turn them on only if styles are requested. Not convinced though if its worth the effort... 


            //now turn on the requested layers
            ManageMapLayersVisibility(bbox.Width / width);


            //TODO Layer reorder according to query; likely to require locking as will modify the served component


            //perform AOI updates for the linked components if required
            if (MSettings.AutoAoi)
            {
                AutoAOI(bbox, width, height);
            }

            //finally render the raster
            output.ResponseContentType = imageEncoder.MimeType;
            output.ResponseBinary = Render(bbox, width, height, imageEncoder, backColor);

            return output;
        }
    }
}
