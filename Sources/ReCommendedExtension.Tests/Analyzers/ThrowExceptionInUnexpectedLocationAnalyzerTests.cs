using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [TestNet50]
    public sealed class ThrowExceptionInUnexpectedLocationAnalyzerTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => @"Analyzers\ThrowExceptionInUnexpectedLocation";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is ThrowExceptionInUnexpectedLocationWarning;

        [Test]
        public void TestThrowExceptionInUnexpectedLocation() => DoNamedTest2();

        [Test]
        [NullableContext(NullableContextKind.Enable)]
        public void TestThrowExceptionInUnexpectedLocation_NullableAnnotationContext() => DoNamedTest2();
    }
}