using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbedIntoParamrefNameExecuteTests : DocCommentsExecuteTests<EmbedIntoParamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoParamrefName";

    [Test]
    public void TestExecuteWord() => DoNamedTestWithSettings();

    [Test]
    public void TestExecuteSelection() => DoNamedTestWithSettings();
}