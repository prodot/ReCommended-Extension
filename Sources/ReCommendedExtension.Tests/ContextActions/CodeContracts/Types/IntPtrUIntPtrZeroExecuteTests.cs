using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class IntPtrUIntPtrZeroExecuteTests : CSharpContextActionExecuteTestBase<IntPtrUIntPtrZero>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\IntPtrUIntPtrZero";

    [Test]
    public void TestExecuteIntPtr() => DoNamedTest2();

    [Test]
    public void TestExecuteUIntPtr() => DoNamedTest2();
}