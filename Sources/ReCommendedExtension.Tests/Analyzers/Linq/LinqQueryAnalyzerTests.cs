using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class LinqQueryAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is RedundantLinqQueryHint;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestNoOpQuery() => DoNamedTest2();

    [Test]
    [TestNet100]
    public void TestNoOpAsyncQuery() => DoNamedTest2();
}