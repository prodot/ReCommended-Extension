using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.FormatString;

[TestFixture]
public sealed class PassOtherFormatSpecifierFixTests : QuickFixTestBase<PassOtherFormatSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\FormatStringQuickFixes";

    [Test]
    public void TestPassOtherFormatSpecifier() => DoNamedTest2();
}