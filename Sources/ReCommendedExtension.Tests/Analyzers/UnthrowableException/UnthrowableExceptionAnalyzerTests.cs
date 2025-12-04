using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.UnthrowableException;

namespace ReCommendedExtension.Tests.Analyzers.UnthrowableException;

[TestFixture]
public sealed class UnthrowableExceptionAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\UnthrowableException";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UnthrowableExceptionWarning;

    [Test]
    public void TestUnthrowableException() => DoNamedTest2();
}