using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// Returns true if the component can be querried
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private bool GetQueryable(M.Layer l)
        {
            //just vector comps are queryable
            if (l.Component.Type == M.ComponentType.ComponentDrawing || l.Component.Type == M.ComponentType.ComponentTheme)
            {
                return true;
            }

            return false;
        }
    }
}
