using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.InterfaceImplementationDeclarations;

namespace ReCommendedExtension.Tests.ContextActions.InterfaceImplementationDeclarations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
[TestNet70]
public sealed class DeclareEqualityOperatorsExecuteTests : CSharpContextActionExecuteTestBase<DeclareEqualityOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareEqualityOperators";

    [Test]
    public void TestExecute_Class() => DoNamedTest2();

    [Test]
    public void TestExecute_Struct() => DoNamedTest2();

    [Test]
    public void TestExecute_Record() => DoNamedTest2();

    [Test]
    public void TestExecute_Record2() => DoNamedTest2();

    [Test]
    public void TestExecute_Record3() => DoNamedTest2();

    [Test]
    public void TestExecute_Record4() => DoNamedTest2();

    [Test]
    public void TestExecute_Record5() => DoNamedTest2();

    [Test]
    public void TestExecute_RecordStruct() => DoNamedTest2();
}