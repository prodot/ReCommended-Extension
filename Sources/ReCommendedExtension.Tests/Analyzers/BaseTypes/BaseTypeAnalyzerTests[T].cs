using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes;

public abstract class BaseTypeAnalyzerTests<T> : CSharpHighlightingTestBase
{
    protected abstract T[] TestValues { get; }

    protected void Test<R>(Func<T, R> expected, Func<T, R> actual, T[]? values = null)
    {
        foreach (var value in values ?? TestValues)
        {
            Assert.AreEqual(expected(value), actual(value), $"with values: {value}");
        }
    }
}