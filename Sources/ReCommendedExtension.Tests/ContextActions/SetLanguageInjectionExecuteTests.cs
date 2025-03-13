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
    public void TestExecuteFieldAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteFieldUpdate() => DoNamedTest2();

    [Test]
    public void TestExecuteConstantAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteConstantUpdate() => DoNamedTest2();

    [Test]
    public void TestExecuteVariableAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteVariableUpdate() => DoNamedTest2();

    [Test]
    public void TestExecuteLocalConstantAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteLocalConstantUpdate() => DoNamedTest2();

    [Test]
    public void TestExecuteAssignmentAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteAssignmentUpdate() => DoNamedTest2();

    [Test]
    public void TestExecuteObjectInitializationAdd() => DoNamedTest2();

    [Test]
    public void TestExecuteObjectInitializationUpdate() => DoNamedTest2();
}