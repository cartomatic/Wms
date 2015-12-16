using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal virtual IWmsDriverResponse HandleWmsDriverException(WmsDriverException e)
        {
            //TODO - when versions < 1.3.0 are to be handled, make exception output depend on WMS version as there are some minor differences
            var output = new WmsDriverResponse
            {
                ResponseContentType = "text/xml"
            };


            //TODO - the actual exception object handled properly!


            return output;
        }
    }
}
