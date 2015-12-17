using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        protected void ExtractRequestParams(HttpWebRequest request)
        {
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
            string pValue = string.Empty;
            if(RequestParams != null)
                pValue = RequestParams[pName];
            return pValue;
        }
    }
}
