using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.CollectionAnalyzer;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
[TestFixture]
public sealed class ReplaceWithEmptyCollectionExpressionQuickFixTests : QuickFixTestBase<ReplaceWithEmptyCollectionExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    [Test]
    public void TestEmptyArrayInitialization5() => DoNamedTest2();
}