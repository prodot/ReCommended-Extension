using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class EmbedIntoParamrefNameExecuteTests : CSharpContextActionExecuteTestBase<EmbedIntoParamrefName>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\EmbedIntoParamrefName";

    [Test]
    public void TestExecuteWord() => DoNamedTest2();

    [Test]
    public void TestExecuteSelection() => DoNamedTest2();
}