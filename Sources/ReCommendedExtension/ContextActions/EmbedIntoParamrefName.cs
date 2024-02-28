﻿using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = """Embed the word or selection into <paramref name="..."/> in XML doc comments""" + ZoneMarker.Suffix,
    Description = """Embed the word or selection into <paramref name="..."/> in XML doc comments.""")]
public sealed class EmbedIntoParamrefName(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text) => $"""<paramref name="{text}"/>""";

    public override string Text => """Embed into <paramref name="..."/>""";
}