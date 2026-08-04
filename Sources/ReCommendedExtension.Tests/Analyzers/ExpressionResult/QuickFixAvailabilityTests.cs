using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ExpressionResult;

namespace ReCommendedExtension.Tests.Analyzers.ExpressionResult;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet70]
public sealed class QuickFixAvailabilityTests : ReCommendedExtension.Tests.Analyzers.QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ExpressionResult\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UseExpressionResultSuggestion;

    [Test]
    public void UseExpressionResultFixAvailability() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void UseExpressionResultFixAvailability_CS12() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void UseExpressionResultFixAvailability_CS11() => DoNamedTest();
}