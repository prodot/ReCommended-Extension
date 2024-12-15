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
    public void TestExecuteWithAbstractProperty() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractProperty2() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractProperty3() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractProperty4() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractIndexer2() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractIndexer3() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractIndexer4() => DoNamedTest2();

    [Test]
    public void TestExecuteWithInterfaceProperty() => DoNamedTest2();

    [Test]
    public void TestExecuteWithInterfaceIndexer() => DoNamedTest2();

    [Test]
    public void TestExecute() => DoNamedTest2();

    [Test]
    public void TestExecuteReadOnlyProperty() => DoNamedTest2();

    [Test]
    public void TestExecuteWriteOnlyProperty() => DoNamedTest2();

    [Test]
    public void TestExecuteWithIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteWithReadOnlyIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteWithWriteOnlyIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteAutoProperty() => DoNamedTest2();
}