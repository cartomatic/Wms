using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    class WmsDriver_BoundingBoxTests
    {
        [TestCase("1.1.1", null)]
        [TestCase("1.1.0", 4326)]
        [TestCase("1.0.0", 2180)]
        public void GetCoordFlip_ForVersionLessThan130_AlwaysReturnsFalse(string version, int? srid)
        {
            var drv = MakeWmsDriver();
            var flip = WmsDriver.GetCoordFlip(version, srid);

            flip.Should().BeFalse();
        }

        [TestCase(2180)]
        [TestCase(4326)]
        public void GetCoordFlip_ForVersion130AndCoordFlipingSrids_ReturnsTrue(int? srid)
        {
            var drv = MakeWmsDriver();
            var flip = WmsDriver.GetCoordFlip("1.3.0", srid);

            flip.Should().BeTrue();
        }

        [Test]
        public void AddCoordFlippingSrid_WhenCoordSysAdded_AdjustsGetCoordFlipOutput()
        {
            var drv = MakeWmsDriver();
            var srid = 666;

            var flipBeforeAdd = WmsDriver.GetCoordFlip("1.3.0", srid);
            WmsDriver.RegisterCoordFlippingSrid(666);
            var flipAfterAdd = WmsDriver.GetCoordFlip("1.3.0", srid);
            WmsDriver.UnregisterCoordFlippingSrid(666);
            var flipAfterRemove = WmsDriver.GetCoordFlip("1.3.0", srid);


            flipBeforeAdd.Should().BeFalse();
            flipAfterAdd.Should().BeTrue();
            flipAfterRemove.Should().BeFalse();
        }

        [Test]
        public void ParsBbox_ForVersionLessThan130_ParsesBboxProperly()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(0,0,2,2);
            
            var bbox = WmsDriver.ParseBBOX("0,0,2,2", "1.1.1", (string)null);

            bbox.Should().BeEquivalentTo(bbox);
        }

        [Test]
        public void ParsBbox_ForVersion130_ParsesBboxProperlyWithCoordFlip()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(1, 0, 3, 2);

            var bbox = WmsDriver.ParseBBOX("0,1,2,3", "1.3.0", "epsg:2180");

            bbox.Should().BeEquivalentTo(refBbox);
        }

        [Test]
        public void ParsBbox_ForVersion130_ParsesBboxProperlyWithoutCoordFlip()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(0, 0, 2, 2);

            var bbox = WmsDriver.ParseBBOX("0,0,2,2", "1.3.0", (int?)null);

            bbox.Should().BeEquivalentTo(refBbox);
        }

        [Test]
        public void ParseBbox_ForVersionLessThan130BboxPartsInWrongOrder_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("3,3,2,2", "1.1.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParseBbox_ForVersion130BboxPartsInWrongOrderWithNoFlippingEpsg_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("3,3,2,2", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParseBbox_ForVersion130BboxPartsInWrongOrderWithFlippingEpsg_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("5,6,3,4", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueWithTruncatedBboxParts_Should_Throw()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("5,3,4", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMinX_Should_Throw()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("x,0,1,1", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMinY_Should_Throw()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("0,x,1,1", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMaxX_Should_Throw()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("0,0,x,1", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMaxY_Should_Throw()
        {
            var drv = MakeWmsDriver();
            Action a = () => WmsDriver.ParseBBOX("0,0,1,x", "1.3.0", (int?)null);

            a.Should().Throw<WmsDriverException>();
        }


        private FakeWmsDriver MakeWmsDriver()
        {
            return new FakeWmsDriver();
        }

        class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
        }
    }
}
