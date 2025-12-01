using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.YieldReturnWithinLock;

namespace ReCommendedExtension.Tests.Analyzers.YieldReturnWithinLock;

[TestFixture]
public sealed class YieldReturnWithinLockAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\YieldReturnWithinLock";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is YieldReturnWithinLockWarning;

    [Test]
    public void TestYieldReturnWithinLock() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet90]
    public void TestYieldReturnWithinLockAroundLockObject() => DoNamedTest2();
}