using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = """Embed the word or selection into <typeparamref name="..."/> in XML doc comments""" + ZoneMarker.Suffix,
    Description = """Embed the word or selection into <typeparamref name="..."/> in XML doc comments.""")]
public sealed class EmbedIntoTypeparamrefName(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text) => $"""<typeparamref name="{text}"/>""";

    public override string Text => """Embed into <typeparamref name="..."/>""";
}