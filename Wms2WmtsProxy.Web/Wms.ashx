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

    public void ProcessRequest (HttpContext context)
    {
        var drv = new Wms2WmtsProxy();

        //Use proxy utils to get the url that should be called
        var proxiedUrl = context.Request.Url.AbsoluteUri.ExtractProxiedUrl("wmtscapsurl");

        var drvResponse = drv.HandleRequest(proxiedUrl);

        drvResponse.TransferToResponse(context.Response);

        //complete the request
        context.ApplicationInstance.CompleteRequest();
    }

    public bool IsReusable => false;
}