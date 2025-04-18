using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers.CatchClauseWithoutVariable;

[TestFixture]
public sealed class CatchClauseWithoutVariableQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariableQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is CatchClauseWithoutVariableHint or NotResolvedError;

    [Test]
    public void TestCatchClauseWithoutVariableAvailability() => DoNamedTest2();
}