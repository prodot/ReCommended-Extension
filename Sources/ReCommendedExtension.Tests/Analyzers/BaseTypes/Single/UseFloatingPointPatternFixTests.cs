using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Single;

[TestFixture]
public sealed class UseFloatingPointPatternFixTests : QuickFixTestBase<UseFloatingPointPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\SingleQuickFixes";

    [Test]
    public void TestIsNaN() => DoNamedTest2();
}