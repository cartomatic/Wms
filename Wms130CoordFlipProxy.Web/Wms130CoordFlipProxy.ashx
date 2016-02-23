<%@ WebHandler Language="C#" Class="WmsProxy" %>

using System;
using System.IO;
using System.Reflection;
using System.Web;

public class WmsProxy : IHttpHandler
{
    public void ProcessRequest (HttpContext context)
    {
        new Cartomatic.Wms.Wms130CoordFlipProxy().HandleRequest(context);
    }

    public bool IsReusable {
        get {
            return true;
        }
    }
}