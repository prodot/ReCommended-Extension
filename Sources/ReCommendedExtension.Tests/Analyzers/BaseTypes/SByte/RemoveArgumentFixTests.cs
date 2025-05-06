using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.SByte;

[TestFixture]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\SByteQuickFixes";

    [Test]
    public void TestParse() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse() => DoNamedTest2();
}