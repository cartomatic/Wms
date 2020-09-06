using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        /// <summary>
        /// Whether or not wms request is for a transparent image
        /// </summary>
        /// <returns></returns>
        protected bool GetTransparent()
        {
            return GetParam<bool>("transparent");
        }
    }
}
