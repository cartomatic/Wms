using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected virtual async Task<IWmsDriverResponse> HandleGetMapAsync()
        {
            Validate(HandleGetMapValidationRules);

            return await HandleGetMapDriverSpecificAsync();
        }

        protected virtual async Task<IWmsDriverResponse> HandleGetMapDriverSpecificAsync()
        {
            throw new WmsDriverException(string.Format("IMPLEMENTATION ERROR: GetMap is a mandatory operation for WMS {0}.", GetParam<string>("version")));
        }
    }
}
