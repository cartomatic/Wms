using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected virtual IWmsDriverResponse HandleGetCapabilities()
        {
            var output = new WmsDriverResponse();

            //TODO - validation rules may depend on the service version. So will need to have a look at this when implementing 1.1.1

            //run all the getCaps checkups
            Validate(HandleGetCapabilitiesValidationRules);


            //TODO - when other versions implemented generated caps doc will depend on the version and will require a proper redirect!

            var capsDoc130 = GenerateWmsCapabilitiesDocument130();

            //TODO - need to return caps in the format specified or in the default for given version. so far so good as service supports 130 only and only xml
            output.ResponseContentType = "text/xml";
            output.ResponseText = capsDoc130.SerializeToXml();


            return output;
        }

        /// <summary>
        /// Generates capabilities document for version 1.3.0 of the service
        /// </summary>
        /// <returns></returns>
        protected virtual WMS_Capabilities GenerateWmsCapabilitiesDocument130()
        {
            var capsDoc = new WMS_Capabilities();
            capsDoc = GenerateCapsServiceSection130(capsDoc);
            capsDoc = GenerateCapsCapabilitiesSection130(capsDoc);
            capsDoc = GenerateCapsLayersSection130(capsDoc);

            return capsDoc;
        }

        /// <summary>
        /// Generates a service section of the capabilities document for version 1.3.0 of the service
        /// </summary>
        /// <param name="capsDoc"></param>
        /// <returns></returns>
        protected virtual WMS_Capabilities GenerateCapsServiceSection130(WMS_Capabilities capsDoc)
        {
            //init service object
            var s = new Service();

            //this is a first value in the enum, so would be assigned automatically anyway
            s.Name = ServiceName.WMS;

            //title & abstract
            s.Title = ServiceDescription.Title;
            s.Abstract = ServiceDescription.Abstract;


            //keywords
            string[] keywordsStr = ServiceDescription.Keywords.ToArray();
            List<Keyword> keywords = new List<Keyword>();
            foreach (string str in keywordsStr)
            {
                var k = new Keyword();
                k.Value = str;
                keywords.Add(k);
            }
            s.KeywordList = keywords.ToArray();


            //Online resource
            var or = new OnlineResource();
            or.type = typeType.simple;
            or.typeSpecified = true;
            or.href = GetPublicAccessURL();
            s.OnlineResource = or;


            //Contact information
            var ci = new ContactInformation();
            var cicpp = new ContactPersonPrimary();
            cicpp.ContactPerson = ServiceDescription.ContactPerson;
            cicpp.ContactOrganization = ServiceDescription.ContactOrganization;
            ci.ContactPersonPrimary = cicpp;
            ci.ContactPosition = ServiceDescription.ContactPosition;
            var cica = new ContactAddress();
            cica.AddressType = ServiceDescription.AddressType;
            cica.Address = ServiceDescription.Address;
            cica.City = ServiceDescription.City;
            cica.StateOrProvince = ServiceDescription.StateOrProvince;
            cica.PostCode = ServiceDescription.PostCode;
            cica.Country = ServiceDescription.Country;
            ci.ContactAddress = cica;
            ci.ContactVoiceTelephone = ServiceDescription.ContactVoiceTelephone;
            ci.ContactElectronicMailAddress = ServiceDescription.ContactElectronicMailAddress;
            s.ContactInformation = ci;

            s.Fees = ServiceDescription.Fees;
            s.AccessConstraints = ServiceDescription.AccessConstraints;

            if (ServiceDescription.LayerLimit.HasValue && ServiceDescription.LayerLimit > 0)
            {
                s.LayerLimit = ServiceDescription.LayerLimit.ToString();
            }

            if (ServiceDescription.MaxWidth.HasValue && ServiceDescription.MaxWidth > 0)
            {
                s.MaxWidth = ServiceDescription.MaxWidth.ToString();
            }

            if (ServiceDescription.MaxHeight.HasValue && ServiceDescription.MaxHeight > 0)
            {
                s.MaxHeight = ServiceDescription.MaxHeight.ToString();
            }

            capsDoc.Service = s;

            return capsDoc;
        }

        /// <summary>
        /// Generates a capabilities section of the capabilities document for version 1.3.0 of the service
        /// </summary>
        /// <param name="capsDoc"></param>
        /// <returns></returns>
        protected virtual WMS_Capabilities GenerateCapsCapabilitiesSection130(WMS_Capabilities capsDoc)
        {
            var c = new Capability();

            //Request
            var r = new Request();

            //get caps
            //----------
            var getcaps = new OperationType();
            getcaps.Format = SupportedGetCapabilitiesFormats["1.3.0"].ToArray();


            //caps DCPType
            List<DCPType> getcaps_dcptypes = new List<DCPType>();
            getcaps_dcptypes.Add(GenerateDCPType130(GetPublicAccessURL(), null));
            getcaps.DCPType = getcaps_dcptypes.ToArray();
            r.GetCapabilities = getcaps;


            //get map
            //----------
            var getmap = new OperationType();
            getmap.Format = SupportedGetMapFormats["1.3.0"].ToArray();

            //map DCPType
            List<DCPType> getmap_dcptypes = new List<DCPType>();
            getmap_dcptypes.Add(GenerateDCPType130(GetPublicAccessURL(), null));
            getmap.DCPType = getmap_dcptypes.ToArray();
            r.GetMap = getmap;


            //get feature info
            //----------
            var getinfo = new OperationType();
            if (SupportedGetFeatureInfoFormats.ContainsKey("1.3.0") && SupportedGetFeatureInfoFormats["1.3.0"].Count > 0)
            {
                getinfo.Format = SupportedGetFeatureInfoFormats["1.3.0"].ToArray();

                //map DCPType
                List<DCPType> getinfo_dcptypes = new List<DCPType>();
                getinfo_dcptypes.Add(GenerateDCPType130(GetPublicAccessURL(), null));
                getinfo.DCPType = getinfo_dcptypes.ToArray();

                r.GetFeatureInfo = getinfo;
            }

            //_ExtendedOperation
            //r._ExtendedOperation

            //exception formats
            //----------
            c.Exception = SupportedExceptionFormats["1.3.0"].ToArray();


            c.Request = r;

            capsDoc.Capability = c;

            return capsDoc;
        }

        /// <summary>
        /// Generates a layers section of the capabilities document for version 1.3.0 of the service
        /// </summary>
        /// <param name="capsDoc"></param>
        /// <returns></returns>
        protected virtual WMS_Capabilities GenerateCapsLayersSection130(WMS_Capabilities capsDoc)
        {
            throw new WmsDriverException(string.Format("IMPLEMENTATION ERROR: GetCapabilities is a mandatory operation for WMS {0}.", GetDeclaredOrMaxSupportedVersion()));
        }

        /// <summary>
        /// Generates DCPType element for the version 1.3.0 of the service
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="postUrl"></param>
        /// <returns></returns>
        protected virtual DCPType GenerateDCPType130(string getUrl, string postUrl)
        {
            var dcpt = new DCPType();
            dcpt.HTTP = new HTTP();

            if (!string.IsNullOrEmpty(getUrl))
            {
                dcpt.HTTP.Get = new Get();
                dcpt.HTTP.Get.OnlineResource = new OnlineResource();
                dcpt.HTTP.Get.OnlineResource.type = typeType.simple;
                dcpt.HTTP.Get.OnlineResource.typeSpecified = true;
                dcpt.HTTP.Get.OnlineResource.href = getUrl;
            }

            if (!string.IsNullOrEmpty(postUrl))
            {
                dcpt.HTTP.Post = new Post();
                dcpt.HTTP.Post.OnlineResource = new OnlineResource();
                dcpt.HTTP.Post.OnlineResource.type = typeType.simple;
                dcpt.HTTP.Post.OnlineResource.typeSpecified = true;
                dcpt.HTTP.Post.OnlineResource.href = postUrl;
            }

            return dcpt;
        }
    }
}
