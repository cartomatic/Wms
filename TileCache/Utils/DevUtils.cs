using Cartomatic.Utils.Drawing;
using System;
using System.Drawing;
using System.IO;

namespace Cartomatic.Wms.TileCache
{
    public partial class Utils
    {
        /// <summary>
        /// Applies dev options as required
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="bmpData"></param>
        /// <param name="tile"></param>
        /// <param name="outMime"></param>
        /// <param name="cached"></param>
        /// <param name="failureMsg"></param>
        /// <returns></returns>
        public static byte[] ApplyDevOptions(Settings settings, byte[] bmpData, string outMime, Tile tile = null, bool cached = false, string failureMsg = null)
        {
            var output = bmpData;

            if (settings.DevModeStampTileAddress)
            {
                output = StampTileAddress(bmpData, outMime, tile, cached, failureMsg);
            }

            return output;
        }

        /// <summary>
        /// Prints a tile address in the centre along with additional information if provided.
        /// </summary>
        /// <param name="bmpData"></param>
        /// <param name="outMime"></param>
        /// <param name="t"></param>
        /// <param name="cached"></param>
        /// <param name="xtraMsg"></param>
        /// <returns></returns>
        private static byte[] StampTileAddress(byte[] bmpData, string outMime, Tile t, bool cached, string xtraMsg)
        {
            byte[] output = null;

            //read the image
            var img = bmpData.BitmapFromByteArr();

            //prepare msg
            var msg = "";
            if (!string.IsNullOrEmpty(xtraMsg))
            {
                msg = xtraMsg + Environment.NewLine;
            }

            if (t != null)
            {
                if (t.Valid)
                {
                    msg += "z:" + t.Address.Z + ", x:" + t.Address.X + ", y:" + t.Address.Y;
                }
                else
                {
                    msg += "invalid tile address";
                }
            }



            //prepare 
            var f = new System.Drawing.StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            //write the tile address info
            using (var g = Graphics.FromImage(img))
            {
                g.DrawString(
                    msg,
                    new System.Drawing.Font("Arial", 15, System.Drawing.GraphicsUnit.Pixel),
                    cached ? new SolidBrush(Color.Green) : new SolidBrush(Color.Red),
                    new RectangleF(0, 0, img.Width, img.Height),
                    f
                );
            }

            //save image to memory stream and convert to byte[]
            using (var ms = new MemoryStream())
            {
                img.Save(ms, outMime.GetEncoderInfo(), null);
                output = ms.ToArray();
            }

            return output;
        }

    }
}
