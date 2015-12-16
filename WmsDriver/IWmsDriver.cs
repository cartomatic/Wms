using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public interface IWmsDriver
    {
        IWmsDriverResponse Handle(string url);

        IWmsDriverResponse Handle(System.Net.HttpWebRequest request);
    }
}
