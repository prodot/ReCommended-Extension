using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements;

[TestFixture]
[TestNetFramework4]
public sealed class MethodExecuteTests : CSharpContextActionExecuteTestBase<NotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Method";

    [Test]
    public void TestExecuteWithEmptyMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod2() => DoNamedTest2();

    [Test]
    public void TestExecuteWithNonEmptyMethod3() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractMethod2() => DoNamedTest2();

    [Test]
    public void TestExecuteWithAbstractMethod3() => DoNamedTest2();

    [Test]
    public void TestExecuteWithInterfaceMethod() => DoNamedTest2();
}