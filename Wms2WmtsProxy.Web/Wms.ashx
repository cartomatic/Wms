<%@ WebHandler Language="C#" Class="Wms" %>

using System;
using System.IO;
using System.Reflection;
using System.Web;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;

public class Wms : IHttpHandler
{

    public void ProcessRequest (HttpContext context)
    {
        var drv = new Wms2WmtsProxy();

        var drvResponse = drv.HandleRequest(context.Request.Url.AbsoluteUri);

        drvResponse.TransferToResponse(context.Response);

        //complete the request
        context.ApplicationInstance.CompleteRequest();
    }

    public bool IsReusable => false;
}