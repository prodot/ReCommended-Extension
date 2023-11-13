using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.Tests.Analyzers.InterfaceImplementation;

[TestFixture]
[TestNet70]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
public sealed class ImplementEqualityOperatorsInterfaceViaEqualsFixTests : QuickFixTestBase<ImplementEqualityOperatorsInterfaceViaEqualsFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementationQuickFixes";

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestEquatableTypes_Class_ImplementEqualityOperators_ViaEquals() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Disable)]
    public void TestEquatableTypes_Class_ImplementEqualityOperators_ViaEquals_NonNullable() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Struct_ImplementEqualityOperators_ViaEquals() => DoNamedTest2();
}