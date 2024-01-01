using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.EmptyArrayInitialization;

namespace ReCommendedExtension.Tests.Analyzers.EmptyArrayInitialization;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
[TestFixture]
public sealed class ReplaceWithEmptyCollectionExpressionQuickFixTests : QuickFixTestBase<ReplaceWithEmptyCollectionExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\EmptyArrayInitializationQuickFixes";

    [Test]
    public void TestEmptyArrayInitialization5() => DoNamedTest2();
}