using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
public sealed class UseFloatingPointPatternFixTests : QuickFixTestBase<UseFloatingPointPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double\QuickFixes";

    [Test]
    public void TestIsNaN() => DoNamedTest2();
}