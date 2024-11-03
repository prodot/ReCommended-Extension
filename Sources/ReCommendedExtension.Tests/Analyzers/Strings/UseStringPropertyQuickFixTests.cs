using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
public sealed class UseStringPropertyQuickFixTests : QuickFixTestBase<UseStringPropertyFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestLastIndexOf_Empty() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_Empty_StringComparison() => DoNamedTest2();
}