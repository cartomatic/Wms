using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    /// <summary>
    /// WMS Exceeption codes as per Table E.1 — Service exception codes of WMS 1.3.0 spec
    /// </summary>
    public enum WmsExceptionCode
    {
        /// <summary>
        /// Request contains a Format not offered by the server.
        /// </summary>
        InvalidFormat,

        /// <summary>
        /// Request contains a CRS not offered by the server for one or more of the
        /// Layers in the Request.
        /// </summary>
        InvalidCRS,

        /// <summary>
        /// GetMap Request is for a Layer not offered by the server, or GetFeatureInfo
        /// Request is for a Layer not shown on the map.
        /// </summary>
        LayerNotDefined,

        /// <summary>
        /// Request is for a Layer in a Style not offered by the server.
        /// </summary>
        StyleNotDefined,

        /// <summary>
        /// GetFeatureInfo Request is applied to a Layer which is not declared queryable.
        /// </summary>
        LayerNotQueryable,

        /// <summary>
        /// GetFeatureInfo Request contains invalid X or Y value.
        /// </summary>
        InvalidPoint,

        /// <summary>
        /// Value of (optional) UpdateSequence parameter in GetCapabilities Request is
        /// equal to current value of service metadata update sequence number.
        /// </summary>
        CurrentUpdateSequence,

        /// <summary>
        /// Value of (optional) UpdateSequence parameter in GetCapabilities Request is
        /// greater than current value of service metadata update sequence number.
        /// </summary>
        InvalidUpdateSequence,

        /// <summary>
        /// Request does not include a sample dimension value, and the server did not
        /// declare a default value for that dimension.
        /// </summary>
        MissingDimensionValue,

        /// <summary>
        /// Request contains an invalid sample dimension value.
        /// </summary>
        InvalidDimensionValue,

        /// <summary>
        /// Request is for an optional operation that is not supported by the server.
        /// </summary>
        OperationNotSupported,

        /// <summary>
        /// No error code
        /// </summary>
        NotApplicable
    }
}
