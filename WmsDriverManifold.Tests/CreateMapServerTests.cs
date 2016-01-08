using System;
using System.Collections.Generic;
using System.IO;
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
    public class CreateMapServerTests
    {
        [Test]
        public void CreateMapServer_WhenMapFileNotSpecified_ShouldThrow()
        {
            var drv = new WmsDriver(null);

            Action a = () => { drv.CreateMapServer(); };
            
            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenSpecifiedMapComponentIsNotMap_ShouldThrow()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new WmsDriver(mapFile, "NE2_50M_SR_W");

            Action a = () => { drv.CreateMapServer(); };

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenSpecifiedMapComponentIsNotAllowedByMapServer_ShouldThrow()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new WmsDriver(mapFile, "NaturalEarth");

            Action a = () => { drv.CreateMapServer(); };

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void HandleRequest_WhenSpecifiedMapComponentDoesNotExist_ShouldThrow()
        {
            var mapFile = Utils.GetMapFilePath();
            var drv = new WmsDriver(mapFile, "XXX");

            Action a = () => { drv.CreateMapServer(); };

            a.ShouldThrow<WmsDriverException>();
        }
    }
}
