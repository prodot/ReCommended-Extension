using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class AnnotateWithPureExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithPure>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithPure";

    [Test]
    public void TestExecuteNotAnnotatedMethod() => DoNamedTest2();

    [Test]
    public void TestExecuteAnnotatedMethod() => DoNamedTest2();
}