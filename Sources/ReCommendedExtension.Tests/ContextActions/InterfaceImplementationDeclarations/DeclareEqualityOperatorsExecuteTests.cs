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
    public void Execute_Class() => DoNamedTest();

    [Test]
    public void Execute_Struct() => DoNamedTest();

    [Test]
    public void Execute_Record() => DoNamedTest();

    [Test]
    public void Execute_Record2() => DoNamedTest();

    [Test]
    public void Execute_Record3() => DoNamedTest();

    [Test]
    public void Execute_Record4() => DoNamedTest();

    [Test]
    public void Execute_Record5() => DoNamedTest();

    [Test]
    public void Execute_RecordStruct() => DoNamedTest();
}