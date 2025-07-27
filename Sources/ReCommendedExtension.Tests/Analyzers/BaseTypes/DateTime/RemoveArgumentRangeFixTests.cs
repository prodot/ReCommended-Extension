using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class RemoveArgumentRangeFixTests : QuickFixTestBase<RemoveArgumentRangeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime\QuickFixes";

    [Test]
    public void Test_Constructors_RedundantArgumentRange() => DoNamedTest2();

    [Test]
    public void Test_Constructors_RedundantArgumentRange2() => DoNamedTest2();
}