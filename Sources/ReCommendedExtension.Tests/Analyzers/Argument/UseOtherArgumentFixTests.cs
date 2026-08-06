using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
[TestNetCore21]
public sealed class UseOtherArgumentFixTests : QuickFixTestBase<UseOtherArgumentSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void UseOtherArgument() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void UseOtherArgument_Named() => DoNamedTest();

    [Test]
    public void UseOtherArgument_AdditionalArgument() => DoNamedTest();

    [Test]
    public void UseOtherArgument_AdditionalArgument_Named() => DoNamedTest();

    [Test]
    public void UseOtherArgument_RedundantArgument() => DoNamedTest();

    [Test]
    public void UseOtherArgument_RedundantArgument_Named() => DoNamedTest();

    [Test]
    public void UseOtherArgument_RedundantArgument_OutOfOrder() => DoNamedTest();
}