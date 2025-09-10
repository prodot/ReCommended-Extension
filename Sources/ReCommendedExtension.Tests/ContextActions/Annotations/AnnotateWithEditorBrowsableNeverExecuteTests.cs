using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithEditorBrowsableNeverExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithEditorBrowsableNever>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithEditorBrowsableNever";

    [Test]
    public void TestExecute() => DoNamedTest2();
}