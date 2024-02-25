using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class ReflowDocCommentsExecuteTests : CSharpContextActionExecuteTestBase<ReflowDocComments>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReflowDocComments";

    [Test]
    public void TestExecute_WrapTopLevelTags() => DoNamedTest2();

    [Test]
    public void TestExecute_WrapNestedTags() => DoNamedTest2();

    [Test]
    public void TestExecute_WrapNestedTags_Para() => DoNamedTest2();

    [Test]
    public void TestExecute_ReorderTopLevelTags() => DoNamedTest2();
}