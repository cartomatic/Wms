using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Wms_1302 = Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        
        protected IWmsDriverResponse HandleWmsDriverException(Exception e)
        {
            return HandleWmsDriverException(new WmsDriverException(e.Message, WmsExceptionCode.NotApplicable));
        }
        
        protected IWmsDriverResponse HandleWmsDriverException(WmsDriverException e)
        {
            
            //TODO - when versions other thsn 1.3.0 are to be handled, make exception output depend on WMS version as there are some differences
            //TODO - at some point look at other exception formats than xml - at this stage though it is enough to only handle xml
            var output = new WmsDriverResponse
            {
                WmsDriverException = e,
                ResponseContentType = "text/xml",
                ResponseText = PrepareExceptionWms130(e)
            };

            return output;
        }

        /// <summary>
        /// Prepares wms 130 exception
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected string PrepareExceptionWms130(WmsDriverException e)
        {
            //service exception report
            var ser = new Wms_1302.ServiceExceptionReport();

            //service exception
            var se = new Wms_1302.ServiceExceptionType();
            se.Value = e.Message;
            se.code = e.WmsExceptionCode.ToString();

            //assign an array of the exceptions to the output
            ser.ServiceException = new [] { se };

            return ser.SerializeToXml();
        }
    }
}
