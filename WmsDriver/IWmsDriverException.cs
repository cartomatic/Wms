namespace Cartomatic.Wms
{
    public interface IWmsDriverException
    {
        string Message { get; }

        WmsExceptionCode WmsExceptionCode { get; set; }
    }
}