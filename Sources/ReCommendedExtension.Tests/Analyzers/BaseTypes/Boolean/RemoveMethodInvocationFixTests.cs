using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Boolean;

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RemoveMethodInvocationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\BooleanQuickFixes";

    [Test]
    public void TestEquals_Boolean_Qualifier() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Qualifier_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Argument() => DoNamedTest2();

    [Test]
    public void TestEquals_Boolean_Argument_Parenthesized() => DoNamedTest2();
}