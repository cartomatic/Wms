using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public interface IWmsDriver
    {
        /// <summary>
        /// Service description handle
        /// </summary>
        IWmsServiceDescription ServiceDescription { get; set; }
    
        /// <summary>
        /// Handles WMS request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<IWmsDriverResponse> HandleRequestAsync(string url);

        /// <summary>
        /// Handles WMS request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IWmsDriverResponse> HandleRequestAsync(System.Net.HttpWebRequest request);

        /// <summary>
        /// Generates a WMS exception response; useful for handling exception in 'WMS style'
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        IWmsDriverResponse GenerateExceptionResponse(Exception ex);

        /// <summary>
        /// Generates a WMS exception response; useful for handling exception in 'WMS style'
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        IWmsDriverResponse GenerateExceptionResponse(string msg);
    }
}
