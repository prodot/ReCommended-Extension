using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Strings;

namespace ReCommendedExtension.Tests.ContextActions.Strings;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
public sealed class ReplaceIsNullOrEmptyWithNegatedListPatternAvailabilityTests
    : CSharpContextActionAvailabilityTestBase<ReplaceIsNullOrEmptyWithNegatedListPattern>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReplaceIsNullOrEmpty";

    [Test]
    public void TestAvailability() => DoNamedTest2(); // same file used by multiple tests
}