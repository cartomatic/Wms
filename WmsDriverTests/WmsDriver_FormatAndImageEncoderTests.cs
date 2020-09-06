using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    class WmsDriver_FormatAndImageEncoderTests
    {
        [TestCase("png", "image/png")]
        [TestCase("image/png", "image/png")]
        [TestCase("jpeg", "image/jpeg")]
        [TestCase("image/jpeg", "image/jpeg")]
        [TestCase("gif", "image/gif")]
        [TestCase("image/gif", "image/gif")]
        public void GetEncoderInfor_ReturnsProperMime(string format, string expectedMime)
        {
            var drv = MakeWmsDriver();

            var codecInfo = drv.GetEncoderInfo(format);

            codecInfo.MimeType.Should().Be(expectedMime);
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
