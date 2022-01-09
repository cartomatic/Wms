using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding.Binders;

namespace Cartomatic.Wms.ManifoldWmsDriverWebApi.Controllers
{
    /// <summary>
    /// alive controller
    /// </summary>
    [RoutePrefix("alive")]
    public class AliveController : BaseController
    {
        /// <summary>
        /// simple api alive tester
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Get()
        {
            return Ok("ALIVE DUDE!");
        }

        
    }
}
