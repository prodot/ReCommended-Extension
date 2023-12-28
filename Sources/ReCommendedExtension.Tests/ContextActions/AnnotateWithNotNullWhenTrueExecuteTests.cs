using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class AnnotateWithNotNullWhenTrueExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithNotNullWhenTrue>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNotNullWhenTrue";

    [Test]
    public void TestExecute() => DoNamedTest2();
}