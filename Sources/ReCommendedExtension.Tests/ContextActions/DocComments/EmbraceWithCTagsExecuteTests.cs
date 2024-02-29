using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbraceWithCTagsExecuteTests : DocCommentsExecuteTests<EmbraceWithCTags>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbraceWithCTags";

    [Test]
    public void TestExecuteWord() => DoNamedTestWithSettings();

    [Test]
    public void TestExecuteSelection() => DoNamedTestWithSettings();
}