using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriverManifold.Tests
{
    [TestFixture]
    public class CreateMapServerTests
    {
        [Test]
        [Category("ManifoldMapFileDependant")]
        public void CreateMapServer_WhenMapFileNotSpecified_Should_Throw()
        {
            var drv = new ManifoldWmsDriver(null);

            Action a = () => { drv.CreateMapServer(); };
            
            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleRequest_WhenSpecifiedMapComponentIsNotMap_Should_Throw()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new ManifoldWmsDriver(mapFile, "NE2_50M_SR_W");

            Action a = () => { drv.CreateMapServer(); };

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleRequest_WhenSpecifiedMapComponentIsNotAllowedByMapServer_Should_Throw()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new ManifoldWmsDriver(mapFile, "NaturalEarth");

            Action a = () => { drv.CreateMapServer(); };

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        [Category("ManifoldMapFileDependant")]
        public void HandleRequest_WhenSpecifiedMapComponentDoesNotExist_Should_Throw()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new ManifoldWmsDriver(mapFile, "XXX");

            Action a = () => { drv.CreateMapServer(); };

            a.Should().Throw<WmsDriverException>();
        }
    }
}
