using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
public sealed class RemoveArgumentQuickFixTests : QuickFixTestBase<RemoveArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilder\QuickFixes";

    [Test]
    public void TestAppend_Char_1() => DoNamedTest2();

    [Test]
    public void TestInsert_String_1() => DoNamedTest2();
}