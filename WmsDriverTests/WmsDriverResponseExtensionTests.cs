using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Wms.WmsDriverExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriverTests
{
    [TestFixture]
    public class WmsDriverResponseExtensionsTests
    {
        [Test]
        public void HasData_WhenBinaryDataPresent_ShouldReturnTrue()
        {
            var r = MakeWmsDriverResponse();
            r.ResponseBinary = new byte[] {};

            r.HasData().Should().BeTrue();
        }

        [Test]
        public void HasData_WhenNoBinaryDataPresent_ShouldReturnFalse()
        {
            var r = MakeWmsDriverResponse();

            r.HasData().Should().BeFalse();
        }

        private IWmsDriverResponse MakeWmsDriverResponse()
        {
            return new WmsDriverResponse();
        }
 
    }
}
