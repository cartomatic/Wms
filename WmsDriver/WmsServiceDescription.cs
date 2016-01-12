using System.Collections.Generic;

namespace Cartomatic.Wms
{
    /// <summary>
    /// An object used to pass the service description data to the driver, so it can generate capabilities document
    /// </summary>
    public class WmsServiceDescription : IWmsServiceDescription
    {
        /// <summary>
        /// Public url to access the service in case service is hosted behind a firewall or service requires params, etc.
        /// </summary>
        public string PublicAccessURL { get; set; }

        /// <summary>
        /// Mandatory Human-readable title for pick lists
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional narrative description providing additional information
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Optional list of keywords or keyword phrases describing the server as a whole to help catalog searching
        /// </summary>
        public List<string> Keywords { get; set; }

        /// <summary>
        /// Primary contact person
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// Organisation of primary person
        /// </summary>
        public string ContactOrganization { get; set; }

        /// <summary>
        /// Position of contact person
        /// </summary>
        public string ContactPosition { get; set; }

        /// <summary>
        /// Type of address (usually "postal").
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// Contact address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Contact City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State or province of contact
        /// </summary>
        public string StateOrProvince { get; set; }

        /// <summary>
        /// Postcode of contact
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Country of contact address
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Telephone
        /// </summary>
        public string ContactVoiceTelephone { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string ContactElectronicMailAddress { get; set; }

        /// <summary>
        /// The optional element "Fees" may be omitted if it do not apply to the server. If
        //  the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        //  fees, as follows: "none".
        /// </summary>
        public string Fees { get; set; }

        /// <summary>
        /// <para>The optional element "AccessConstraints" may be omitted if it do not apply to the server. If
        //  the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        //  access constraints, as follows: "none".</para>
        //  <para>When constraints are imposed, no precise syntax has been defined for the text content of these elements, but
        //  client applications may display the content for user information and action.</para>
        /// </summary>
        public string AccessConstraints { get; set; }

        /// <summary>
        /// Maximum number of layers allowed (0=no restrictions)
        /// </summary>
        public int? LayerLimit { get; set; }

        /// <summary>
        /// Maximum width allowed in pixels
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Maximum height allowed in pixels
        /// </summary>
        public int? MaxHeight { get; set; }

        public WmsServiceDescription()
        {
            Keywords = new List<string>();
        }
    }
}
