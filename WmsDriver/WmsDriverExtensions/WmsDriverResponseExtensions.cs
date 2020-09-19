using System.Web;
#if NETSTANDARD2_0 || NETCOREAPP3_1
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
#endif

namespace Cartomatic.Wms.WmsDriverExtensions
{
    public static class WmsDriverResponseExtensions
    {
        public static bool HasData(this IWmsDriverResponse wmsDriverResponse)
        {
            return wmsDriverResponse.ResponseBinary != null;
        }

        /// <summary>
        /// Transfers IWmsDriverResponse data to System.Web.HttpResponse
        /// </summary>
        /// <param name="wmsDriverResponse"></param>
        /// <param name="response"></param>
#if NETFULL
        public static void TransferToResponse(this IWmsDriverResponse wmsDriverResponse, HttpResponse response)
        {
            response.ContentType = wmsDriverResponse.ResponseContentType;
            if (wmsDriverResponse.HasData())
            {
                response.BinaryWrite(wmsDriverResponse.ResponseBinary);
            }
            else
            {
                response.Write(wmsDriverResponse.ResponseText);
            }
        }
#endif
#if NETSTANDARD2_0 || NETCOREAPP3_1
        public static async Task TransferToResponse(this IWmsDriverResponse wmsDriverResponse, HttpResponse response)
        {
            response.ContentType = wmsDriverResponse.ResponseContentType;
            if (wmsDriverResponse.HasData())
            {
                await response.Body.WriteAsync(wmsDriverResponse.ResponseBinary, 0, wmsDriverResponse.ResponseBinary.Length);
            }
            else
            {
                await response.WriteAsync(wmsDriverResponse.ResponseText);
            }
        }
        
#endif
    }
}
