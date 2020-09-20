using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    public class WmsDriver_ExceptionTests
    {
        
        [Test]
        public async Task Handle_WhenFailingWithWmsDriverException_ProperlyHandlesWmsResponse()
        {
            var drv = MakeWmsDriver() as FakeWmsDriverFailsWithWmsDriverException;

            var response = await drv.HandleRequestAsync("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(drv.WmsExceptionCode);
            response.WmsDriverException.Message.Should().Be(drv.Message);

            //Warning - this will fail when the exception handler stops defaulting to xml!
            //also need to verify the xml!
            response.ResponseContentType.Should().Be("text/xml");
            response.ResponseText.Should().Contain(drv.Message);
            response.ResponseText.Should().Contain(drv.WmsExceptionCode.ToString());
        }

        [Test]
        public async Task Handle_WhenFailingWithException_ProperlyHandlesWmsResponse()
        {
            var drv = MakeWmsDriver(stdEx: true) as FakeWmsDriverFailsWithException;

            var response = await drv.HandleRequestAsync("http://some.url/");

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(drv.Message);

            //Warning - this will fail when the exception handler stops defaulting to xml!
            //also need to verify the xml!
            response.ResponseContentType.Should().Be("text/xml");
            response.ResponseText.Should().Contain(drv.Message);
            response.ResponseText.Should().Contain(WmsExceptionCode.NotApplicable.ToString());
        }





        private IWmsDriver MakeWmsDriver(bool stdEx = false)
        {
            if (stdEx)
                return new FakeWmsDriverFailsWithException();
            else
                return new FakeWmsDriverFailsWithWmsDriverException();
        }

        private class FakeWmsDriverFailsWithWmsDriverException : Cartomatic.Wms.WmsDriver
        {
            public WmsExceptionCode WmsExceptionCode = WmsExceptionCode.NotApplicable;
            public string Message = "test wms driver exception";
            protected internal override void PrepareDriver()
            {
                base.PrepareDriver();
                throw new WmsDriverException(Message, WmsExceptionCode);
            }
        }

        private class FakeWmsDriverFailsWithException : Cartomatic.Wms.WmsDriver
        {
            public string Message = "test standard exception";

            protected internal override void PrepareDriver()
            {
                base.PrepareDriver();
                throw new Exception(Message);
            }
        }
    }
}