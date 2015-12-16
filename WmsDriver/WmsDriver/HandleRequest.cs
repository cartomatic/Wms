using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver : IWmsDriver
    {
        /// <summary>
        /// Handles WMS request in a form of a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IWmsDriverResponse Handle(string url)
        {
            return Handle(url.CreateHttpWebRequest());
        }


        public IWmsDriverResponse Handle(HttpWebRequest request)
        {
            IWmsDriverResponse output = null;

            try
            {
                //prepare driver


                //run all the initial request checkups


                //when ready delegate request handling based on the required operation


            }
            catch (WmsDriverException drve)
            {

            }
            catch (Exception ex)
            {
                
            }

            return output;
        }
    }
}
