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
public sealed class DeclareComparisonOperatorsExecuteTests : CSharpContextActionExecuteTestBase<DeclareComparisonOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareComparisonOperators";

    [Test]
    public void Execute_Class() => DoNamedTest();

    [Test]
    public void Execute_Struct() => DoNamedTest();

    [Test]
    public void Execute_Record() => DoNamedTest();
}