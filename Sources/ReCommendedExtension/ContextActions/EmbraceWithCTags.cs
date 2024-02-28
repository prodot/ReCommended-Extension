﻿using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Embrace the word or selection with <c>...</c> in XML doc comments" + ZoneMarker.Suffix,
    Description = "Embraces the word or selection with <c>...</c> in XML doc comments.")]
public sealed class EmbraceWithCTags(ICSharpContextActionDataProvider provider) : EncompassInDocComment(provider)
{
    protected override string Encompass(string text) => $"<c>{text}</c>";

    public override string Text => "Embrace with <c>...</c>";
}