using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.Tests.Analyzers.InterfaceImplementation;

[TestFixture]
[TestNet70]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class InterfaceImplementationQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementationQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ImplementEqualityOperatorsSuggestion;

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestEquatableTypes_Availability() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Disable)]
    public void TestEquatableTypes_NonNullable_Availability() => DoNamedTest2();
}