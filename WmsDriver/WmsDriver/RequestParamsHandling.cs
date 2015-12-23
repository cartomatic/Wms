using System.Collections.Specialized;
using System.Net;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {

        /// <summary>
        /// Params extracted from the request
        /// </summary>
        protected NameValueCollection RequestParams { get; set; }

        /// <summary>
        /// Extracts request params off the request object
        /// </summary>
        /// <param name="request"></param>
        protected internal void ExtractRequestParams(HttpWebRequest request)
        {
            Request = request;

            if(request != null)
                RequestParams = System.Web.HttpUtility.ParseQueryString(request.Address.Query);
        }

        /// <summary>
        /// Returns value of given param name
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        protected string GetParam(string pName )
        {
            string pValue = null;
            if(RequestParams != null)
                pValue = RequestParams[pName];
            return pValue;
        }

        /// <summary>
        /// Whether or not the param value casing should be respcted    
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetIgnoreCase()
        {
            //Note:
            //1.3.0 is case sensitive; many clients though have problems with it so we make it case insensitive
            //and ignore case for the time being
            //TODO - when other versions are supported, ignore case will have to depend on the version
            return true;
        }
    }
}
