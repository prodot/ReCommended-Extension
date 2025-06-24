using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RemoveMethodInvocationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilder\QuickFixes";

    [Test]
    public void TestAppend_Char_0() => DoNamedTest2();

    [Test]
    public void TestAppend_Char_0_FluentLast() => DoNamedTest2();

    [Test]
    public void TestAppend_Char_0_Statement() => DoNamedTest2();

    [Test]
    public void TestAppend_Char_0_StatementFluentLast() => DoNamedTest2();

    [Test]
    public void TestAppend_Char_0_StatementFluent() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAppend_Char_0_Nullable() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAppend_Char_0_FluentLast_Nullable() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAppend_Char_0_Statement_Nullable() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAppend_Char_0_StatementFluentLast_Nullable() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAppend_Char_0_StatementFluent_Nullable() => DoNamedTest2();

    [Test]
    public void TestAppend_CharArray() => DoNamedTest2();

    [Test]
    public void TestAppend_CharArray_0_0() => DoNamedTest2();

    [Test]
    public void TestAppend_String() => DoNamedTest2();

    [Test]
    public void TestAppend_String_0_0() => DoNamedTest2();

    [Test]
    public void TestAppend_StringBuilder() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestAppend_StringBuilder_0_0() => DoNamedTest2();

    [Test]
    public void TestAppend_Object() => DoNamedTest2();

    [Test]
    public void TestInsert_Object() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestAppendJoin_EmptyArray() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    public void TestAppendJoin_EmptyArray_Nullable() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String() => DoNamedTest2();

    [Test]
    public void TestReplace_Char_Char() => DoNamedTest2();
}