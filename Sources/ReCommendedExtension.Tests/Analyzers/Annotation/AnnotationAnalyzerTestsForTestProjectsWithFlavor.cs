using System.Reflection;
using JetBrains.Application.Settings;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AnnotationAnalyzerTestsForTestProjectsWithFlavor : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is MissingSuppressionJustificationWarning or NotResolvedError;

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    protected override void DoTest(Lifetime lifetime, IProject project)
    {
        // patch the project type guids (applying [TestFlavours("3AC096D0-A1C2-E12C-1390-A8335801FDAB")] doesn't work)

        var projectTypeGuids = project.ProjectProperties.ProjectTypeGuids.ToHashSet();
        if (projectTypeGuids.Add(MsTestProjectFlavor.MsTestProjectFlavorGuid))
        {
            var field = project.ProjectProperties.GetType().BaseType.GetField("myProjectTypeGuids", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(project.ProjectProperties, projectTypeGuids);
        }

        Assert.True(project.HasFlavour<MsTestProjectFlavor>());

        base.DoTest(lifetime, project);
    }

    [Test]
    public void TestSuppressMessage_TestProject() => DoNamedTest2();

    [Test]
    [TestNet50]
    public void TestSuppressMessage_TestProject_NET_5() => DoNamedTest2();
}