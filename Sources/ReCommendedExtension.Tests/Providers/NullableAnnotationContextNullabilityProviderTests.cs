using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Providers
{
    [TestFixture]
    [TestNetCore30]
    [NullableContext(NullableContextKind.Enable)]
    public sealed class NullableAnnotationContextNullabilityProviderTests : CSharpHighlightingTestBase
    {
        [NotNull]
        readonly HighlightingTypes ignoredHighlightingTypes = new HighlightingTypes();

        protected override string RelativeTestDataPath => @"Providers\NullableAnnotationContextNullability";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => !ignoredHighlightingTypes.Contains(highlighting);

        [Test]
        public void TestField() => DoNamedTest2();

        [Test]
        public void TestProperty() => DoNamedTest2();

        [Test]
        public void TestIndexer() => DoNamedTest2();

        [Test]
        public void TestParameter() => DoNamedTest2();

        [Test]
        public new void TestMethod() => DoNamedTest2();

        [Test]
        public void TestDelegate() => DoNamedTest2();

        [Test]
        public void TestLocalFunction() => DoNamedTest2();

        [Test]
        public void TestCompiledAnnotations() => DoNamedTest2();

        [Test]
        public void TestUnconstrainedGenericTypes() => DoNamedTest2();

        [Test]
        public void TestAttributes() => DoNamedTest2();

        [Test]
        public void TestUnconstrainedGenericClass() => DoNamedTest2();
    }
}