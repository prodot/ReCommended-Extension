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
    public void ExecuteWithEmptyMethod() => DoNamedTest();

    [Test]
    public void ExecuteWithNonEmptyMethod() => DoNamedTest();

    [Test]
    public void ExecuteWithNonEmptyMethod2() => DoNamedTest();

    [Test]
    public void ExecuteWithNonEmptyMethod3() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractMethod() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractMethod2() => DoNamedTest();

    [Test]
    public void ExecuteWithAbstractMethod3() => DoNamedTest();

    [Test]
    public void ExecuteWithInterfaceMethod() => DoNamedTest();
}