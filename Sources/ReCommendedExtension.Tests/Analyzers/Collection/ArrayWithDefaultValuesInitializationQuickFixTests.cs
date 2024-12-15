using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
public sealed class ArrayWithDefaultValuesInitializationQuickFixTests : QuickFixTestBase<ReplaceWithNewArrayWithLengthFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    [Test]
    public void TestArrayWithDefaultValuesInitialization() => DoNamedTest2();

    [Test]
    public void TestArrayWithDefaultValuesInitialization2() => DoNamedTest2();

    [Test]
    public void TestArrayWithDefaultValuesInitialization3() => DoNamedTest2();

    [Test]
    public void TestArrayWithDefaultValuesInitialization4() => DoNamedTest2();

    [Test]
    public void TestArrayWithDefaultValuesInitialization5() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [NullableContext(NullableContextKind.Enable)]
    public void TestArrayWithDefaultValuesInitialization6() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    public void TestArrayWithDefaultValuesInitialization7() => DoNamedTest2();
}