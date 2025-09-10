using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.IntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class RemoveFormatPrecisionSpecifierFixTests : QuickFixTestBase<RemoveFormatPrecisionSpecifierFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\IntPtr\QuickFixes";

    [Test]
    public void TestToString() => DoNamedTest2();
}