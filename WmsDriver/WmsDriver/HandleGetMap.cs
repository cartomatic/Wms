using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected virtual IWmsDriverResponse HandleGetMap()
        {
            Validate(HandleGetMapValidationRules);

            return HandleGetMapDriverSpecific();
        }

        protected virtual IWmsDriverResponse HandleGetMapDriverSpecific()
        {
            throw new WmsDriverException(string.Format("IMPLEMENTATION ERROR: GetMap is a mandatory operation for WMS {0}.", GetParam("version")));
        }
    }
}
