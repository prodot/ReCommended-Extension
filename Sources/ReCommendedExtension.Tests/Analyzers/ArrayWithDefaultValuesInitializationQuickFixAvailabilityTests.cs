using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitializationQuickFixes";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is ArrayWithDefaultValuesInitializationSuggestion;

        [Test]
        public void TestArrayWithDefaultValuesInitializationAvailability() => DoNamedTest2();

        [Test]
        [NullableContext(NullableContextKind.Enable)]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        public void TestArrayWithDefaultValuesInitializationAvailabilityWithNullableAnnotations() => DoNamedTest2();
    }
}