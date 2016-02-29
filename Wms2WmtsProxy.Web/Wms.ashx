<%@ WebHandler Language="C#" Class="Wms" %>

using System;
using System.IO;
using System.Reflection;
using System.Web;
using Cartomatic.Utils.Web;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;

public class Wms : IHttpHandler
{

    IWmsDriver _drv = null;
    
    public void ProcessRequest (HttpContext context)
    {
        var drv = GetWmsDriver();

        //Use proxy utils to get the url that should be called
        var proxiedUrl = context.Request.Url.AbsoluteUri.ExtractProxiedUrl();

        var drvResponse = drv.HandleRequest(proxiedUrl);

        drvResponse.TransferToResponse(context.Response);

        //complete the request
        context.ApplicationInstance.CompleteRequest();
    }

    private IWmsDriver GetWmsDriver()
    {
        if (_drv == null)
        {
            _drv = new Wms2WmtsProxy();
        }

        return _drv;
    }

    public bool IsReusable {
        get {
            return true;
        }
    }

}