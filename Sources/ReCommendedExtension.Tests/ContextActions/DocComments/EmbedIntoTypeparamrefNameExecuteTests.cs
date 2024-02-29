using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbedIntoTypeparamrefNameExecuteTests : DocCommentsExecuteTests<EmbedIntoTypeparamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoTypeparamrefName";

    [Test]
    public void TestExecuteWord() => DoNamedTestWithSettings();

    [Test]
    public void TestExecuteSelection() => DoNamedTestWithSettings();
}