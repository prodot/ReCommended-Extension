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
    public void ExecuteJaggedArrayOnField() => DoNamedTest();

    [Test]
    public void ExecuteMultiDimensionalArrayOnField() => DoNamedTest();

    [Test]
    public void ExecuteArrayOnMethod() => DoNamedTest();

    [Test]
    public void ExecuteGenericCollectionOnParameter() => DoNamedTest();

    [Test]
    public void ExecuteGenericListOnProperty() => DoNamedTest();

    [Test]
    public void ExecuteGenericListOnIndexer() => DoNamedTest();

    [Test]
    public void ExecuteGenericListOnParameter() => DoNamedTest();

    [Test]
    public void ExecuteGenericTaskOnMethod() => DoNamedTest();

    [Test]
    public void ExecuteLazyOnDelegate() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void ExecuteGenericValueTaskOnMethod() => DoNamedTest();
}