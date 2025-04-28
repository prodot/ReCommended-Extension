using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class UseFloatingPointPatternFixTests : QuickFixTestBase<UseFloatingPointPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DoubleQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestIsNaN() => DoNamedTest2();
}