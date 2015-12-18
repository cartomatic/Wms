using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected virtual IWmsDriverResponse HandleGetCapabilities()
        {
            return new WmsDriverResponse();
        }
    }
}
