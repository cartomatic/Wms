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


        //in order to properly handle axis flipping crss need to allow for providing a list in the request.
        //at some stage the driver will be aware of more and more crss, but this gives an option to customise things without having to recompile
        var crss = context.Request.Params["coordflippingcrs"]?.Split(',');
        if (crss != null && crss.Length > 0)
        {
            foreach (var crs in crss)
            {
                int epsgCode;
                if (int.TryParse(crs.Substring(crs.LastIndexOf(":")), out epsgCode))
                {
                    drv.AddCoordFlippingSrid(epsgCode);
                }
            }
        }

        var drvResponse = drv.HandleRequest(context.Request.Url.AbsoluteUri);

        drvResponse.TransferToResponse(context.Response);

        //complete the request
        context.ApplicationInstance.CompleteRequest();
    }

    public bool IsReusable => false;
}