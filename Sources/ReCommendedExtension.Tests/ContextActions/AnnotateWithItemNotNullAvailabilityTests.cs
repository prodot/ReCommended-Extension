using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithItemNotNullAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithItemNotNull>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
        [TestNetFramework45]
        [TestPackagesWithAnnotations("System.Threading.Tasks.Extensions")]
        public void TestAvailability() => DoNamedTest2();

        [Test]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        [NullableContext(NullableContextKind.Enable)]
        public void TestAvailabilityNullableAnnotationContext() => DoNamedTest2();
    }
}