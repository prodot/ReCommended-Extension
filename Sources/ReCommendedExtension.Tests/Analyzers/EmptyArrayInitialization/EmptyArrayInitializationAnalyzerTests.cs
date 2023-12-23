﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.EmptyArrayInitialization;

namespace ReCommendedExtension.Tests.Analyzers.EmptyArrayInitialization;

[TestFixture]
public sealed class EmptyArrayInitializationAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\EmptyArrayInitialization";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is EmptyArrayInitializationWarning or UseArrayEmptyMethodWarning; // to figure out which cases are supported by R#

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
    [TestNetFramework46]
    public void TestEmptyArrayInitialization() => DoNamedTest2();
}