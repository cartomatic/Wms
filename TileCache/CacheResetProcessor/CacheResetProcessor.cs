using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cartomatic.Wms.TileCache
{
    public partial class CacheResetProcessor
    {
        public class CacheResetSummary
        {
            public int FilesPossible { get; set; }
            public int FilesCleaned { get; set; }
            public int FilesFailed { get; set; }
        }

        /// <summary>
        /// Cleans cache for the specified bounding box and layer
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="ts"></param>
        /// <param name="maps">List of maps to clear the cache for</param>
        /// <param name="layers">List of layers to clear the cache for</param>
        /// <param name="minX">min x as specified by the wms 1.1.1 specs</param>
        /// <param name="maxX">max x as specified by the wms 1.1.1 specs</param>
        /// <param name="minY">min y as specified by the wms 1.1.1 specs</param>
        /// <param name="maxY">max y as specified by the wms 1.1.1 specs</param>
        /// <returns></returns>
        public static CacheResetSummary ResetCache(
            Settings settings,
            TileScheme ts,
            IEnumerable<string> maps,
            IEnumerable<string> layers,
            double minX, double maxX,
            double minY, double maxY
        )
        {
            var output = new CacheResetSummary();

            if (settings == null || ts == null)
            {
                return output;
            }

            //need maps in the first place
            if (maps == null || !maps.Any())
                return output;

            //first check if layers are present
            if (layers == null || !layers.Any())
                return output;

            ts.Prepare();

            //file names parts in a form of z/x/y path
            var zxyFileNames = new List<string>();

            //inspect the min, max xy for each zoom level for the 
            for (var z = 0; z < ts.NumZoomLevels; z++)
            {

                for (var x = ts.GetXColumn(minX, z); x <= ts.GetXColumn(maxX, z); x++)
                {
                    for (var y = ts.GetYColumn(minY, z); y <= ts.GetYColumn(maxY, z); y++)
                    {
                        if (x.HasValue && y.HasValue)
                        {
                            zxyFileNames.Add(Path.Combine(z.ToString(), Path.Combine(x.ToString(), y.ToString())));
                        }
                    }
                }
            }

            //at this stage we have a list of all the files that should be deleted for each cache
            //file names are in form of z\x\y
            //so will need to work out the formats too!


            //start inspecting the cache folder...

            //cache folder structure looks like this:
            //map\
            //   \epsg
            //        \layers
            //               \styles & transparency
            //                                     \image format
            //                                                  \z
            //                                                    \x
            //                                                      \y                        


            //find folders for maps to be cleaned up
            var mapDirs = new List<string>();
            foreach (var dir in Directory.GetDirectories(settings.CacheFolder))
            {
                var dirNameOnly = Path.GetFileName(dir); //file name should actually return dir name in this scenario...
                foreach (var m in maps)
                {
                    if (string.Compare(m, dirNameOnly, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        mapDirs.Add(dir);
                    }
                }
            }


            //need all the projection dirs
            var projectionDirs = new List<string>();
            foreach (var mapDir in mapDirs)
            {
                projectionDirs.AddRange(Directory.GetDirectories(mapDir));
            }

            //map layers are defined in the next level
            var layerDirs = new List<string>();
            foreach (var projectionDir in projectionDirs)
            {
                foreach (var layerDir in Directory.GetDirectories(projectionDir))
                {
                    foreach (var layer in layers)
                    {
                        if (layerDir.IndexOf(layer, StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            layerDirs.Add(layerDir);
                            break;
                        }
                    }
                }
            }

            //got the layer dirs, so next stage is styles; need all of them
            var stylesDirs = new List<string>();
            foreach (var layerDir in layerDirs)
            {
                stylesDirs.AddRange(Directory.GetDirectories(layerDir));
            }

            //the next level is the image formats level; need all the formats
            //make sure though to split the dirs between formats, so can actually know what is the format later without having to inspect the path
            var imageFormatDirs = new Dictionary<string, List<string>>();
            foreach (var stylesDir in stylesDirs)
            {
                foreach (var imageFormatDir in Directory.GetDirectories(stylesDir))
                {
                    var imgFormat = Path.GetFileName(imageFormatDir); //file name should actually return dir name in this scenario...
                    if (!imageFormatDirs.ContainsKey(imgFormat))
                    {
                        imageFormatDirs[imgFormat] = new List<string>();
                    }
                    imageFormatDirs[imgFormat].Add(imageFormatDir);
                }
            }

            //at this stage we're at the level of the actual files, so for each dir can now start assembling potential file paths
            var filesToDelete = new List<string>();

            foreach (var imageFormatDir in imageFormatDirs.Keys)
            {
                //need to workout format based on the allowed extensions
                var info = Cartomatic.Utils.Web.ContentType.GetContentTypeInfo(imageFormatDir.Replace("_", "/"));

                foreach (var cachePath in imageFormatDirs[imageFormatDir])
                {
                    foreach (var zxy in zxyFileNames)
                    {
                        output.FilesPossible++;
                        filesToDelete.Add(Path.Combine(cachePath, zxy) + info.extension);
                    }
                }
            }



            //finally can wipe out the files
            foreach (var file in filesToDelete)
            {
                try
                {
                    if (File.Exists(file)) //assuming that some other call may have removed the file in the meantime
                    {
                        File.Delete(file);
                        output.FilesCleaned++;
                    }
                }
                catch
                {
                    //retry delete
                    try
                    {
                        if (File.Exists(file)) //assuming that some other call may have removed the file in the meantime
                        {
                            File.Delete(file);
                            output.FilesCleaned++;
                        }
                    }
                    catch
                    {
                        output.FilesFailed++;
                    }
                }
            }

            return output;
        }

    }
}
