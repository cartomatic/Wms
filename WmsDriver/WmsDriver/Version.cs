using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Finds the highest supported version of the service. particularly useful when returning get caps or exception for requests that do not specify version
        /// </summary>
        /// <returns></returns>
        protected internal string GetMaxSupportedVersion()
        {
            //Note:
            //For the time being assuming the patch number will bever go over 9.

            var max = (from v in SupportedVersions
                      select new {Version =  v, Rank = int.Parse(v.Replace(".", "")) }
                      into tempV
                      orderby tempV.Rank descending
                      select tempV.Version).FirstOrDefault();

            return max;
        }

        /// <summary>
        /// Gets declared or max supported version in a case request does not declare expected version
        /// </summary>
        /// <returns></returns>
        protected internal string GetDeclaredOrMaxSupportedVersion()
        {
            var version = GetParam("version");
            if (string.IsNullOrEmpty(version))
            {
                version = GetMaxSupportedVersion();
            }
            return version;
        }
    }
}
