using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.Results;

namespace Cartomatic.Wms.ManifoldWmsDriverWebApi.Controllers
{
    public class BaseController : ApiController
    {
        /// <summary>
        /// Handles exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logException"></param>
        /// <returns></returns>
        protected virtual IHttpActionResult HandleException(Exception ex, bool logException = false)
        {
            LogException(ex);

            //by default assume internal server err and just a msg
            object obj = new { ErrorMessage = ex.Message };
            var status = HttpStatusCode.InternalServerError;
            
            return HandleException(status, obj);
        }

        /// <summary>
        /// handles exception returns in a unified way for all the apis
        /// </summary>
        /// <param name="status"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected virtual IHttpActionResult HandleException(HttpStatusCode status, object o)
        {
            return new NegotiatedContentResult<object>(status, o, this);
        }

        /// <summary>
        /// Logs an exception to a file in err_logs directory of app domain base dir
        /// </summary>
        /// <param name="ex"></param>

        protected virtual void LogException(Exception ex)
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "err_logs");
            var logFile = Path.Combine(logDir, $"{DateTime.Now:yyyyMMdd}.log");

            try
            {
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                while (ex != null)
                {
                    File.AppendAllLines(logFile, new[]
                    {
                        $"{DateTime.Now.ToLongDateString()} :: {DateTime.Now.ToLongTimeString()}",
                        ex.Message,
                        ex.StackTrace,
                        new string('-', 30),
                        Environment.NewLine
                    });

                    ex = ex.InnerException;
                }

            }
            catch
            {
                //ignore
            }
        }
    }
}
