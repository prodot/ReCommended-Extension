using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.ParameterNameHints;
using JetBrains.ReSharper.Feature.Services.TypeNameHints;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Providers
{
    [TestFixture]
    [TestNetCore30]
    [NullableContext(NullableContextKind.Enable)]
    public sealed class NullableSubtypeAnnotationContextNullabilityProviderTests : CSharpHighlightingTestBase
    {
        [NotNull]
        readonly HighlightingTypes ignoredHighlightingTypes = new HighlightingTypes();

        public NullableSubtypeAnnotationContextNullabilityProviderTests()
        {
            ignoredHighlightingTypes.Add<TypeNameHintHighlighting>();
            ignoredHighlightingTypes.Add<TypeNameHintContextActionHighlighting>();
            ignoredHighlightingTypes.Add<UnassignedFieldCompilerWarning>();
            ignoredHighlightingTypes.Add<UnusedVariableWarning>();
            ignoredHighlightingTypes.Add<CollectionNeverUpdatedLocalWarning>();
            ignoredHighlightingTypes.Add<ParameterNameHintHighlighting>();
            ignoredHighlightingTypes.Add<ParameterNameHintContextActionHighlighting>();
        }

        protected override string RelativeTestDataPath => @"Providers\NullableSubtypeAnnotationContextNullability";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => !ignoredHighlightingTypes.Contains(highlighting);

        [Test]
        public void TestFieldEnumerable() => DoNamedTest2();

        [Test]
        public void TestFieldCollection() => DoNamedTest2();

        [Test]
        public void TestFieldArray() => DoNamedTest2();

        [Test]
        public void TestFieldTask() => DoNamedTest2();

        [Test]
        public void TestFieldValueTask() => DoNamedTest2();

        [Test]
        public void TestFieldLazy() => DoNamedTest2();

        [Test]
        public new void TestMethod() => DoNamedTest2();
    }
}