using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.UnthrowableException;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class UnthrowableExceptionAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\UnthrowableException";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is UnthrowableExceptionWarning;

        [Test]
        public void TestUnthrowableException() => DoNamedTest2();
    }
}