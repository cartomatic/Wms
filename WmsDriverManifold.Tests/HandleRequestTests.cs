using System;
using System.Collections.Generic;
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
    public class HandleRequestTests
    {
        [Test]
        public void CreateMapServer_WhenMapFileNotSpecified_ShouldThrow()
        {
            var drv = new WmsDriver(null);

            Action a = () => { drv.CreateMapServer(); };
            
            a.ShouldThrow<WmsDriverException>();
        }

        //[Test]
        //public void HandleRequest_WhenSpecifiedMapComponentIsNotMap_ShouldThrow()
        //{
        //    var drv = new WmsDriver(null);

        //    Action a = () => { drv.HandleRequest("http://some.url/"); };

        //    a.ShouldThrow<WmsDriverException>();
        //}
    }
}
