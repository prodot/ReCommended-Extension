using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithInstantHandleExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithInstantHandle>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

    [Test]
    public void ExecuteGenericEnumerable() => DoNamedTest();

    [Test]
    public void ExecuteDelegate() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    public void ExecuteAsyncEnumerable() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void ExecuteLambda() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void ExecuteLambda2() => DoNamedTest();
}