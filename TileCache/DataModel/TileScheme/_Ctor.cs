using System;
using Cartomatic.Utils.Dto;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms.TileCache
{

    public partial class TileScheme
    {
        public TileScheme()
        {
            Precision = 7;
        }

        public TileScheme(string json)
            : this()
        {
            var ts = json.DeserializeFromJson<TileScheme>();
            this.CopyPublicPropertiesFrom(ts);
        }
    }
}
