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
        protected override IWmsDriverResponse HandleGetLegendGraphic()
        {
            Validate(HandleGetLegendGraphicValidationRulesDriverSpecific);

            var output = new WmsDriverResponse();




            return output;
        }
    }
}
