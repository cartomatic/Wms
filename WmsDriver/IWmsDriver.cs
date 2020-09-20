using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public interface IWmsDriver
    {
        IWmsServiceDescription ServiceDescription { get; set; }
    
        Task<IWmsDriverResponse> HandleRequestAsync(string url);

        Task<IWmsDriverResponse> HandleRequestAsync(System.Net.HttpWebRequest request);
    }
}
