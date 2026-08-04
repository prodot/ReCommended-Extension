using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class ConvertToLinqQueryAvailabilityTests : CSharpContextActionAvailabilityTestBase<ConvertToLinqQuery>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ConvertToLinqQuery";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_IEnumerable() => DoNamedTest();

    [Test]
    public void Availability_IEnumerable_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_IReadOnlyCollection() => DoNamedTest();

    [Test]
    public void Availability_IReadOnlyCollection_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_IReadOnlyList() => DoNamedTest();

    [Test]
    public void Availability_IReadOnlyList_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_ICollection() => DoNamedTest();

    [Test]
    public void Availability_ICollection_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_IList() => DoNamedTest();

    [Test]
    public void Availability_IList_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_List() => DoNamedTest();

    [Test]
    public void Availability_List_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_HashSet() => DoNamedTest();

    [Test]
    public void Availability_HashSet_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_Array() => DoNamedTest();

    [Test]
    public void Availability_Array_NoCollectionExpressions() => DoNamedTest();

    [Test]
    [TestNet100]
    public void Availability_IAsyncEnumerable() => DoNamedTest();
}