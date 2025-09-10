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
    public void TestAvailability_IEnumerable() => DoNamedTest2();

    [Test]
    public void TestAvailability_IEnumerable_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_IReadOnlyCollection() => DoNamedTest2();

    [Test]
    public void TestAvailability_IReadOnlyCollection_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_IReadOnlyList() => DoNamedTest2();

    [Test]
    public void TestAvailability_IReadOnlyList_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_ICollection() => DoNamedTest2();

    [Test]
    public void TestAvailability_ICollection_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_IList() => DoNamedTest2();

    [Test]
    public void TestAvailability_IList_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_List() => DoNamedTest2();

    [Test]
    public void TestAvailability_List_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_HashSet() => DoNamedTest2();

    [Test]
    public void TestAvailability_HashSet_NoCollectionExpressions() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestAvailability_Array() => DoNamedTest2();

    [Test]
    public void TestAvailability_Array_NoCollectionExpressions() => DoNamedTest2();
}