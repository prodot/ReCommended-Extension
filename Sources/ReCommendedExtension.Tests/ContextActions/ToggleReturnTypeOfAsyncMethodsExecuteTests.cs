using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class ToggleReturnTypeOfAsyncMethodsExecuteTests : CSharpContextActionExecuteTestBase<ToggleReturnTypeOfAsyncMethods>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ToggleReturnTypeOfAsyncMethods";

    [Test]
    public void TaskToValueTask() => DoNamedTest();

    [Test]
    public void ValueTaskToTask() => DoNamedTest();

    [Test]
    public void GenericTaskToGenericValueTask_Int32() => DoNamedTest();

    [Test]
    public void GenericTaskToGenericValueTask_T() => DoNamedTest();

    [Test]
    public void GenericValueTaskToGenericTask_Int32() => DoNamedTest();

    [Test]
    public void GenericValueTaskToGenericTask_T() => DoNamedTest();
}