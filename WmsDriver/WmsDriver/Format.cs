using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Imaging;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Used for setting up output format of image file
        /// </summary>
        protected internal ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(encoder => encoder.MimeType == mimeType || encoder.MimeType.Replace("image/", "") == mimeType);
        }
    }
}
