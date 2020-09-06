<%@ WebHandler Language="C#" Class="Wms" %>

using System;
using System.IO;
using System.Reflection;
using System.Web;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;
using WmsDriver = Cartomatic.Manifold.WmsDriver;

public class Wms : IHttpHandler
{

    IWmsDriver _drv = null;
    
    public void ProcessRequest (HttpContext context)
    {
        var drv = GetWmsDriver();

        var drvResponse = drv.HandleRequest(context.Request.Url.AbsoluteUri);

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
                "../../WmsDriverManifold/TestData/TestData.map"
            );
            
            //The most simplistic constructor
            _drv = new WmsDriver(path);
            
            //Note:
            //Through the constructor could also customise the served component name 
            //and feed in the WmsServiceDescription createde manually or deserialised
            //
            //_drv.MapComp / (_drv as WmsDriver).MapComp
            //_drv.ServiceDescription
        }

        return _drv;
    }

    public bool IsReusable {
        get {
            return true;
        }
    }

}