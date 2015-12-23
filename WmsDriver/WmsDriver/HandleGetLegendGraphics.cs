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
        /// Generic GetLegendGraphic handler
        /// </summary>
        /// <returns></returns>
        protected virtual IWmsDriverResponse HandleGetLegendGraphic()
        {
            Validate(HandleGetLegendGraphicValidationRules);

            return HandleGetLegendGraphicDriverSpecific();
        }

        /// <summary>
        /// Driver specific GetLegendGraphic implementation
        /// </summary>
        /// <returns></returns>
        protected virtual IWmsDriverResponse HandleGetLegendGraphicDriverSpecific()
        {
            throw new WmsDriverException("IMPLEMENTATION ERROR: Operation 'GetLegendGraphic' is marked as supported but it is not implemented.");
        }
    }
}
