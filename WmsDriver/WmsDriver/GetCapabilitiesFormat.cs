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
        /// Gets default or declared format for a get capabilities response
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        protected virtual string GetDeclaredOrDefaultGetCapabilitiesFormatForVersion(string version)
        {
            var format = GetParam("format");
            if (string.IsNullOrEmpty(format))
            {
                format = DefaultGetCapabilitiesFormats[version];
            }
            return format;
        }
    }
}
