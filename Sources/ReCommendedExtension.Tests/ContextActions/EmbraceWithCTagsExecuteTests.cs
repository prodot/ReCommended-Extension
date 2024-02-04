using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbraceWithCTagsExecuteTests : CSharpContextActionExecuteTestBase<EmbraceWithCTags>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbraceWithCTags";

    [Test]
    public void TestExecuteWord() => DoNamedTest2();

    [Test]
    public void TestExecuteSelection() => DoNamedTest2();
}