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

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriverTests
    {
        [Test]
        public void Constructor_WhenCalled_ShouldAlwaysInitDataContainers()
        {
            var drv = MakeWmsDriver() as FakeWmsDriver;

            drv.SupportedGetFeatureInfoFormats.Should().NotBeNull();
            drv.SupportedGetCapabilitiesFormats.Should().NotBeNull();
            drv.SupportedGetMapFormats.Should().NotBeNull();
            drv.SupportedExceptionFormats.Should().NotBeNull();
            drv.SupportedVersions.Should().NotBeNull();
        }

        [Test]
        public void Handle_Always_CallsPrepareDriver()
        {
            var drv = MakeWmsDriver() ;

            drv.Handle(string.Empty);

            (drv as FakeWmsDriver).PrepareCalled.Should().BeTrue();
        }

        [Test]
        public void Handle_WhenServiceDescriptionNotProvided_CreatesDefaultWmsServiceDescription()
        {
            var drv = MakeWmsDriver();

            drv.ServiceDescription.Should().BeNull();

            drv.Handle(string.Empty);
            
            drv.ServiceDescription.Should().NotBeNull();
        }

        [Test]
        public void Handle_WhenFailingWithWmsDriverException_ProperlyHandlesWmsResponse()
        {
            var drv = MakeWmsDriver(wmsEx: true) as FakeWmsDriverFailsWithWmsDriverException;

            var response = drv.Handle(string.Empty);

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
        public void Handle_WhenFailingWithException_ProperlyHandlesWmsResponse()
        {
            var drv = MakeWmsDriver(stdEx: true) as FakeWmsDriverFailsWithException;

            var response = drv.Handle(string.Empty);

            response.WmsDriverException.Should().NotBeNull();
            response.WmsDriverException.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            response.WmsDriverException.Message.Should().Be(drv.Message);

            //Warning - this will fail when the exception handler stops defaulting to xml!
            //also need to verify the xml!
            response.ResponseContentType.Should().Be("text/xml");
            response.ResponseText.Should().Contain(drv.Message);
            response.ResponseText.Should().Contain(WmsExceptionCode.NotApplicable.ToString());
        }





        private IWmsDriver MakeWmsDriver(bool wmsEx = false, bool stdEx = false)
        {
            if(wmsEx)
                return new FakeWmsDriverFailsWithWmsDriverException();

            if(stdEx)
                return new FakeWmsDriverFailsWithException();

            return new FakeWmsDriver();
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
            public bool PrepareCalled { get; private set; }
            
            protected internal override void PrepareDriver()
            {
                PrepareCalled = true;
                base.PrepareDriver();
            }
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