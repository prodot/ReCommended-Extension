using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoSeeCRefExecuteTests : CSharpContextActionExecuteTestBase<EmbedIntoSeeCRef>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoSeeCRef";

    [Test]
    public void TestExecuteWord() => DoNamedTest2();

    [Test]
    public void TestExecuteSelection() => DoNamedTest2();
}