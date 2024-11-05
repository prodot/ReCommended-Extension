using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore30]
public sealed class UseOtherMethodQuickFixTests : QuickFixTestBase<UseOtherMethodFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestIndexOf_Char_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_StringComparison_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_lt_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_StringComparison_lt_0() => DoNamedTest2();
}