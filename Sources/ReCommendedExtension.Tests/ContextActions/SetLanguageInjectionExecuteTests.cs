using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class SetLanguageInjectionExecuteTests : CSharpContextActionExecuteTestBase<SetLanguageInjection>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\SetLanguageInjection";

    [Test]
    public void ExecuteFieldAdd() => DoNamedTest();

    [Test]
    public void ExecuteFieldUpdate() => DoNamedTest();

    [Test]
    public void ExecuteConstantAdd() => DoNamedTest();

    [Test]
    public void ExecuteConstantUpdate() => DoNamedTest();

    [Test]
    public void ExecuteVariableAdd() => DoNamedTest();

    [Test]
    public void ExecuteVariableUpdate() => DoNamedTest();

    [Test]
    public void ExecuteLocalConstantAdd() => DoNamedTest();

    [Test]
    public void ExecuteLocalConstantUpdate() => DoNamedTest();

    [Test]
    public void ExecuteAssignmentAdd() => DoNamedTest();

    [Test]
    public void ExecuteAssignmentUpdate() => DoNamedTest();

    [Test]
    public void ExecuteObjectInitializationAdd() => DoNamedTest();

    [Test]
    public void ExecuteObjectInitializationUpdate() => DoNamedTest();
}