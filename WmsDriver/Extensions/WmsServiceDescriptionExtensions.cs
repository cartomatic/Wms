using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Serialization;

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

        /// <summary>
        /// Creates WmsServiceInstance from json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IWmsServiceDescription WmsServiceDescriptionFromJson(this string json)
        {
            return json.DeserializeFromJson<WmsServiceDescription>();
        }

        public static void Merge(this IWmsServiceDescription sdBase,
            IWmsServiceDescription sdToBeMerged, params string[] concat )
        {
            sdBase.Merge(sdToBeMerged, (IEnumerable<string>)concat);
        }

        public static void Merge(this IWmsServiceDescription sdBase,
            IWmsServiceDescription sdToBeMerged, IEnumerable<string> concat = null )
        {
            if (sdToBeMerged == null) return; //ignore null objects!

            //get the object properties
            var props = sdBase.GetType().GetProperties();

            //now test the properties in order to transfer the values only if needed
            foreach (var p in props)
            {
                //get the type of a property
                var pv = p.GetValue(sdToBeMerged);

                if (p.PropertyType == typeof(string))
                {
                    //only transfer strings if not null and not empty
                    if (!string.IsNullOrEmpty(pv as string))
                    {
                        //if user specified that this property should be merged, do so
                        if (concat != null && concat.Contains(p.Name))
                        {
                            p.SetValue(
                                sdBase,
                                string.Concat(p.GetValue(sdBase), " ", pv)
                            );
                        }
                        //otherwise simply set the property value
                        else
                        {
                            p.SetValue(sdBase, pv);
                        }
                    }
                }
                else if (p.PropertyType == typeof(int?))
                {
                    if (pv != null) p.SetValue(sdBase, pv);
                }
                else if (p.PropertyType == typeof(List<string>))
                {
                    //Note:
                    //if keywords are declared, then do not override but rather combine
                    if (pv != null)
                    {
                        foreach (var s in pv as List<string>)
                        {
                            if (!sdBase.Keywords.Exists(keyword => keyword == s))
                            {
                                sdBase.Keywords.Add(s);
                            }
                        }
                    }
                }
            }
        }
    }
}
