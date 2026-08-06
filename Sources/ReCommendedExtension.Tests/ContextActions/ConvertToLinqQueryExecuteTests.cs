using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class ConvertToLinqQueryExecuteTests : CSharpContextActionExecuteTestBase<ConvertToLinqQuery>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ConvertToLinqQuery";

    [Test]
    public void Execute_IEnumerable() => DoNamedTest();

    [Test]
    public void Execute_IEnumerable_Generic() => DoNamedTest();

    [Test]
    public void Execute_IEnumerable_Fluent() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_ICollection_Spread() => DoNamedTest();

    [Test]
    public void Execute_ICollection_Target_IEnumerable() => DoNamedTest();

    [Test]
    public void Execute_Array_Target_IEnumerable() => DoNamedTest();

    [Test]
    public void Execute_List_ToArray() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_List_Target_IReadOnlyCollection_CollectionExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_List_Target_IReadOnlyList_CollectionExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_HashSet_Target_ICollection_CollectionExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_Array_Target_IList_CollectionExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_List_Target_List_CollectionExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Execute_Array_Target_Array_CollectionExpression() => DoNamedTest();

    [Test]
    public void Execute_List() => DoNamedTest();

    [Test]
    public void Execute_Array() => DoNamedTest();

    [Test]
    public void Execute_HashSet() => DoNamedTest();

    [Test]
    [TestNet100]
    public void Execute_IAsyncEnumerable() => DoNamedTest();

    [Test]
    [TestNet100]
    public void Execute_IAsyncEnumerable_Generic() => DoNamedTest();

    [Test]
    [TestNet100]
    public void Execute_IAsyncEnumerable_Fluent() => DoNamedTest();
}