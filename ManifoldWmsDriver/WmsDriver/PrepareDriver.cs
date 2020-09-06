using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        /// <summary>
        /// Performs driver specific setup
        /// </summary>
        protected override void PrepareDriver()
        {
            CreateMapServer();

            ExtractWmsDriverSettings();

            base.PrepareDriver();
        }

    }
}
