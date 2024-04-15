using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithLinqTunnelExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithLinqTunnel>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

    [Test]
    public void TestExecute() => DoNamedTest2();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestExecuteAsyncEnumerable() => DoNamedTest2();
}