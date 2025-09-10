using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Guid;

[TestFixture]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Guid\QuickFixes";

    [Test]
    public void TestEquals_Guid() => DoNamedTest2();
}