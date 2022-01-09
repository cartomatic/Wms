using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.Wms.WmsDriverExtensions;

namespace Cartomatic.Wms.ManifoldWmsDriverWebTests
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Wms : IHttpHandler
    {
        IWmsDriver _drv = null;

        public void ProcessRequest(HttpContext context)
        {
            var drv = GetWmsDriver();

            IWmsDriverResponse drvResponse = null;
            
            //async so need to cheat in ashx handler
            Task.WaitAll(
        Task.Run(async () => drvResponse = await drv.HandleRequestAsync(context.Request.Url.AbsoluteUri))
            );


            drvResponse.TransferToResponse(context.Response);

            //complete the request
            context.ApplicationInstance.CompleteRequest();
        }

        private IWmsDriver GetWmsDriver()
        {
            if (_drv == null)
            {
                //work out the path to the served map file. This can be totally dynamic, as one handler can serve pretty much any number of projects

                //assuming the project structure is as in the repo, so the test project should be
                //located in ../../WmsDriverManifold/TestData/TestData.map
                var path = Path.Combine(
                    HttpContext.Current.Server.MapPath(HttpContext.Current.Request.Path),
                    "../../ManifoldWmsDriver/TestData/TestData.map"
                );

                //The most simplistic constructor
                _drv = new ManifoldWmsDriver(path);

                //Note:
                //Through the constructor could also customise the served component name 
                //and feed in the WmsServiceDescription created manually or deserialised
                //
                //_drv.MapComp / (_drv as WmsDriver).MapComp
                //_drv.ServiceDescription
            }

            return _drv;
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}