﻿using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.ValueTask
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Possible multiple consumption of 'ValueTask' or 'ValueTask<T>'" + ZoneMarker.Suffix,
        "Possible multiple consumption of 'ValueTask' or 'ValueTask<T>'",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class PossibleMultipleConsumptionWarning : Highlighting
    {
        const string SeverityId = "PossibleMultipleConsumption";

        [NotNull]
        readonly ICSharpExpression usage;

        internal PossibleMultipleConsumptionWarning([NotNull] string message, [NotNull] ICSharpExpression usage) : base(message)
            => this.usage = usage;

        public override DocumentRange CalculateRange() => usage.GetHighlightingRange();
    }
}