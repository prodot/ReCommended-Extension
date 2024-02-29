using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.InterfaceImplementationDeclarations;

namespace ReCommendedExtension.Tests.ContextActions.InterfaceImplementationDeclarations;

[TestFixture]
[TestNet70]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[NullableContext(NullableContextKind.Enable)]
public sealed class DeclareComparisonOperatorsExecuteTests : CSharpContextActionExecuteTestBase<DeclareComparisonOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareComparisonOperators";

    [Test]
    public void TestExecute_Class() => DoNamedTest2();

    [Test]
    public void TestExecute_Struct() => DoNamedTest2();

    [Test]
    public void TestExecute_Record() => DoNamedTest2();
}