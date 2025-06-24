using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.InterpolatedStringExpression;

[TestFixture]
public sealed class PassOtherFormatSpecifierFixTests : QuickFixTestBase<PassOtherFormatSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\InterpolatedStringExpression\QuickFixes";

    [Test]
    public void TestPassOtherFormatSpecifier() => DoNamedTest2();
}