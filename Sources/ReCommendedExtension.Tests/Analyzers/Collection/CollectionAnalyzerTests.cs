using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
public sealed class CollectionAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is UseEmptyForArrayInitializationWarning
            or UseTargetTypedCollectionExpressionSuggestion
            or ArrayWithDefaultValuesInitializationSuggestion
            or UseArrayEmptyMethodWarning // to figure out which cases are supported by R#
            or UseCollectionExpressionWarning // to figure out which cases are supported by R#
            or UseCollectionExpressionForArrayInitializerWarning // to figure out which cases are supported by R#
            or ReplaceInvocationWithCollectionExpressionWarning // to figure out which cases are supported by R#
            or ReplaceInvocationWithSingleSpreadCollectionExpressionWarning // to figure out which cases are supported by R#
            or ReplaceWithEmptyCollectionExpressionWarning; // to figure out which cases are supported by R#

    [Test]
    [TestNet60]
    [SuppressMessage("ReSharper", "EmptyArrayInitialization")]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    public void Compiler()
    {
        static int[] ParamsArray(params int[] items) => items;
        Assert.AreSame(Array.Empty<int>(), ParamsArray());

        int[] array = { };
        Assert.AreNotSame(Array.Empty<int>(), array);

        int[] array2 = [];
        Assert.AreSame(Array.Empty<int>(), array2);

        Assert.AreNotSame(Array.Empty<int>(), new int[] { });
        Assert.AreNotSame(Array.Empty<int>(), new int[0]);

        Assert.AreNotSame(Array.Empty<int[]>(), new int[][] { }); // not yet supported
        Assert.AreNotSame(Array.Empty<int[]>(), new int[0][]); // not yet supported
        Assert.AreNotSame(Array.Empty<int[,]>(), new int[][,] { }); // not yet supported
        Assert.AreNotSame(Array.Empty<int[,]>(), new int[0][,]); // not yet supported
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNetFramework46]
    public void EmptyArrayInitialization() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void EmptyArrayInitialization_CS12() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Array_NonTargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_Array() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_IEnumerable() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_IReadOnlyCollection() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_IReadOnlyList() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_ICollection() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_Array_Target_IList() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_List_NonTargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_List_Target_List() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_List_Target_IEnumerable() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_List_Target_IReadOnlyCollection() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_List_Target_IReadOnlyList() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_List_Target_ICollection() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_List_Target_IList() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_HashSet_NonTargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_HashSet_Target_HashSet() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_HashSet_Target_IEnumerable() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_HashSet_Target_IReadOnlyCollection() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_Dictionary_NonTargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionInitialization_Dictionary_Target_Dictionary() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Other_NonTargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_Other_Target_Other() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void CollectionInitialization_ItemTypes() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void ArrayWithDefaultValuesInitialization() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void ArrayWithDefaultValuesInitialization_CS12() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    public void ArrayWithDefaultValuesInitializationWithNullableAnnotations() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void ArrayWithDefaultValuesInitialization_TargetTyped() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void ArrayWithDefaultValuesInitialization_ParameterlessCtor() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet80]
    public void CollectionExpressionWithDefaultValuesInitialization() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void CausingExpression() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNetCore20]
    public void ParameterizedCollectionCreationWithInferredTypeArgument() => DoNamedTest();
}