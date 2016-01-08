using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Serialization;
using Cartomatic.Wms;
using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// Extracts the WmsDriver settings for the configured map component. If the settings also contain the wms service description object
        /// it overrides the one supplied in the constructor;
        /// Extracting WmsDriverSettings should only be performed after the map server has been properly constructed, otherwise
        /// null ref exception will occur
        /// </summary>
        protected internal void ExtractWmsDriverSettings()
        {
            //the driver specific settings are kept in a comments component in a form of a json serialised Cartomatic.Manifold.WmsDriverSettings object
            //the comments component is named 'wms.settings'
            
            try
            {
                var c = MapServer.Document.ComponentSet[WmsSettingsComp] as M.Comments;

                if (c == null)
                {
                    throw new Exception(string.Format("Configuration component '{0}' should be a Comments component.", WmsSettingsComp));
                }

                //get a list of settings for each served map
                var wmsSettings = c.Text.DeserializeFromJson<List<WmsDriverSettings>>();

                //and extract the one needed
                MSettings = wmsSettings.Find(wmss => wmss.MapComponent == MapComp);
            }
            catch (Exception ex)
            {
                throw new WmsDriverException("CONFIGURATION ERROR: " + ex.Message);
            }

            if (MSettings == null)
            {
                throw new WmsDriverException(string.Format("CONFIGURATION ERROR: {0} does not specify config for '{1}' map.", WmsSettingsComp, MapComp));
            }

            MergeWmsServiceDescription(MSettings.WmsServiceDescription);
        }


        protected internal void MergeWmsServiceDescription(IWmsServiceDescription wmsServiceDescription)
        {

            //TODO - if driver settings contain wms service description for a map it should get merged into the wms driver description
            //if wms service description provided. otherwise it should replace the wms service description
        }
    }
}
