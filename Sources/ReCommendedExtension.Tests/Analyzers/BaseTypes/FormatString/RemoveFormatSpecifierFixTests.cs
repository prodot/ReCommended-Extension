using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.FormatString;

[TestFixture]
public sealed class RemoveFormatSpecifierFixTests : QuickFixTestBase<RemoveFormatSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\FormatString\QuickFixes";

    [Test]
    public void TestRemoveFormatSpecifier() => DoNamedTest2();
}