using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.StringBuilder;

[TestFixture]
public sealed class RemoveArgumentQuickFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringBuilderQuickFixes";

    [Test]
    public void TestAppend_Char_1() => DoNamedTest2();

    [Test]
    public void TestInsert_String_1() => DoNamedTest2();
}