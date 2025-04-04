using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Byte;

[TestFixture]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\ByteQuickFixes";

    [Test]
    public void TestParse_String_Integer() => DoNamedTest2();

    [Test]
    public void TestParse_String_Null() => DoNamedTest2();

    [Test]
    public void TestParse_String_Integer_IFormatProvider() => DoNamedTest2();

    [Test]
    public void TestParse_String_NumberStyles_Null() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestParse_ReadOnlySpanOfChar_Null() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestParse_ReadOnlySpanOfByte_Null() => DoNamedTest2();

    [Test]
    public void TestToString_NullString() => DoNamedTest2();

    [Test]
    public void TestToString_NullIFormatProvider() => DoNamedTest2();

    [Test]
    public void TestToString_EmptyString() => DoNamedTest2();

    [Test]
    public void TestToString_Null_IFormatProvider() => DoNamedTest2();

    [Test]
    public void TestToString_EmptyString_IFormatProvider() => DoNamedTest2();

    [Test]
    public void TestToString_String_Null() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse_String_Integer() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse_String_Null() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse_ReadOnlySpanOfChar_Integer() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestTryParse_ReadOnlySpanOfChar_Null() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestTryParse_ReadOnlySpanOfByte_Integer() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestTryParse_ReadOnlySpanOfByte_Null() => DoNamedTest2();
}