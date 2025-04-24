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
    public void TestParse() => DoNamedTest2();

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
    public void TestTryParse() => DoNamedTest2();
}