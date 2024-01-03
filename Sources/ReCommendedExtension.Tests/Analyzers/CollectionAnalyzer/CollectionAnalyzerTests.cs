﻿using JetBrains.Application.Settings;
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
    public void TestCollectionInitialization_NonTargetTyped() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetArray() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetEnumerable() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetReadOnlyCollection() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetReadOnlyList() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetCollection() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestCollectionInitialization_TargetList() => DoNamedTest2();

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
}