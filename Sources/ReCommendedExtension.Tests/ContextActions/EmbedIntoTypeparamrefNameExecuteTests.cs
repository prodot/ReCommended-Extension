using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoTypeparamrefNameExecuteTests : CSharpContextActionExecuteTestBase<EmbedIntoTypeparamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoTypeparamrefName";

    [Test]
    public void TestExecuteWord() => DoNamedTest2();

    [Test]
    public void TestExecuteSelection() => DoNamedTest2();
}