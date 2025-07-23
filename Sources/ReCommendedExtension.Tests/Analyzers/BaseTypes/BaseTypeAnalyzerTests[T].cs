using System.Globalization;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using NUnit.Framework;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes;

public abstract class BaseTypeAnalyzerTests<T> : CSharpHighlightingTestBase
{
    protected abstract T[] TestValues { get; }

    protected IFormatProvider?[] FormatProviders { get; } = [null, CultureInfo.InvariantCulture, new CultureInfo("en-US"), new CultureInfo("de-DE")];

    protected void Test<R>(Func<R> expected, Func<R> actual) => Assert.AreEqual(expected(), actual());

    protected void Test<R>(Func<T, R> expected, Func<T, R> actual, T[]? values = null)
    {
        foreach (var value in values ?? TestValues)
        {
            Assert.AreEqual(expected(value), actual(value), $"with values: {value}");
        }
    }

    protected void Test<U, R>(Func<T, U, R> expected, Func<T, U, R> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y), actual(x, y), $"with values: {x}, {y}");
            }
        }
    }

    protected void Test<U, V, R>(Func<T, U, V, R> expected, Func<T, U, V, R> actual, T[] xValues, U[] yValues, V[] zValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    Assert.AreEqual(expected(x, y, z), actual(x, y, z), $"with values: {x}, {y}, {z}");
                }
            }
        }
    }

    protected void Test<U, V, W, R>(Func<T, U, V, W, R> expected, Func<T, U, V, W, R> actual, T[] xValues, U[] yValues, V[] zValues, W[] z1Values)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    foreach (var z1 in z1Values)
                    {
                        Assert.AreEqual(expected(x, y, z, z1), actual(x, y, z, z1), $"with values: {x}, {y}, {z}, {z1}");
                    }
                }
            }
        }
    }

    protected delegate R FuncWithOut<O, out R>(T arg1, out O arg2);

    protected void Test(FuncWithOut<T, bool> expected, FuncWithOut<T, bool> actual, T[]? values = null)
    {
        foreach (var value in values ?? TestValues)
        {
            Assert.AreEqual(expected(value, out var expectedResult), actual(value, out var actualResult), $"with values: {value}");
            Assert.AreEqual(expectedResult, actualResult, $"with values: {value}");
        }
    }

    protected delegate R FuncWithOut<in U, O, out R>(T arg1, U arg2, out O arg3);

    protected void Test<U>(FuncWithOut<U, T, bool> expected, FuncWithOut<U, T, bool> actual, T[] xValues, U[] yValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                Assert.AreEqual(expected(x, y, out var expectedResult), actual(x, y, out var actualResult), $"with values: {x}, {y}");
                Assert.AreEqual(expectedResult, actualResult, $"with values: {x}, {y}");
            }
        }
    }

    protected delegate R FuncWithOut<in U, in V, O, out R>(T arg1, U arg2, V arg3, out O arg4);

    protected void Test<U, V>(FuncWithOut<U, V, T, bool> expected, FuncWithOut<U, V, T, bool> actual, T[] xValues, U[] yValues, V[] zValues)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    Assert.AreEqual(expected(x, y, z, out var expectedResult), actual(x, y, z, out var actualResult), $"with values: {x}, {y}, {z}");
                    Assert.AreEqual(expectedResult, actualResult, $"with values: {x}, {y}, {z}");
                }
            }
        }
    }

    protected delegate R FuncWithOut<in U, in V, in W, O, out R>(T arg1, U arg2, V arg3, W arg4, out O arg5);

    protected void Test<U, V, W>(
        FuncWithOut<U, V, W, T, bool> expected,
        FuncWithOut<U, V, W, T, bool> actual,
        T[] xValues,
        U[] yValues,
        V[] zValues,
        W[] z1Values)
    {
        foreach (var x in xValues)
        {
            foreach (var y in yValues)
            {
                foreach (var z in zValues)
                {
                    foreach (var z1 in z1Values)
                    {
                        Assert.AreEqual(
                            expected(x, y, z, z1, out var expectedResult),
                            actual(x, y, z, z1, out var actualResult),
                            $"with values: {x}, {y}, {z}, {z1}");
                        Assert.AreEqual(expectedResult, actualResult, $"with values: {x}, {y}, {z}, {z1}");
                    }
                }
            }
        }
    }
}