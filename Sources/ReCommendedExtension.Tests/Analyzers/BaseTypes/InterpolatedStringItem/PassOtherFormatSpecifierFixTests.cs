using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.InterpolatedStringItem;

[TestFixture]
public sealed class PassOtherFormatSpecifierFixTests : QuickFixTestBase<PassOtherFormatSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\InterpolatedStringItemQuickFixes";

    [Test]
    public void TestPassOtherFormatSpecifier() => DoNamedTest2();
}