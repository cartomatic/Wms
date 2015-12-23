using System;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsBoundingBoxTests
    {
        
        [Test]
        public void Constructor_WhenBbox0123_AssignsPropertiesProperly()
        {
            var bbox = MakeWmsBoundingBox();

            bbox.MinX.Should().Be(0);
            bbox.MinY.Should().Be(1);
            bbox.MaxX.Should().Be(2);
            bbox.MaxY.Should().Be(3);
        }

        [Test]
        public void Width_WhenBbox0123_Is2()
        {
            var bbox = MakeWmsBoundingBox();

            bbox.Width.Should().Be(2);
        }

        [Test]
        public void Height_WhenBbox0123_Is2()
        {
            var bbox = MakeWmsBoundingBox();

            bbox.Height.Should().Be(2);
        }

        [Test]
        public void CenterX_WhenBbox0123_Is1()
        {
            var bbox = MakeWmsBoundingBox();

            bbox.CenterX.Should().Be(1);
        }

        [Test]
        public void CenterY_WhenBbox0123_Is2()
        {
            var bbox = MakeWmsBoundingBox();

            bbox.CenterY.Should().Be(2);
        }

        [Test]
        public void MaxX_WhenLessOrEqualMinX_Throws()
        {
            var bbox = MakeWmsBoundingBox();

            Action act = () => bbox.MaxX = bbox.MinX;

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void MinX_WhenGreaterOrEqualMaxX_Throws()
        {
            var bbox = MakeWmsBoundingBox();

            Action act = () => bbox.MinX = bbox.MaxX;

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void MaxY_WhenLessOrEqualMinY_Throws()
        {
            var bbox = MakeWmsBoundingBox();

            Action act = () => bbox.MaxY = bbox.MinY;

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void MinY_WhenGreaterOrEqualMaxX_Throws()
        {
            var bbox = MakeWmsBoundingBox();

            Action act = () => bbox.MinY = bbox.MaxY;

            act.ShouldThrow<ArgumentException>();
        }

        private IWmsBoundingBox MakeWmsBoundingBox(double minX = 0, double minY = 1, double maxX = 2, double maxY = 3)
        {
            return new WmsBoundingBox(minX, minY, maxX, maxY);
        }
    }
}
