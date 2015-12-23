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
        /// Generic GetFeatureInfo handler
        /// </summary>
        /// <returns></returns>
        protected virtual IWmsDriverResponse HandleGetFeatureInfo()
        {
            //TODO - generic request validation as in other ops

            return HandleGetFeatureInfoDriverSpecific();
        }

        /// <summary>
        /// Driver specific implementation of GetFeatureInfo
        /// </summary>
        /// <returns></returns>
        protected internal virtual IWmsDriverResponse HandleGetFeatureInfoDriverSpecific()
        {
            throw new WmsDriverException("IMPLEMENTATION ERROR: Operation 'GetFeatureInfo' is marked as supported but it is not implemented.");
        }
    }
}
