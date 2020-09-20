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
        protected virtual async Task<IWmsDriverResponse> HandleGetFeatureInfoAsync()
        {
            //TODO - generic request validation as in other ops

            return await HandleGetFeatureInfoDriverSpecificAsync();
        }

        /// <summary>
        /// Driver specific implementation of GetFeatureInfo
        /// </summary>
        /// <returns></returns>
        protected internal virtual async Task<IWmsDriverResponse> HandleGetFeatureInfoDriverSpecificAsync()
        {
            throw new WmsDriverException("IMPLEMENTATION ERROR: Operation 'GetFeatureInfo' is marked as supported but it is not implemented.");
        }
    }
}
