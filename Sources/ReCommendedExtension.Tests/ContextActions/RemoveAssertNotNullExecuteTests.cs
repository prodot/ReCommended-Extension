using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
public sealed class RemoveAssertNotNullExecuteTests : CSharpContextActionExecuteTestBase<RemoveAssertNotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\RemoveAssertNotNull";

    [Test]
    public void TestExecute() => DoNamedTest2();
}