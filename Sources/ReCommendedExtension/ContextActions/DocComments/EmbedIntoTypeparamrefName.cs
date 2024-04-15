using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions.DocComments;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = """Embed the word or selection into <typeparamref name="..."/> in XML doc comments""" + ZoneMarker.Suffix,
    Description = """Embed the word or selection into <typeparamref name="..."/> in XML doc comments.""")]
public sealed class EmbedIntoTypeparamrefName(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text, Settings settings)
        => BuildTag("typeparamref", ("name", text), null, TagOption.Collapsed, settings);

    public override string Text => """Embed into <typeparamref name="..."/>""";
}