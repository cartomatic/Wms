using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public class WmsDriverException : Exception, IWmsDriverException
    {
        public WmsExceptionCode WmsExceptionCode { get; set; }
    
        public WmsDriverException()
            : this("Unknown exception.")
        {
        }

        public WmsDriverException(string msg)
            : this(msg, WmsExceptionCode.NotApplicable)
        {
        }

        public WmsDriverException(string msg, WmsExceptionCode wmsExceptionCode)
            : this(msg, wmsExceptionCode, null)
        {
        }

        public WmsDriverException(string msg, Exception inner)
            : this(msg, WmsExceptionCode.NotApplicable, inner)
        {

        }

        public WmsDriverException(string msg, WmsExceptionCode wmsExceptionCode, Exception inner)
            : base(msg, inner)
        {
            WmsExceptionCode = wmsExceptionCode;
        }
    }
}
