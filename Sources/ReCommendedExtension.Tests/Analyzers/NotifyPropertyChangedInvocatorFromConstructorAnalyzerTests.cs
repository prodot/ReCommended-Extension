using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorAnalyzerTests : HighlightingTestBaseWithAnnotationAssemblyReference
    {
        protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
            => highlighting is NotifyPropertyChangedInvocatorFromConstructorHighlighting;

        [Test]
        public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
    }
}