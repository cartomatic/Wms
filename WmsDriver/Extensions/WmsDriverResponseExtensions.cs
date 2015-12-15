using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms.WmsDriverExtensions
{
    public static class WmsDriverResponseExtensions
    {
        public static bool HasData(this IWmsDriverResponse wmsDriverResponse)
        {
            return wmsDriverResponse.ResponseBinary != null;
        }

        //TODO
        //could also have some logic to feed the data into web response
        //or something along these lines so the web handlers are simpler.
    }
}
