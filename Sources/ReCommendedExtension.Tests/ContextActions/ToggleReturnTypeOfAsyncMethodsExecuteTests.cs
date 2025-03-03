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
    public void TestTaskToValueTask() => DoNamedTest2();

    [Test]
    public void TestValueTaskToTask() => DoNamedTest2();

    [Test]
    public void TestGenericTaskToGenericValueTask_Int32() => DoNamedTest2();

    [Test]
    public void TestGenericTaskToGenericValueTask_T() => DoNamedTest2();

    [Test]
    public void TestGenericValueTaskToGenericTask_Int32() => DoNamedTest2();

    [Test]
    public void TestGenericValueTaskToGenericTask_T() => DoNamedTest2();
}