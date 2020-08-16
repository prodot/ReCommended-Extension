using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [TestFixture]
    public sealed class AnnotateWithInstantHandleAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithInstantHandle>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        public void TestAvailabilityCS80() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
        public void TestAvailabilityCS90() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        public void TestAvailabilityAsyncEnumerableCS80() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        public void TestAvailabilityAsyncEnumerableCS90() => DoNamedTest2();
    }
}