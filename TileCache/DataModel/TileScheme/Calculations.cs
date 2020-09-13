using System;

namespace Cartomatic.Wms.TileCache
{

    public partial class TileScheme
    {
        /// <summary>
        /// Prepares the tile set for further calculations
        /// </summary>
        public void Prepare()
        {
            if (Ready) return;

            CalculateResolutions();

            Ready = true;
        }

        /// <summary>
        /// Flags the TileScheme as not ready for the calculations
        /// 
        /// Whenever a property that affects the internals is set 
        /// UnPrepare() is called to let the class know it should recalculate some basic stuff
        /// </summary>
        private void UnPrepare()
        {
            Ready = false;
        }

        /// <summary>
        /// Calculates tile set's resolutions
        /// </summary>
        private void CalculateResolutions()
        {
            Resolutions = new decimal[NumZoomLevels];

            CalculateMaxResolution();

            for (var zl = 0; zl < NumZoomLevels; zl++)
            {
                Resolutions[zl] = decimal.Round(((MaxResolution ?? 0) * (decimal)(1 / Math.Pow(2, zl))), Precision);
            }
        }

        /// <summary>
        /// Calculates tile set's max resolution
        /// </summary>
        private void CalculateMaxResolution()
        {
            //Note: it assumed so far that x & y resolutions are equal

            if (!MaxResolution.HasValue)
            {
                MaxResolution = (decimal)(TileSetBounds.MaxX - TileSetBounds.MinX) / (TileSetBaseWidth * TileSize); ;
            }
        }


        /// <summary>
        /// Calculates tile address based on passed bounds
        /// </summary>
        /// <param name="tileBounds">Tile bounds</param>
        public Tile CalculateTileAddress(Bounds tileBounds)
        {
            var output = new Tile();

            if (!tileBounds.IsValid())
            {
                output.Valid = false;
                return output;
            }


            //check if the tile fits into the tile set bounds
            if (
                tileBounds.MinX < TileSetBounds.MinX ||
                tileBounds.MinY < TileSetBounds.MinY ||
                tileBounds.MaxX > TileSetBounds.MaxX ||
                tileBounds.MaxY > TileSetBounds.MaxY
            )
            {
                output.Valid = false;
                return output;
            }


            //Note:
            //when calculating resolutions there are problems with precision starting at 7th, 8th decimal place
            //doubles are quicker than decimals so as long as possible decimals are avoided for calculations, but used for comparisons
            //
            //therefore the internal tileset resolutions are counted using decimals rounded to a precision
            //specified by the precision param - see constructor for details
            //also because of that the calculated tile resolution for the comparison is converted to decimal and rounded
            //for the specified precision

            //first calculate the tile resolution and see if it is on the list of the available tileset resolutions
            var tileRes = (double)(tileBounds.MaxX - tileBounds.MinX) / TileSize;
            var decTileRes = decimal.Round((decimal)tileRes, Precision);

            var zoomLvl = Array.IndexOf(Resolutions, decTileRes); //use the rounded res!!!

            if (zoomLvl == -1)
            {
                //boom, no zoom lvl for the resolution
                output.Valid = false;
                return output;
            }

            //Get the tile offset against the tileset's start point
            var offsetX = (double)(tileBounds.MinX - TileSetBounds.MinX);


            //y may be reversed so depending on how the tilescheme is set adjust offset y
            double offsetY;
            if (ReverseY)
            {
                offsetY = (double)(TileSetBounds.MaxY - tileBounds.MaxY);
            }
            else
            {
                offsetY = (double)(tileBounds.MinY - TileSetBounds.MinY);
            }


            //The solution is to allow little differences as long as they are smaller than one pixel

            var tileIndexOffsetX = offsetX / (TileSize * tileRes);
            var tileIndexOffsetY = offsetY / (TileSize * tileRes);


            //get the reminders to see if there are any and if they fit in the allowed margin of pxSize
            var xrmdr = tileIndexOffsetX % 1;
            var yrmdr = tileIndexOffsetY % 1;

            //get the smaller reminder
            //if absolute value of 1 - rmdr is smaller than the rmdr it means that the tile address (row or col)
            //was something like 1.999... instead of 1.000...
            xrmdr = Math.Min(xrmdr, Math.Abs(1 - xrmdr));
            yrmdr = Math.Min(yrmdr, Math.Abs(1 - yrmdr));


            //pixel size expressed as the part of the whole column
            var pxSize = 1.0 / TileSize;

            if (xrmdr > pxSize || yrmdr > pxSize)
            {
                output.Valid = false;
            }
            else
            {
                //Note: 
                //casting to int does math floor and gives invalid tile addressing when addresses are .99999...
                //so need to round the number
                //(int)9.99999 is 9 while expected is 10
                //(int)9.00001 is 9 which is good
                //(int)-1.1 is -2 while round(-1.1) is -1

                output.Valid = true;
                output.Address = new TileAddress(z: zoomLvl, x: (int)Math.Round(tileIndexOffsetX), y: (int)Math.Round(tileIndexOffsetY));
            }

            return output;
        }


        /// <summary>
        /// Calculates tile bounds based on passed address
        /// </summary>
        /// <param name="tileAddress">Tile address</param>
        public Tile CalculateTileBounds(TileAddress tileAddress)
        {
            var output = new Tile();

            //check if the address is complete
            //Note: Currently only simple x,y,z addressing is supported
            if (!tileAddress.IsValid())
            {
                output.Valid = false;
                return output;
            }

            //first check if the tile zoom lvl is valid
            if (tileAddress.Z > Resolutions.Length)
            {
                output.Valid = false;
                return output;
            }

            var currentZoomLvlResolution = (double)Resolutions[(int)tileAddress.Z];

            //now calculate tileset size at zoom lvl - power of 2 (each zoom level enlarges tileset tile count by two on each side
            var currentZoomLvlTilesetWidth = TileSetBaseWidth * (int)Math.Pow(2, (int)tileAddress.Z);
            var currentZoomLevelTilesetHeight = TileSetBaseHeight * (int)Math.Pow(2, (int)tileAddress.Z);

            //and check if the tile address is valid
            if (
                tileAddress.X < 0 || tileAddress.X > currentZoomLvlTilesetWidth
                || tileAddress.Y < 0 || tileAddress.Y > currentZoomLevelTilesetHeight
            )
            {
                output.Valid = false;
                return output;
            }


            //at this stage it looks like the tile address is valid so can calculate its bounds

            //x bounds are easy
            var minX = (double)TileSetBounds.MinX + (int)tileAddress.X * TileSize * currentZoomLvlResolution;
            var maxX = minX + TileSize * currentZoomLvlResolution;


            //y bounds require different start point depending on the y start
            double minY, maxY;
            if (ReverseY) //y 0 on the top of tileset (tilsete max y!)
            {
                maxY = (double)TileSetBounds.MaxY - (int)tileAddress.Y * TileSize * currentZoomLvlResolution;
                minY = maxY - TileSize * currentZoomLvlResolution;
            }
            else
            {
                minY = (double)TileSetBounds.MinY + (int)tileAddress.Y * TileSize * currentZoomLvlResolution;
                maxY = minY + TileSize * currentZoomLvlResolution;
            }


            //This should now be ok
            //Though if not some more testing will be required - going from tile address to bounds and the other way round
            //so it is tested properly that bounds are similar after addressing tile and going back again

            output.Bounds = new Bounds(minX, minY, maxX, maxY);

            return output;
        }
    }
}
