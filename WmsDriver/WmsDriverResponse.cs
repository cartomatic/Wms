using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public class WmsDriverResponse : IWmsDriverResponse
    {
        /// <summary>
        /// Wms driver exception if any
        /// </summary>
        public IWmsDriverException WmsDriverException { get; set; }

        /// <summary>
        /// Mime of the response
        /// </summary>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// Response text
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// binary response data
        /// </summary>
        public byte[] ResponseBinary { get; set; }
        
    }
}
