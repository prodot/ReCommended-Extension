using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass the single characters" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassSingleCharactersSuggestion : MultipleHighlightings
{
    const string SeverityId = "PassSingleCharacters";

    PassSingleCharactersSuggestion(string message, char[] characters) : base(message) => Characters = characters;

    public PassSingleCharactersSuggestion(
        string message,
        ICSharpArgument[] arguments,
        string?[] parameterNames,
        char[] characters,
        ICSharpArgument? redundantArgument = null) : this(message, characters)
    {
        Arguments = arguments;
        ParameterNames = parameterNames;
        RedundantArgument = redundantArgument;
    }

    public PassSingleCharactersSuggestion(string message, ICollectionExpressionElement[] collectionExpressionElements, char[] characters) : this(
        message,
        characters)
        => CollectionExpressionElements = collectionExpressionElements;

    public PassSingleCharactersSuggestion(string message, IArrayCreationExpression arrayCreationExpression, char[] characters) : this(
        message,
        characters)
        => ArrayCreationExpression = arrayCreationExpression;

    internal ICSharpArgument[]? Arguments { get; }

    internal string?[]? ParameterNames { get; }

    internal ICollectionExpressionElement[]? CollectionExpressionElements { get; }

    internal IArrayCreationExpression? ArrayCreationExpression { get; }

    internal char[] Characters { get; }

    internal ICSharpArgument? RedundantArgument { get; }
}