﻿using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.ContextActions.Strings;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = $"Replace 'string.{nameof(string.IsNullOrEmpty)}(text)' with 'text is not [_, ..]'" + ZoneMarker.Suffix,
    Description = $"Replaces 'string.{nameof(string.IsNullOrEmpty)}(text)' with 'text is not [_, ..]'.")]
public sealed class ReplaceIsNullOrEmptyWithNegatedListPattern(ICSharpContextActionDataProvider provider) : ReplaceIsNullOrEmpty(provider)
{
    private protected override string ReplacementPattern => "not [_, ..]";

    protected override bool IsAvailable(IInvocationExpression selectedElement)
        => selectedElement.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 && base.IsAvailable(selectedElement);
}