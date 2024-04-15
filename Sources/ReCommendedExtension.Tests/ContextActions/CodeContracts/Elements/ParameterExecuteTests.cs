using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements;

[TestNetFramework4]
[TestFixture]
public sealed class ParameterExecuteTests : CSharpContextActionExecuteTestBase<NotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Parameter";

    [Test]
    public void TestExecuteWithAbstractMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithInterfaceMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithEmptyMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod2() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod3() => DoNamedTest2();

    [Test]
    public void TestExecuteWithRefParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteWithOutParameter() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteWithInterfaceIndexer() => DoNamedTest2();

    [Test]
    public void TestExecuteWithIndexer() => DoNamedTest2();
}