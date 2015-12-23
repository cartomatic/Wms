using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms.WmsDriverExtensions;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Performs driver specific setup; expected to throw WmsDriverException when unable to set up the driver properly;
        /// </summary>
        protected internal virtual void PrepareDriver()
        {
            ValidateWmsDescription();
        }


        /// <summary>
        /// If no wms description is provided, a default data set is provided instead
        /// </summary>
        protected internal virtual void ValidateWmsDescription()
        {
            //if no wms service description provided, use the default one
            if (ServiceDescription == null)
            {
                ServiceDescription = new WmsServiceDescription().ApplyDefaults();
            }
        }
    }
}
