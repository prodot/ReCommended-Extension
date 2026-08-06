using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class CollectionCountPositiveExecuteTests : CSharpContextActionExecuteTestBase<CollectionCountPositive>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\CollectionCountPositive";

    [Test]
    public void ExecuteGenericCollection() => DoNamedTest();

    [Test]
    public void ExecuteArray() => DoNamedTest();

    [Test]
    public void ExecuteArrayType() => DoNamedTest();

    [Test]
    public void ExecuteDictionary() => DoNamedTest();

    [Test]
    public void ExecuteCollection() => DoNamedTest();
}