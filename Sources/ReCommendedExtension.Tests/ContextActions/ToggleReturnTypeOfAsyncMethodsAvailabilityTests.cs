using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[TestNetFramework46]
public sealed class ToggleReturnTypeOfAsyncMethodsAvailabilityTests : CSharpContextActionAvailabilityTestBase<ToggleReturnTypeOfAsyncMethods>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ToggleReturnTypeOfAsyncMethods";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    public void TestUnavailability() => DoNamedTest2();
}