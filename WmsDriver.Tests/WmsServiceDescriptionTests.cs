using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Serialization;
using Cartomatic.Wms.WmsDriverExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Cartomatic.Wms.WmsDriver.Tests
{
    [TestFixture]
    public class WmsServiceDescriptionTests
    {
        [Test]
        public void Keywords_WhenObjectConstructed_AreInstantiated()
        {
            var sd = MakeWmsServiceDescription();

            sd.Keywords.Should().NotBeNull();
        }

        [Test]
        public void WmsServiceDescriptionFromJson_WhenCalledOnValidJson_ProperlyDeserializesObject()
        {
            var refSd = MakeWmsServiceDescription().ApplyDefaults();
            var json = refSd.SerializeToJson();

            var sd = json.WmsServiceDescriptionFromJson();

            sd.Should().NotBeNull();
            sd.ShouldBeEquivalentTo(refSd);
        }

        [Test]
        public void Merge_WhenPassedAnotherWmsServiceDescription_ShouldMergeBothObjects()
        {
            var sd1 = MakeWmsServiceDescription();
            var sd2 = MakeWmsServiceDescription().ApplyDefaults();

            sd1.Merge(sd2);

            sd1.ShouldBeEquivalentTo(sd2);
        }

        [Test]
        public void Merge_WhenPropertiesToBeConcatenatedAreSpecifiedAsIEnumerable_ShouldCombineValuesOfStringProperties()
        {
            var sd1 = MakeWmsServiceDescription().ApplyDefaults();
            var sd2 = MakeWmsServiceDescription().ApplyDefaults();

            sd1.Merge(sd2, new List<string>() { "Title" });

            sd1.Title.Should().Be(string.Concat(sd2.Title, " ", sd2.Title));
        }

        [Test]
        public void Merge_WhenPropertiesToBeConcatenatedAreSpecifiedAsParams_ShouldCombineValuesOfStringProperties()
        {
            var sd1 = MakeWmsServiceDescription().ApplyDefaults();
            var sd2 = MakeWmsServiceDescription().ApplyDefaults();

            sd1.Merge(sd2, "Title", "Abstract");

            sd1.Title.Should().Be(string.Concat(sd2.Title, " ", sd2.Title));
            sd1.Abstract.Should().Be(string.Concat(sd2.Abstract, " ", sd2.Abstract));
        }

        private IWmsServiceDescription MakeWmsServiceDescription()
        {
            return new WmsServiceDescription();
        }
    }
}
