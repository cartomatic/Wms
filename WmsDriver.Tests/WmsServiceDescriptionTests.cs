using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriver.Tests
{
    [TestFixture]
    public class WmsServiceDescriptionTests
    {
        [Test]
        public void Keywords_WhenObjectCOnstructed_AreInstantiated()
        {
            var sd = MakeWmsServiceDescription();

            sd.Keywords.Should().NotBeNull();
        }

        private IWmsServiceDescription MakeWmsServiceDescription()
        {
            return new WmsServiceDescription();
        }
    }
}
