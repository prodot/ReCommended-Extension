using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument;

namespace ReCommendedExtension.Tests.Analyzers.ArgumentExceptionConstructorArgument;

[TestFixture]
public sealed class ArgumentExceptionConstructorArgumentAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ArgumentExceptionConstructorArgument";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ArgumentExceptionConstructorArgumentWarning;

    [Test]
    public void TestArgumentExceptionConstructorArgument() => DoNamedTest2();
}