using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitialization";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is ArrayWithDefaultValuesInitializationSuggestion;

        [Test]
        public void TestArrayWithDefaultValuesInitialization() => DoNamedTest2();

        [Test]
        [NullableContext(NullableContextKind.Enable)]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        public void TestArrayWithDefaultValuesInitializationWithNullableAnnotations() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
        public void TestArrayWithDefaultValuesInitialization_TargetTyped() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public void TestArrayWithDefaultValuesInitialization_ParameterlessCtor() => DoNamedTest2();
    }
}