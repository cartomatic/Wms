using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Tries to work out if there is a vendor op implementation provided and calls it if so.
        /// Otherwise redirects back to Handle unsupported
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        protected virtual IWmsDriverResponse HandleVendorOp(string op)
        {
            //TODO - make sure vendor op is actually supported by the driver prior to trying to call it


            var t = this.GetType();
            var m = t.GetMethod(
                "Handle" + op,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase
            );

            if (m != null && m.ReturnType == typeof(IWmsDriverResponse))
            {
                return (IWmsDriverResponse)m.Invoke(this, null);
            }
            else
            {
                //no custom vendor handler found so just fallback for the unsupported but make it not search for vendor op again
                return HandleUnsupported(op, false);
            }
        }
    }
}
