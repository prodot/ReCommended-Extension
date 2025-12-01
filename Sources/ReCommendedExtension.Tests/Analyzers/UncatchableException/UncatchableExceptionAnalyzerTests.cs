using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.UncatchableException;

namespace ReCommendedExtension.Tests.Analyzers.UncatchableException;

[TestFixture]
public sealed class UncatchableExceptionAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\UncatchableException";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UncatchableExceptionWarning;

    [Test]
    public void TestUncatchableException() => DoNamedTest2();
}