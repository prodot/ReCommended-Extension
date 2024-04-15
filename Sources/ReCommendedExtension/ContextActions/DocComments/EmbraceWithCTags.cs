using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions.DocComments;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Embrace the word or selection with <c>...</c> in XML doc comments" + ZoneMarker.Suffix,
    Description = "Embraces the word or selection with <c>...</c> in XML doc comments.")]
public sealed class EmbraceWithCTags(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text, Settings settings) => BuildTag("c", text, TagOption.Expanded, settings);

    public override string Text => "Embrace with <c>...</c>";
}