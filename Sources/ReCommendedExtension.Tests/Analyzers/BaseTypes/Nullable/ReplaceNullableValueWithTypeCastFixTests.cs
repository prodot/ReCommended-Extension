using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
public sealed class ReplaceNullableValueWithTypeCastFixTests : QuickFixTestBase<ReplaceNullableValueWithTypeCastFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Nullable\QuickFixes";

    [Test]
    public void TestValue() => DoNamedTest2();

    [Test]
    public void TestValue_Parenthesized() => DoNamedTest2();
}