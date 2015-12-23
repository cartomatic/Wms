using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected virtual IWmsDriverResponse HandleUnsupported(string operation, bool tryVendorOp = true)
        {
            if (tryVendorOp)
            {
                return HandleVendorOp(operation);
            }
            else
            {
                throw new WmsDriverException(string.Format("Operation '{0}' is not supported.", operation));
            }
        }
    }
}
