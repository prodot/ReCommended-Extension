﻿using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.StringBuilder;

[TestFixture]
public sealed class UseStringBuilderQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\StringBuilderQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is PassSingleCharacterSuggestion
            or PassSingleCharactersSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentHint
            or RedundantMethodInvocationHint
            or NotResolvedError;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestPassSingleCharacterFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestUseOtherMethodFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestRemoveMethodInvocationAvailability() => DoNamedTest2();
}