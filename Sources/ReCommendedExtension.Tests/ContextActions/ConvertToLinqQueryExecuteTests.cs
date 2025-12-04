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
    public void TestExecute_IEnumerable() => DoNamedTest2();

    [Test]
    public void TestExecute_IEnumerable_Generic() => DoNamedTest2();

    [Test]
    public void TestExecute_IEnumerable_Fluent() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_ICollection_Spread() => DoNamedTest2();

    [Test]
    public void TestExecute_ICollection_Target_IEnumerable() => DoNamedTest2();

    [Test]
    public void TestExecute_Array_Target_IEnumerable() => DoNamedTest2();

    [Test]
    public void TestExecute_List_ToArray() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_List_Target_IReadOnlyCollection_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_List_Target_IReadOnlyList_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_HashSet_Target_ICollection_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_Array_Target_IList_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_List_Target_List_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestExecute_Array_Target_Array_CollectionExpression() => DoNamedTest2();

    [Test]
    public void TestExecute_List() => DoNamedTest2();

    [Test]
    public void TestExecute_Array() => DoNamedTest2();

    [Test]
    public void TestExecute_HashSet() => DoNamedTest2();

    [Test]
    [TestNet100]
    public void TestExecute_IAsyncEnumerable() => DoNamedTest2();

    [Test]
    [TestNet100]
    public void TestExecute_IAsyncEnumerable_Generic() => DoNamedTest2();

    [Test]
    [TestNet100]
    public void TestExecute_IAsyncEnumerable_Fluent() => DoNamedTest2();
}