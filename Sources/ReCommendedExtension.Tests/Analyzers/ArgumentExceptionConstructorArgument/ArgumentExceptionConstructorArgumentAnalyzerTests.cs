using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument;

namespace ReCommendedExtension.Tests.Analyzers.ArgumentExceptionConstructorArgument;

[TestFixture]
public sealed class ArgumentExceptionConstructorArgumentAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ArgumentExceptionConstructorArgument";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is ArgumentExceptionConstructorArgumentWarning;

    [Test]
    public void TestArgumentExceptionConstructorArgument() => DoNamedTest2();
}