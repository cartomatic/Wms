using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// Whether or not wms request is for a transparent image
        /// </summary>
        /// <returns></returns>
        protected bool GetTransparent()
        {
            return string.Compare(GetParam("transparent"), "true", true) == 0;
        }
    }
}
