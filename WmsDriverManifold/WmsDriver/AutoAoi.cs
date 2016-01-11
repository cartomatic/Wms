using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// A locker used when performin AOI based data refreshes
        /// </summary>
        protected object _aoiRefreshLocker;

        /// <summary>
        /// Performs an AOI based data refresh of linked components
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void AutoAOI(WmsBoundingBox bbox, int? width, int? height)
        {
            throw new NotImplementedException();
        }
    }
}
