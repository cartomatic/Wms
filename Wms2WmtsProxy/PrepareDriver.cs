using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Get caps lock
        /// </summary>
        protected object WmtsGetCapsLock = new object();

        /// <summary>
        /// Prepares the driver so it can be used for processing the requests
        /// </summary>
        protected override void PrepareDriver()
        {
            if (WmtsCaps == null)
            {
                lock (WmtsGetCapsLock)
                {
                    if (WmtsCaps == null)
                    {
                        PullWmtsCaps();

                        ConfigureWmsDriver();
                    }
                }
            }

            base.PrepareDriver();
        }
    }
}
