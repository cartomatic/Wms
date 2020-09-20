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
    public class WmsDriver_BasicTests
    {
        [Test]
        public async Task Handle_Always_CallsPrepareDriver()
        {
            var drv = MakeWmsDriver() ;

            await drv.HandleRequestAsync("http://some.url/");

            (drv as FakeWmsDriver).PrepareCalled.Should().BeTrue();
        }

        [Test]
        public async Task Handle_WhenServiceDescriptionNotProvided_CreatesDefaultWmsServiceDescription()
        {
            var drv = MakeWmsDriver();

            drv.ServiceDescription.Should().BeNull();

            await drv.HandleRequestAsync("http://some.url/");
            
            drv.ServiceDescription.Should().NotBeNull();
        }

        private IWmsDriver MakeWmsDriver()
        {
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
    }
}