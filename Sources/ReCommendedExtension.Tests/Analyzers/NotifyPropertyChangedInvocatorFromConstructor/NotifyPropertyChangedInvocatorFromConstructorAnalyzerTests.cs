using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
public sealed class NotifyPropertyChangedInvocatorFromConstructorAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is NotifyPropertyChangedInvocatorFromConstructorWarning or NotResolvedError;

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
}