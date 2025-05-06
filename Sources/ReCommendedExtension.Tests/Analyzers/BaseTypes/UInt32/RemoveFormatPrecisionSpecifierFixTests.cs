using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt32;

[TestFixture]
public sealed class RemoveFormatPrecisionSpecifierFixTests : QuickFixTestBase<RemoveFormatPrecisionSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt32QuickFixes";

    [Test]
    public void TestToString() => DoNamedTest2();
}