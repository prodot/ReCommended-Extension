using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions.DocComments;

[ContextAction(
    Group = "C#",
    Name = """Embed the word or selection into <see cref="..."/> in XML doc comments""" + ZoneMarker.Suffix,
    Description = """Embed the word or selection into <see cref="..."/> in XML doc comments.""")]
public sealed class EmbedIntoSeeCRef(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text, Settings settings) => BuildTag("see", ("cref", text), null, TagOption.Collapsed, settings);

    public override string Text => """Embed into <see cref="..."/>""";
}