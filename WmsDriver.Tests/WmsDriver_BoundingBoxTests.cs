using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
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
            var flip = drv.GetCoordFlip(version, srid);

            flip.Should().BeFalse();
        }

        [TestCase(2180)]
        [TestCase(4326)]
        public void GetCoordFlip_ForVersion130AndCoordFlipingSrids_ReturnsTrue(int? srid)
        {
            var drv = MakeWmsDriver();
            var flip = drv.GetCoordFlip("1.3.0", srid);

            flip.Should().BeTrue();
        }

        [Test]
        public void ParsBbox_ForVersionLessThan130_ParsesBboxProperly()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(0,0,2,2);
            
            var bbox = drv.ParseBBOX("0,0,2,2", "1.1.1", (string)null);

            bbox.ShouldBeEquivalentTo(bbox);
        }

        [Test]
        public void ParsBbox_ForVersion130_ParsesBboxProperlyWithCoordFlip()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(1, 0, 3, 2);

            var bbox = drv.ParseBBOX("0,1,2,3", "1.3.0", "epsg:2180");

            bbox.ShouldBeEquivalentTo(refBbox);
        }

        [Test]
        public void ParsBbox_ForVersion130_ParsesBboxProperlyWithoutCoordFlip()
        {
            var drv = MakeWmsDriver();
            var refBbox = new WmsBoundingBox(0, 0, 2, 2);

            var bbox = drv.ParseBBOX("0,0,2,2", "1.3.0", (int?)null);

            bbox.ShouldBeEquivalentTo(refBbox);
        }

        [Test]
        public void ParseBbox_ForVersionLessThan130BboxPartsInWrongOrder_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("3,3,2,2", "1.1.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParseBbox_ForVersion130BboxPartsInWrongOrderWithNoFlippingEpsg_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("3,3,2,2", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParseBbox_ForVersion130BboxPartsInWrongOrderWithFlippingEpsg_ThrowsWmsDriverException()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("5,6,3,4", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueWithTruncatedBboxParts_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("5,3,4", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMinX_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("x,0,1,1", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMinY_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("0,x,1,1", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMaxX_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("0,0,x,1", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
        }

        [Test]
        public void ParsBbox_InvalidParamValueBboxMaxY_ShouldThrow()
        {
            var drv = MakeWmsDriver();
            Action a = () => drv.ParseBBOX("0,0,1,x", "1.3.0", (int?)null);

            a.ShouldThrow<WmsDriverException>();
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
