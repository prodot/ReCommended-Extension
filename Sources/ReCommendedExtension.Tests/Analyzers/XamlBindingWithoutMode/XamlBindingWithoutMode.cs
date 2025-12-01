using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.XamlBindingWithoutMode;

namespace ReCommendedExtension.Tests.Analyzers.XamlBindingWithoutMode;

[TestFixture]
public sealed class XamlBindingWithoutMode : XamlAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\XamlBindingWithoutMode";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is XamlBindingWithoutModeWarning;

    [Test]
    public void TestXamlBindingWithoutMode() => DoNamedTest2();
}