namespace Cartomatic.Wms
{
    public interface IWmsDriverResponse
    {
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