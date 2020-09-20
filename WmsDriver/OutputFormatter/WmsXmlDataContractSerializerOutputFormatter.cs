using Cartomatic.Utils.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms.OutputFormatter
{
    public class WmsXmlDataContractSerializerOutputFormatter : TextOutputFormatter
    {
        public WmsXmlDataContractSerializerOutputFormatter()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedEncodings.Clear();

            //Look for specific media type declared with Content-Type header in request  
            this.SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/xml"));
            this.SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/xml"));
            this.SupportedEncodings.Add(new UTF8Encoding());
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return base.CanWriteResult(context);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;

            var content = string.Empty;
            if (context.Object is string contextObjectStr)
            {
                content = contextObjectStr;
            }
            else
            {
                content = context.Object.SerializeToXml();
            }

            response.WriteAsync(content);

            return Task.FromResult(response);
        }
    }
}
