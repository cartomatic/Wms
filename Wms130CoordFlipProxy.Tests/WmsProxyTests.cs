using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace WmsProxy.Tests
{
    [TestFixture]
    public class WmsProxyTests
    {
        [Test] public void WmsProxyCoordSwap_WhenCoordSwappingEpsgRequested_SwapsCoords()
        {
            var url = "http://some.url.com?swapcoords=true&version=1.3.0&bbox=1,2,3,4";
            var proxy = GetWmsProxy();

            proxy.SwapBboxCoordsOrder(url).Should().Contain("bbox=2,1,4,3");
        }

        protected Cartomatic.Wms.Wms130CoordFlipProxy GetWmsProxy()
        {
            return new Cartomatic.Wms.Wms130CoordFlipProxy();
        }
    }
}
