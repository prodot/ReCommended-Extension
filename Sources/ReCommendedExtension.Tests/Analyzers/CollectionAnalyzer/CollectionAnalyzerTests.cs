using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.CollectionAnalyzer;

[TestFixture]
public sealed class CollectionAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseEmptyForArrayInitializationWarning
            or UseTargetTypedCollectionExpressionSuggestion
            or ArrayWithDefaultValuesInitializationSuggestion
            or UseArrayEmptyMethodWarning // to figure out which cases are supported by R#
            or UseCollectionExpressionWarning; // to figure out which cases are supported by R#

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
    public void TestEmptyArrayInitialization() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestEmptyArrayInitialization_CS12() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_NonTargetTyped() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_Array() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_IEnumerable() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_IReadOnlyCollection() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_IReadOnlyList() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_ICollection() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Array_Target_IList() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_NonTargetTyped() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_List() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_IEnumerable() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_IReadOnlyCollection() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_IReadOnlyList() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_ICollection() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_List_Target_IList() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_HashSet_NonTargetTyped() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_HashSet_Target_HashSet() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_HashSet_Target_IEnumerable() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_HashSet_Target_IReadOnlyCollection() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Dictionary_NonTargetTyped() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_Dictionary_Target_Dictionary() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_ItemTypes() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestArrayWithDefaultValuesInitialization() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestArrayWithDefaultValuesInitialization_CS12() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    public void TestArrayWithDefaultValuesInitializationWithNullableAnnotations() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    public void TestArrayWithDefaultValuesInitialization_TargetTyped() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestArrayWithDefaultValuesInitialization_ParameterlessCtor() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionExpressionWithDefaultValuesInitialization() => DoNamedTest2();
}