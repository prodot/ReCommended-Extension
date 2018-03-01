using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework46]
    [TestFixture]
    public sealed class EmptyArrayInitializationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\EmptyArrayInitialization";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is EmptyArrayInitializationHighlighting;

        [Test]
        [SuppressMessage("ReSharper", "EmptyArrayInitialization")]
        public void Compiler()
        {
            int[] ParamsArray(params int[] items) => items;

            Assert.AreSame(Array.Empty<int>(), ParamsArray());

            int[] array = { };
            Assert.AreNotSame(Array.Empty<int>(), array);

            Assert.AreNotSame(Array.Empty<int>(), new int[] { });
            Assert.AreNotSame(Array.Empty<int>(), new int[0]);

            Assert.AreNotSame(Array.Empty<int[]>(), new int[][] { }); // not yet supported
            Assert.AreNotSame(Array.Empty<int[]>(), new int[0][]); // not yet supported
            Assert.AreNotSame(Array.Empty<int[,]>(), new int[][,] { }); // not yet supported
            Assert.AreNotSame(Array.Empty<int[,]>(), new int[0][,]); // not yet supported
        }

        [Test]
        public void TestEmptyArrayInitialization() => DoNamedTest2();
    }
}