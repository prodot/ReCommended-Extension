using JetBrains.ReSharper.Psi.CSharp.Tree;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class EmbedIntoSeeCRefExecuteTests : DocCommentsExecuteTests<EmbedIntoSeeCRef, IDocCommentNode>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoSeeCRef";

    [Test]
    public void TestExecuteWord() => DoNamedTestWithSettings();

    [Test]
    public void TestExecuteSelection() => DoNamedTestWithSettings();
}