using System;
using System.Collections.Generic;
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
    public class WmsServiceDescriptionExtensionsTests
    {
        [Test]
        public void ApplyDefaults_ReturnsAnInstanceOfObject()
        {
            var sd = MakeWmsServiceDescription();

            sd.ApplyDefaults();

            //it does not really matter what sort of default data is applied, so just making sure and instance is returned
            sd.Should().NotBeNull();
        }

        private IWmsServiceDescription MakeWmsServiceDescription()
        {
            return new WmsServiceDescription();
        }
    }
}
