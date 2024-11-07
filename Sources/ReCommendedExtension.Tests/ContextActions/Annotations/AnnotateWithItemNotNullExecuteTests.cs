using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNetFramework45]
[TestPackagesWithAnnotations("System.Threading.Tasks.Extensions")]
public sealed class AnnotateWithItemNotNullExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithItemNotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

    [Test]
    public void TestExecuteJaggedArrayOnField() => DoNamedTest2();

    [Test]
    public void TestExecuteMultiDimensionalArrayOnField() => DoNamedTest2();

    [Test]
    public void TestExecuteArrayOnMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericCollectionOnParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericListOnProperty() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericListOnIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericListOnParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericTaskOnMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteLazyOnDelegate() => DoNamedTest2();

    [Test]
    public void TestExecuteGenericValueTaskOnMethod() => DoNamedTest2();
}