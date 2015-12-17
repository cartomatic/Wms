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
        /// Validation rule; used to specify a piece of logic used to validate wms request
        /// </summary>
        protected internal class ValidationRule : IValidationRule
        {

            public string Message { get; set; }

            public WmsExceptionCode WmsEcExceptionCode { get; set; }

            public Action<WmsDriver, IValidationRule> Action { get; set; }
        }

        /// <summary>
        /// Executes a set of specified validation rules
        /// </summary>
        /// <param name="rules"></param>
        protected void Validate(Dictionary<string, IValidationRule> rules)
        {
            foreach (var rule in rules)
            {
                var vr = rule.Value;
                if (vr.Action != null)
                {
                    vr.Action(this, vr);
                }
            }
        }
    }
}
