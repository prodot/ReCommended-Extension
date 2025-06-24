using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RemoveMethodInvocationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Strings\QuickFixes";

    [Test]
    public void TestPadLeft_0() => DoNamedTest2();

    [Test]
    public void TestPadLeft_0_Char() => DoNamedTest2();

    [Test]
    public void TestPadRight_0() => DoNamedTest2();

    [Test]
    public void TestPadRight_0_Char() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_Identical() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_Identical() => DoNamedTest2();

    [Test]
    public void TestReplace_Char_Char_Identical() => DoNamedTest2();

    [Test]
    public void TestSubstring_0() => DoNamedTest2();

    [Test]
    public void TestToString_IFormatProvider() => DoNamedTest2();
}