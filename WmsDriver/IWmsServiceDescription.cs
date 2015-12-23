using System.Collections.Generic;

namespace Cartomatic.Wms
{
    /// <summary>
    /// Basic WMS Service description. Used to provide proper description data to the driver so it can then create a Capabilities document
    /// </summary>
    public interface IWmsServiceDescription
    {
        /// <summary>
        /// Public url to access the service in case service is hosted behind firewall or service requires params, etc.
        /// </summary>
        string PublicAccessURL { get; set; }

        /// <summary>
        /// Mandatory Human-readable title for pick lists
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Optional narrative description providing additional information
        /// </summary>
        string Abstract { get; set; }

        /// <summary>
        /// Optional list of keywords or keyword phrases describing the server as a whole to help catalog searching
        /// </summary>
        List<string> Keywords { get; set; }

        /// <summary>
        /// Primary contact person
        /// </summary>
        string ContactPerson { get; set; }

        /// <summary>
        /// Organisation of primary person
        /// </summary>
        string ContactOrganization { get; set; }

        /// <summary>
        /// Position of contact person
        /// </summary>
        string ContactPosition { get; set; }

        /// <summary>
        /// Type of address (usually "postal").
        /// </summary>
        string AddressType { get; set; }

        /// <summary>
        /// Contact address
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Contact City
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// State or province of contact
        /// </summary>
        string StateOrProvince { get; set; }

        /// <summary>
        /// Postcode of contact
        /// </summary>
        string PostCode { get; set; }

        /// <summary>
        /// Country of contact address
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// Telephone
        /// </summary>
        string ContactVoiceTelephone { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        string ContactElectronicMailAddress { get; set; }

        /// <summary>
        /// The optional element "Fees" may be omitted if it do not apply to the server. If
        ///  the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        ///  fees, as follows: "none".
        /// </summary>
        string Fees { get; set; }

        /// <summary>
        /// <para>The optional element "AccessConstraints" may be omitted if it do not apply to the server. If
        ///  the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        ///  access constraints, as follows: "none".</para>
        ///  <para>When constraints are imposed, no precise syntax has been defined for the text content of these elements, but
        ///  client applications may display the content for user information and action.</para>
        /// </summary>
        string AccessConstraints { get; set; }

        /// <summary>
        /// Maximum number of layers allowed (0=no restrictions)
        /// </summary>
        int? LayerLimit { get; set; }

        /// <summary>
        /// Maximum width allowed in pixels
        /// </summary>
        int? MaxWidth { get; set; }

        /// <summary>
        /// Maximum height allowed in pixels
        /// </summary>
        int? MaxHeight { get; set; }
    }
}