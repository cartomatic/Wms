using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Manifold;

namespace WmsDriverManifold.Tests
{
    public static class Utils
    {
        /// <summary>
        /// Gets a path to a map file off the ncrunch workspace - ncrunch copies all the stuff there
        /// </summary>
        /// <returns></returns>
        public static string GetMapFilePath()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(WmsDriver)).Location);
            var file = "..\\..\\TestData\\TestData.map";
            var mapFile = Path.Combine(path, file);

            return mapFile;
        }
    }
}
