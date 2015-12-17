using System.Dynamic;

namespace Cartomatic.Wms
{
    public interface IWmsDriverResponse
    {
        /// <summary>
        /// Exception if any
        /// </summary>
        IWmsDriverException WmsDriverException { get; set; }

        /// <summary>
        /// Mime of the response
        /// </summary>
        string ResponseContentType { get; set; }

        /// <summary>
        /// Response text
        /// </summary>
        string ResponseText { get; set; }

        /// <summary>
        /// binary response data
        /// </summary>
        byte[] ResponseBinary { get; set; }
    }
}