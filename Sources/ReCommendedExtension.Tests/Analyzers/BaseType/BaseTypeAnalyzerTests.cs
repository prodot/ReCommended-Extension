using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseType;

namespace ReCommendedExtension.Tests.Analyzers.BaseType;

[TestFixture]
public sealed class BaseTypeAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseType";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RemoveRedundantBaseTypeDeclarationSuggestion;

    [Test]
    [TestNet80]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestBaseTypes() => DoNamedTest2();
}