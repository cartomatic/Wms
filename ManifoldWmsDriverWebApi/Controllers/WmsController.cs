using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding.Binders;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;
using Newtonsoft.Json;

namespace Cartomatic.Wms.ManifoldWmsDriverWebApi.Controllers
{
    /// <summary>
    /// WMS
    /// </summary>
    [RoutePrefix("wms")]
    public class WmsController : BaseController
    {
        private static IWmsDriver _dummyDrv = null;
        private IWmsDriver _drv = null;

        static WmsController()
        {
            _dummyDrv = new ManifoldWmsDriver("dummy");
        }

        /// <summary>
        /// returns manifold version
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Get()
        {
            HttpStatusCode statusCode;
            IWmsDriverResponse drvResponse;
            try
            {
                var wmsDrv = GetWmsDriver();
                drvResponse = await wmsDrv.HandleRequestAsync(HttpContext.Current.Request.Url.AbsoluteUri);

                statusCode = drvResponse.WmsDriverException == null
                    ? HttpStatusCode.OK
                    : drvResponse.WmsDriverException.WmsExceptionCode == WmsExceptionCode.NotApplicable
                        ? HttpStatusCode.InternalServerError
                        : HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                LogException(ex);
                drvResponse = _dummyDrv.GenerateExceptionResponse(ex);
                statusCode = HttpStatusCode.InternalServerError;
            }

            //write directly to response
            drvResponse.TransferToResponse(HttpContext.Current.Response);

            //handle response codes appropriately
            return statusCode switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.BadRequest => BadRequest(),
                _ => InternalServerError()
            };
        }

        private string MapName => Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "map").Value;

        /// <summary>
        /// Gets a wms driver to process a request
        /// </summary>
        /// <returns></returns>
        private IWmsDriver GetWmsDriver()
        {
            if (_drv == null)
            {
                var mapFile = Path.Combine(
                    HttpContext.Current.Server.MapPath(HttpContext.Current.Request.Path),
                    "../../ManifoldWmsDriver/TestData/TestData.map"
                );
                _drv = new ManifoldWmsDriver(mapFile);
            }

            return _drv;
        }
    }
}
