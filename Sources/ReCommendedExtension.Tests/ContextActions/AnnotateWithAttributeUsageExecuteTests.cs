using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class AnnotateWithAttributeUsageExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithAttributeUsage>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithAttributeUsage";

    [Test]
    public void TestExecute() => DoNamedTest2();
}