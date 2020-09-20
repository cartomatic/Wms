using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Cartomatic.Wms
{
    public static class MvcConfiguration
    {
        public static void ConfigureWmsDriverMvcOpts(this IMvcBuilder builder)
        {
            //Need to turn off default content not acceptable / content negotiation, as when WMS returns exceptions
            //they have different content type than the requested image
            builder.AddMvcOptions(opts =>
            {
                opts.ReturnHttpNotAcceptable = false;
                opts.OutputFormatters.Add(new Cartomatic.Wms.OutputFormatter.WmsXmlDataContractSerializerOutputFormatter());
            });
        }
    }
}
