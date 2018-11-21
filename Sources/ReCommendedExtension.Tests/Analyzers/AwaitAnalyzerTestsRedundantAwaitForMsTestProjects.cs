using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using NUnit.Framework;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantAwaitForMsTestProjects : CSharpHighlightingTestBase
    {
        protected override IEnumerable<string> GetReferencedAssemblies(TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(targetFrameworkId) ?? Enumerable.Empty<string>()).EnsureMsTestAssembly();

        protected override string RelativeTestDataPath => @"Analyzers\Await";

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
            => highlighting is RedundantAwaitHighlighting;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected override void DoTest(IProject project)
        {
            project.PatchProjectAddMsTestProjectFlavor();

            base.DoTest(project);
        }

        [Test]
        public void TestRedundantAwait_TestProject() => DoNamedTest2();
    }
}