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
        /// Executes a set of specified validation rules
        /// </summary>
        /// <param name="rules"></param>
        protected void Validate<T>(Dictionary<string, Action<T>> rules) where T : WmsDriver
        {
            foreach (var rule in rules)
            {
                var vr = rule.Value;
                if (vr != null)
                {
                    vr((T)this);
                }
            }
        }
    }
}
