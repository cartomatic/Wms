namespace Cartomatic.Wms.TileCache
{
    public class Output
    {
        public Output() { }

        /// <summary>
        /// Whether or not the output contains binary response data
        /// </summary>
        public bool HasData { get; set; }

        /// <summary>
        /// Whether or not the output contains the cached file path
        /// </summary>
        public bool HasFile { get; set; }

        /// <summary>
        /// cached file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Mime of the response
        /// </summary>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// Response text
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// binary response data response_binary
        /// </summary>
        public byte[] ResponseBinary { get; set; }
    }

}
