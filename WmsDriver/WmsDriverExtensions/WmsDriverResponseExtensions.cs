using System.Web;

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
    }
}
