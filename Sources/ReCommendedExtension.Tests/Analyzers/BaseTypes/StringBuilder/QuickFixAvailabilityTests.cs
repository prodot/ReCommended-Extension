using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
[NullableContext(NullableContextKind.Enable)]
[TestNet90]
public sealed class QuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilder\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is PassSingleCharactersSuggestion or UseOtherMethodSuggestion or RedundantMethodInvocationHint || highlighting.IsError();

    [Test]
    public void TestUseOtherMethodFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveMethodInvocationAvailability() => DoNamedTest2();
}