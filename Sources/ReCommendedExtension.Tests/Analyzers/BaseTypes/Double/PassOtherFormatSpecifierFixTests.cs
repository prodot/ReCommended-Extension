using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class PassOtherFormatSpecifierFixTests : QuickFixTestBase<PassOtherFormatSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double\QuickFixes";

    [Test]
    public void TestToString() => DoNamedTest2();
}