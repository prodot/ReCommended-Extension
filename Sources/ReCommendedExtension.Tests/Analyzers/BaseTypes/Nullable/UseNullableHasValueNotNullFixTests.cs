using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
public sealed class UseNullableHasValueNotNullFixTests : QuickFixTestBase<UseNullableHasValueNotNullFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\NullableQuickFixes";

    [Test]
    public void TestHasValue_NotNull() => DoNamedTest2();

    [Test]
    public void TestHasValue_NotNull_Parenthesized() => DoNamedTest2();
}