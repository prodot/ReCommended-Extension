using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes;

public abstract class BaseTypeAnalyzerTests<T> : CSharpHighlightingTestBase
{
    protected abstract T[] TestValues { get; }

    protected void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    protected void Test<R>(Func<T, R> expected, Func<T, R> actual, T[]? values = null)
    {
        foreach (var value in values ?? TestValues)
        {
            Assert.AreEqual(expected(value), actual(value));
        }
    }

    protected void Test<U, R>(Func<T, U, R> expected, Func<T, U, R> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y), actual(x, y));
            }
        }
    }

    protected delegate R FuncWithOut<O, out R>(T arg1, out O arg2);

    protected void Test(FuncWithOut<T, bool> expected, FuncWithOut<T, bool> actual, T[]? values = null)
    {
        foreach (var value in values ?? TestValues)
        {
            Assert.AreEqual(expected(value, out var expectedResult), actual(value, out var actualResult));
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}