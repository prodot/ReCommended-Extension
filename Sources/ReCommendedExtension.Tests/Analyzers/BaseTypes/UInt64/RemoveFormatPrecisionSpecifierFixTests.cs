using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt64;

[TestFixture]
public sealed class RemoveFormatPrecisionSpecifierFixTests : QuickFixTestBase<RemoveFormatPrecisionSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt64QuickFixes";

    [Test]
    public void TestToString() => DoNamedTest2();
}