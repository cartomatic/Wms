using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;
using FluentAssertions;
using NUnit.Framework;

namespace WmsDriver.Tests
{
    [TestFixture]
    public class WmsDriverExceptionTests
    {
        [Test]
        public void Constructor_Default_InstantiatesObjectProperly()
        {
            var e = new WmsDriverException();

            e.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            e.Message.Should().Be("Unknown exception.");
        }

        [Test]
        public void Constructor_WhenInvokedWithMessageOnly_ShouldHaveSpecifiedMessagegAndNotApplicableWmsExceptionCode()
        {
            var msg = "Test message.";

            var e = new WmsDriverException(msg);

            e.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            e.Message.Should().Be(msg);
        }

        [Test]
        public void Constructor_WhenInvokedWithMessageAndExceptionCode_ShouldHaveSpecifiedMessageAndWmsExceptionCode()
        {
            var msg = "Test message.";
            var ec = WmsExceptionCode.InvalidCRS;

            var e = new WmsDriverException(msg, ec);

            e.WmsExceptionCode.Should().Be(ec);
            e.Message.Should().Be(msg);
        }

        [Test]
        public void Constructor_WhenInvokedWithMsgAndInnerException_ShouldHaveSpecifiedMsgAndInnerExceptionAndNotApplicableWmsExceptionCode()
        {
            var msg = "Test message.";
            var inex = new Exception("inex");

            var e = new WmsDriverException(msg, inex);

            e.WmsExceptionCode.Should().Be(WmsExceptionCode.NotApplicable);
            e.Message.Should().Be(msg);
            e.InnerException.Should().BeSameAs(inex);
        }

        [Test]
        public void Constructor_WhenInvokedWithMsgWmsExceptionCodeAndInnerException_ShouldHaveSpecifiedMsgWmsExceptionCodeAndInnerException()
        {
            var msg = "Test message.";
            var ec = WmsExceptionCode.InvalidFormat;
            var inex = new Exception("inex");

            var e = new WmsDriverException(msg, ec, inex);

            e.WmsExceptionCode.Should().Be(ec);
            e.Message.Should().Be(msg);
            e.InnerException.Should().BeSameAs(inex);
        }
    }
}
