using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Configures Wms driver based on the Wmts caps document
        /// </summary>
        protected internal void ConfigureWmsDriver()
        {
            if (WmtsCaps == null) return;
        }
    }
}
