using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms.WmsDriverExtensions
{
    public static class WmsServiceDescriptionExtensions
    {
        /// <summary>
        /// Applies default service description; uses example data based on the default OGC capabiliteis example
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static IWmsServiceDescription ApplyDefaults(this IWmsServiceDescription sd)
        {
            sd.Title = "Acme Corp. Map Server";
            sd.Abstract = "Map Server maintained by Acme Corporation.  Contact: webmaster@wmt.acme.com.  High-quality maps showing roadrunner nests and possible ambush locations.";
            sd.Keywords = new List<string>() { "bird", "roadrunner", "ambush" };

            sd.ContactPerson = "Jeff Smith";
            sd.ContactOrganization = "NASA";
            sd.ContactPosition = "Computer Scientist";

            sd.AddressType = "postal";
            sd.Address = "NASA Goddard Space Flight Center";
            sd.City = "Greenbelt";
            sd.StateOrProvince = "MD";
            sd.PostCode = "20771";
            sd.Country = "USA";

            sd.ContactVoiceTelephone = "+1 301 555-1212";
            sd.ContactElectronicMailAddress = "user@host.com";

            sd.Fees = "none";
            sd.AccessConstraints = "none";

            sd.LayerLimit = 16;
            sd.MaxWidth = 2048;
            sd.MaxHeight = 2048;

            return sd;
        }
    }
}
