using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {

        /// <summary>
        /// Wms driver validation rule
        /// </summary>
        public interface IValidationRule
        {
            string Message { get; set; }
        
            WmsExceptionCode WmsEcExceptionCode { get; set; }

            Action<WmsDriver, IValidationRule> Action { get; set; }
        }
    }
}
