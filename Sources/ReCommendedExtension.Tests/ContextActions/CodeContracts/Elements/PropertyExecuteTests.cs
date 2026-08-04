using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements;

[TestFixture]
[TestNetFramework4]
public sealed class PropertyExecuteTests : CSharpContextActionExecuteTestBase<NotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Property";

    [Test]
    public void ExecuteWithAbstractProperty() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractProperty2() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractProperty3() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractProperty4() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractIndexer() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractIndexer2() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractIndexer3() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractIndexer4() => DoNamedTest();

    [Test]
    public void ExecuteWithInterfaceProperty() => DoNamedTest();

    [Test]
    public void ExecuteWithInterfaceIndexer() => DoNamedTest();

    [Test]
    public void Execute() => DoNamedTest();

    [Test]
    public void ExecuteReadOnlyProperty() => DoNamedTest();

    [Test]
    public void ExecuteWriteOnlyProperty() => DoNamedTest();

    [Test]
    public void ExecuteWithIndexer() => DoNamedTest();

    [Test]
    public void ExecuteWithReadOnlyIndexer() => DoNamedTest();

    [Test]
    public void ExecuteWithWriteOnlyIndexer() => DoNamedTest();

    [Test]
    public void ExecuteAutoProperty() => DoNamedTest();
}