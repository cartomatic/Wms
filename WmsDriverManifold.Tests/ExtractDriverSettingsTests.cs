using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;
using WmsDriver = Cartomatic.Manifold.WmsDriver;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class ExtractDriverSettingsTests
    {
        [Test]
        public void ExtractDriverSettings_WhenSettingsCmpNotPresent_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            drv.WmsSettingsComp = "NotPresent"; //override the settings cmp so it is not present

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ExtractDriverSettings_WhenSettingsCmpNotOfCommentsType_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            drv.WmsSettingsComp = "NaturalEarth"; //override the settings cmp so it becomes invalid
            drv.CreateMapServer();

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ExtractDriverSettings_WhenSettingsCmpOk_ShouldDeserialiseTheObjectProperly()
        {
            var drv = MakeWmsDriver();
            drv.CreateMapServer();

            drv.ExtractWmsDriverSettings();

            drv.MSettings.Should().NotBeNull();
        }


        [Test]
        public void ExtractDriverSettings_WhenSettingsCmpOkButNoSettingsForServedMap_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            drv.MapComp = "MapWithNoWmsSettingsProvided";
            drv.CreateMapServer();

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void MergeWmsServiceDescription_IfNoServiceDescriptionProvidedInTheMapSettings_NothingHappens()
        {
            var drv1 = MakeWmsDriver();
            drv1.ServiceDescription = null;
            var drv2 = MakeWmsDriver();
            drv2.ServiceDescription = new WmsServiceDescription();
            
            drv1.MergeWmsServiceDescription(null);
            drv2.MergeWmsServiceDescription(null);

            drv1.ServiceDescription.Should().BeNull();
            drv2.ServiceDescription.Should().NotBeNull();
        }



        private WmsDriver MakeWmsDriver()
        {
            return new WmsDriver(Utils.GetMapFilePath());
        }

    }
}
