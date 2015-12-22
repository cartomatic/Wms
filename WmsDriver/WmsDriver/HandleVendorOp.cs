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

            //verify if the op is supported!
            var version = GetParam("version");

            if(!SupportedVendorOperations.ContainsKey(version) || SupportedVendorOperations[version] == null ||  !SupportedVendorOperations[version].Exists(vop => string.Compare(vop, op, GetIgnoreCase()) == 0))
                return HandleUnsupported(op, false);

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
                throw new WmsDriverException(string.Format("IMPLEMENTATION ERROR: Operation '{0}' is marked as supported but it is not implemented.", op));
            }
        }
    }
}
