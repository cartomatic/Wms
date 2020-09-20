using System;
using System.CodeDom;
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
    public class WmsDriver_RequestParams
    {
        [Test]
        public async Task GetParamGeneric_ProperlyExtractsStrings()
        {
            var drv = MakeWmsDriver() ;
            await drv.HandleRequestAsync("http://some.url/?param=string");

            var param = drv.GetParam<string>("param");

            param.Should().Be("string");
        }

        [TestCase("TRUE")]
        [TestCase("true")]
        public async Task GetParamGeneric_ProperlyExtractsBools(string pValue)
        {
            var drv = MakeWmsDriver();
            await drv.HandleRequestAsync(string.Format("http://some.url/?param={0}", pValue));

            var param = drv.GetParam<bool>("param");

            param.Should().Be(true);
        }

        [Test]
        public async Task GetParamGeneric_ProperlyExtractsInts()
        {
            var drv = MakeWmsDriver();
            await drv.HandleRequestAsync("http://some.url/?param=666");

            var param = drv.GetParam<int>("param");

            param.Should().Be(666);
        }

        [Test]
        public async Task GetParamGeneric_ProperlyExtractsDoubles()
        {
            var drv = MakeWmsDriver();
            await drv.HandleRequestAsync("http://some.url/?param=666.666");

            var param = drv.GetParam<double>("param");

            param.Should().Be(666.666);
        }

        [Test]
        public void GetParamGeneric_WhenExtractingParamMoreThanOnce_UsesCache()
        {
            var drv = MakeWmsDriver();
            var date = (DateTime) Convert.ChangeType("2009/12/12", typeof (DateTime));
            
            drv.ExtractedRequestParams = new Dictionary<string, Dictionary<Type, object>>();
            drv.ExtractedRequestParams["param"] = new Dictionary<Type, object>();
            drv.ExtractedRequestParams["param"].Add(date.GetType(), date);

            var param = drv.GetParam<DateTime>("param");

            param.Should().Be(date);
        }

        private Cartomatic.Wms.WmsDriver MakeWmsDriver()
        {
            return new FakeWmsDriver();
        }

        private class FakeWmsDriver : Cartomatic.Wms.WmsDriver
        {
        }
    }
}