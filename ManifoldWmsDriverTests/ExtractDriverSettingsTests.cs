using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class ExtractDriverSettingsTests
    {
        [Test]
        [Category("ManifoldMapFileDependant")]
        public void ExtractDriverSettings_WhenSettingsCmpNotPresent_Should_Throw()
        {
            var drv = MakeWmsDriver();
            drv.WmsSettingsComp = "NotPresent"; //override the settings cmp so it is not present
            drv.CreateMapServer();

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void ExtractDriverSettings_WhenSettingsCmpNotOfCommentsType_Should_Throw()
        {
            var drv = MakeWmsDriver();
            drv.WmsSettingsComp = "NaturalEarth"; //override the settings cmp so it becomes invalid
            drv.CreateMapServer();

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void ExtractDriverSettings_WhenSettingsCmpOk_ShouldDeserialiseTheObjectProperly()
        {
            var drv = MakeWmsDriver();
            drv.CreateMapServer();

            drv.ExtractWmsDriverSettings();

            drv.MSettings.Should().NotBeNull();
        }


        [Test]
        [Category("ManifoldMapFileDependant")]
        public void ExtractDriverSettings_WhenSettingsCmpOkButNoSettingsForServedMap_Should_Throw()
        {
            var drv = MakeWmsDriver();
            drv.MapComp = "MapWithNoWmsSettingsProvided";
            drv.CreateMapServer();

            Action a = () => { drv.ExtractWmsDriverSettings(); };

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
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


        [Test]
        [Category("ManifoldMapFileDependant")]
        public void MergeWmsServiceDescription_IfExternalServiceDescriptionIsProvidedAndMapSettingsServiceDescriptionIsProvided_ShouldMergeMapFileSercideDesriptionIntoTheExternalOne()
        {
            var drv = MakeWmsDriver();
            drv.ServiceDescription = new WmsServiceDescription();
            drv.ServiceDescription.Title = "Title before merge";
            drv.ServiceDescription.Abstract = "Abstract before merge";
            var keyWordsCount = drv.ServiceDescription.Keywords.Count;

            drv.CreateMapServer();
            drv.ExtractWmsDriverSettings();

            drv.ServiceDescription.Title.Should().Be("Natural Earth 50m");
            drv.ServiceDescription.Abstract.Should().Be("Natural Earth 50m");
            drv.ServiceDescription.Keywords.Count.Should().Be(keyWordsCount + 1);
        }

        
        [Test]
        [Category("ManifoldMapFileDependant")]
        public void MergeWmsServiceDescription_IfExternalServiceDescriptionIsNotProvidedAndMapSettingsServiceDescriptionIsProvided_ShouldMakeTheMapFileServiceDescriptionTheMainOne()
        {
            var drv = MakeWmsDriver();

            drv.CreateMapServer();
            drv.ExtractWmsDriverSettings();

            drv.ServiceDescription.Title.Should().Be("Natural Earth 50m");
            drv.ServiceDescription.Abstract.Should().Be("Natural Earth 50m");
            drv.ServiceDescription.Keywords.Count.Should().Be(1);
        }

        private ManifoldWmsDriver MakeWmsDriver()
        {
            return new ManifoldWmsDriver(Utils.GetMapFilePath());
        }
    }
}
