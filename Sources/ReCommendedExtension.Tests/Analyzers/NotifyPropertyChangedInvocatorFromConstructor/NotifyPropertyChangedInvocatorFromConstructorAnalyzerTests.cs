using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
public sealed class NotifyPropertyChangedInvocatorFromConstructorAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is NotifyPropertyChangedInvocatorFromConstructorWarning;

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
}