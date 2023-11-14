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
public sealed class MarkAsImplementingEqualityOperatorsInterfaceFixTests : QuickFixTestBase<ImplementEqualityOperatorsInterfaceFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementationQuickFixes";

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestEquatableTypes_Class_ImplementEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Struct_ImplementEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_ImplementEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_ImplementEqualityOperators2() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_ImplementEqualityOperators3() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_ImplementEqualityOperators4() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_ImplementEqualityOperators5() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_RecordStruct_MarkAsImplementingEqualityOperators() => DoNamedTest2();
}