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
public sealed class MarkAsImplementingEqualityOperatorsInterfaceFixTests : QuickFixTestBase<MarkAsImplementingEqualityOperatorsInterfaceFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementationQuickFixes";

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestEquatableTypes_Class_MarkAsImplementingEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Struct_MarkAsImplementingEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_MarkAsImplementingEqualityOperators() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_MarkAsImplementingEqualityOperators2() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_MarkAsImplementingEqualityOperators3() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_MarkAsImplementingEqualityOperators4() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_Record_MarkAsImplementingEqualityOperators5() => DoNamedTest2();

    [Test]
    public void TestEquatableTypes_RecordStruct_MarkAsImplementingEqualityOperators() => DoNamedTest2();
}