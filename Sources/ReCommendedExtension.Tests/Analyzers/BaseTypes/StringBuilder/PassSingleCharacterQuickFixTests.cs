using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
public sealed class PassSingleCharacterQuickFixTests : QuickFixTestBase<PassSingleCharacterFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilderQuickFixes";

    [Test]
    public void TestAppend_String_SingleChar() => DoNamedTest2();

    [Test]
    public void TestAppend_String_Int32_Int32() => DoNamedTest2();

    [Test]
    public void TestInsert_String_SingleChar_1() => DoNamedTest2();

    [Test]
    public void TestInsert_String_SingleChar() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    public void TestAppendJoin_SingleChar() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNetCore21]
    public void TestAppendJoin_SingleChar_Nullable() => DoNamedTest2();
}