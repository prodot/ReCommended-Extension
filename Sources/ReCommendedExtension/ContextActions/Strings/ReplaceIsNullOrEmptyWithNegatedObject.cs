using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.ContextActions.Strings;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = $$"""Replace 'string.{{nameof(string.IsNullOrEmpty)}}(text)' with 'text is not { {{nameof(string.Length)}}: > 0 }'""" + ZoneMarker.Suffix,
    Description = $$"""Replaces 'string.{{nameof(string.IsNullOrEmpty)}}(text)' with 'text is not { {{nameof(string.Length)}}: > 0 }'.""")]
public sealed class ReplaceIsNullOrEmptyWithNegatedObject(ICSharpContextActionDataProvider provider) : ReplaceIsNullOrEmpty(provider)
{
    private protected override string ReplacementPattern => $$"""not { {{nameof(string.Length)}}: > 0 }""";

    protected override bool IsAvailable(IInvocationExpression selectedElement)
        => selectedElement.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90 && base.IsAvailable(selectedElement);
}