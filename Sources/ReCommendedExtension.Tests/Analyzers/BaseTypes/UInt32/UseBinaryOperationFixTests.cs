using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt32;

[TestFixture]
public sealed class UseBinaryOperationFixTests : QuickFixTestBase<UseBinaryOperationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt32QuickFixes";

    [Test]
    public void TestEquals_UInt32() => DoNamedTest2();
}