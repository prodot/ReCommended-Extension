using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
public sealed class ArrayWithDefaultValuesInitializationQuickFixTests : QuickFixTestBase<ArrayWithDefaultValuesInitializationSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    [Test]
    public void ArrayWithDefaultValuesInitialization() => DoNamedTest();

    [Test]
    public void ArrayWithDefaultValuesInitialization2() => DoNamedTest();

    [Test]
    public void ArrayWithDefaultValuesInitialization3() => DoNamedTest();

    [Test]
    public void ArrayWithDefaultValuesInitialization4() => DoNamedTest();

    [Test]
    public void ArrayWithDefaultValuesInitialization5() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [NullableContext(NullableContextKind.Enable)]
    public void ArrayWithDefaultValuesInitialization6() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    public void ArrayWithDefaultValuesInitialization7() => DoNamedTest();
}