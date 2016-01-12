using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Utils.Drawing;

using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// Renders the map
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageEncoder"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        protected byte[] Render(WmsBoundingBox bbox, int width, int height, ImageCodecInfo imageEncoder, Color backColor)
        {
            byte[] output = null;



            //TODO - do not render the map if the bbox is outside of the map's bbox
            //This may be tricky with maps that only have linked AOI refreshed data...
            //maybe some manual map bbox would help here.
            //Note: if a map only contains such linked data with manifold 



            //work out the render quality for the mapserver
            MapServer.RenderQuality = this.GetRenderQuality();


            //prepare appropriate view extent
            MapServer.ViewCenterX = bbox.CenterX;
            MapServer.ViewCenterY = bbox.CenterY;

            MapServer.ViewScaleX = bbox.Width / width;
            MapServer.ViewScaleY = bbox.Height / height;

            MapServer.CX = width;
            MapServer.CY = height;


            MapServer.RenderFormat = GetMapServerRenderFormat(imageEncoder.MimeType);



            //get collar size
            int collar = GetCollarSize();

            //adjust tile size to include collar
            MapServer.CX += collar;
            MapServer.CY += collar;

            //output bitmap
            Bitmap baseImage;


            if (MSettings.RespectLayerOpacity)
            {
                //Note: so far only the respecting manifold opacity allows for customising the background colour.
                //Not that it really makes sense to use back colour when respecting opacity...
                //But reason for this is manifold not rendering transparent maps

                baseImage = RenderRespectingOpacity(backColor);

                //Note: no need to make img transparent here as the back color will already be transparent
            }
            else
            {
                baseImage = new Bitmap(new MemoryStream(MapServer.Render() as byte[], true));

                //make transparent
                if (GetTransparent())
                {
                    baseImage = MakeTransparent(baseImage);
                }

            }

            //create output image
            var outputImage = new Bitmap(width, height);

            //effective collar - if the requested size was less than 100 (manifold mapserver minimum size is 100 x 100)
            //the actual collar to be rendered may be different than expected (collar)
            int effectiveCollar = (baseImage.Width - outputImage.Width) / 2;
            //Note: the effective collar is the collar applied to each side of the image!



            if (effectiveCollar > 0)
            {
                //crop the image
                using (Graphics g = Graphics.FromImage(outputImage))
                {
                    g.DrawImage(
                        baseImage, //source image
                        new Rectangle(0, 0, outputImage.Width, outputImage.Height),//destination rect
                        new Rectangle(effectiveCollar, effectiveCollar, outputImage.Width, outputImage.Height),//source rect
                        GraphicsUnit.Pixel //drawing unit
                    );
                }
            }
            else
            {
                outputImage = baseImage;
            }


            //prepare data for output
            using (var ms = new MemoryStream())
            {
                //if the optimise Png flag has been set to true, optimise it
                //TODO - this could become async as this potentially would improve performance... 
                if (MapServer.RenderFormat == M.MapServerRenderFormat.MapServerRenderFormatPng && OptimisePng)
                {
                    nQuant.WuQuantizer q = new nQuant.WuQuantizer();
                    outputImage = (Bitmap)q.QuantizeImage(outputImage);
                }

                outputImage.Save(ms, imageEncoder, null);
                output = ms.ToArray();
            }


            return output;
        }


        /// <summary>
        /// Renders layers one by one and then puts them together into one image
        /// </summary>
        /// <returns></returns>
        private Bitmap RenderRespectingOpacity(Color backColor)
        {

            //first collect the layers that are on so need to be rendered - layers are collected so there is an access to their properties later on
            List<M.Layer> layersToBeRendered = new List<M.Layer>();
            foreach (M.Layer layer in ((M.Map)MapServer.Component).LayerSet)
            {
                //if a layer is visible in scale grab its name so it can be turned on later
                if (MapServer.get_LayerShown(layer.Component.Name))
                {
                    layersToBeRendered.Add(layer);
                }

                //turn layer off
                MapServer.TurnLayer(layer.Component.Name, false);
            }


            //new image that will contain the rendered data
            Bitmap outputImage = new Bitmap(MapServer.CX, MapServer.CY, PixelFormat.Format32bppArgb);

            //paint the base image with the required back colour
            using (Graphics g = Graphics.FromImage(outputImage))
            {
                g.FillRectangle(
                    new SolidBrush(backColor),
                    0,
                    0,
                    outputImage.Width,
                    outputImage.Height
                );
            }


            //now iterate through collected layers - turn a layer on, render image and turn it off
            //render layers from the bottom most to the top
            for (int l = layersToBeRendered.Count - 1; l >= 0; l--)
            {
                //get the layer
                M.Layer layer = layersToBeRendered[l];

                //turn it on
                MapServer.TurnLayer(layer.Component.Name, true);

                //do the actual map rendering and its manipulation
                outputImage = outputImage.OverlayBitmapWithOpacity(
                    MakeTransparent( //make sure the background is removed
                        new Bitmap(new MemoryStream(MapServer.Render() as byte[], true))//the image that is to be painted over the base image
                    ),
                    (float)((double)layer.Opacity / 100) //opacity value
                );

                //turn layer off again
                MapServer.TurnLayer(layer.Component.Name, false);
            }

            //return output bitmap
            return outputImage;
        }

        /// <summary>
        /// Maps mime to manifold MapServerRenderFormat
        /// </summary>
        /// <param name="mime"></param>
        /// <returns></returns>
        M.MapServerRenderFormat GetMapServerRenderFormat(string mime)
        {
            //by default do png
            M.MapServerRenderFormat output = M.MapServerRenderFormat.MapServerRenderFormatPng;

            switch (mime.ToLower())
            {

                case "image/jpg":
                case "image/jpeg":
                    output = M.MapServerRenderFormat.MapServerRenderFormatJpeg;
                    break;

                case "image/gif":
                    output = M.MapServerRenderFormat.MapServerRenderFormatGif;
                    break;
            }

            return output;
        }

        /// <summary>
        /// Gets the configured mapserver render quality
        /// </summary>
        /// <returns></returns>
        protected int GetRenderQuality()
        {
            int rQ = DEFAULT_RENDER_QUALITY;

            if (MSettings.RenderQuality.HasValue)
            {
                rQ = (int)MSettings.RenderQuality;
                if (rQ > 100) rQ = 100;
                if (rQ <= 0) rQ = 1;
            }

            return rQ;
        }

        /// <summary>
        /// Gets the collar size for map rendering
        /// </summary>
        /// <returns></returns>
        protected int GetCollarSize()
        {
            int collarSize = MSettings.CollarSize;

            //Note:
            //make the smart collar size choice - the basic collar size is smaller and provides correct vector rendering (so it does look ok on the edges
            //however some labels (especially the longe ones) may not render properly and appear shifted on the neighbour tiles. therefore a larger
            //collar is needed. 512 pixels collar (256 on each side) usually deos the trick but not always. 1024 collar size is much better in such situations

            //get the scale so multiple scale checkups are avoided in the LayerVisible method later on
            var scale = CheckScale();

            //go through all the layers of the current map component
            foreach (M.Layer l in ((M.Map)MapServer.Component).LayerSet)
            {

                //and only test the layers that have been turned on for rendering

                //check if layer is currently visible through the mapserver component 
                //note: l.Visible is the state of the layer on the map comp, which is not always the same as the this.mapServer.get_LayerShown(l)
                //that indicates whether the alyer is visible in the mapserver context
                if (MapServer.get_LayerShown(l.Component.Name))
                {
                    //when a label component is found
                    if (l.Component.Type == M.ComponentType.ComponentLabels)
                    {
                        //check if it is visible in current view
                        if (LayerVisible(l.Component.Name, scale))
                        {
                            //make collar size be the label collar size
                            collarSize = MSettings.LabelCollarSize;

                            //and break as there is no need to search for next labels layers
                            break;
                        }
                    }
                }

            }


            //Note:
            //Maximum size of an image manifold map server can render is 2048
            //therefore for larger images, it is needed to reduce collar size if a collarSize + CX || CY > 2048;
            //What is also important - manifold map server renders images only of sizes > 100 px and increased by 4
            //So it will render 100 x 104 properly, but 102 may not be 102 but 104 instead (or 100 I don't remember nor have time to test it now...)

            if (MapServer.CX + collarSize > 2048)
            {
                //first check what is the size remaining
                int cs = 2048 - MapServer.CX;

                //and make sure its a mutlitplication of 4
                cs = cs - (cs % 4);

                collarSize = cs;
            }

            //now do the same for CY
            if (MapServer.CX + collarSize > 2048)
            {
                //first check what is the size remaining
                int cs = 2048 - MapServer.CY;

                //and make sure its a mutlitplication of 4
                cs = cs - (cs % 4);

                collarSize = cs;
            }


            if (collarSize < 0) collarSize = 0;

            return collarSize;
        }

        /// <summary>
        /// Makes mapserver's map component background color transparent; works well when no antialiasing is on; otherwise edges will be blurry
        /// </summary>
        /// <param name="mapImageBitmap"></param>
        /// <returns></returns>
        private Bitmap MakeTransparent(Bitmap mapImageBitmap)
        {

            //get the background color
            //grab the map component first
            M.Color mapColor = ((M.Map)MapServer.Component).BackgroundColor as M.Color;

            Color colorToBeReplaced;
            if (mapColor == null) //this means the color was not previously set and it will be white by default (see context menu map, properities, background color
            {
                colorToBeReplaced = Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                //now create System.Drawing.Color based on the Manifold color
                colorToBeReplaced = Color.FromArgb(mapColor.Alpha, mapColor.Red, mapColor.Green, mapColor.Blue);
            }

            //make the color transparent
            mapImageBitmap.MakeTransparent(colorToBeReplaced);

            return mapImageBitmap;

        }
    }
}
