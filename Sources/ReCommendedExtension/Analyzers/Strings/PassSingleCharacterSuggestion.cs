using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass the single character" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassSingleCharacterSuggestion(
    string message,
    ICSharpArgument argument,
    string? parameterName,
    char character,
    string? additionalArgument = null,
    ICSharpArgument? redundantArgument = null) : Highlighting(message)
{
    const string SeverityId = "PassSingleCharacter";

    internal ICSharpArgument Argument => argument;

    internal string? ParameterName => parameterName;

    internal char Character => character;

    internal string? AdditionalArgument => additionalArgument;

    internal ICSharpArgument? RedundantArgument => redundantArgument;

    public override DocumentRange CalculateRange() => argument.Value.GetDocumentRange();
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass the single characters" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassSingleCharactersSuggestion : MultipleHighlightings // todo: move to a separate file
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

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use expression result" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseExpressionResultSuggestion(string message, IInvocationExpression invocationExpression, string replacement) : Highlighting(
    message) // todo: move to a separate file
{
    const string SeverityId = "UseExpressionResult";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal string Replacement => replacement;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}

public enum ListPatternSuggestionKind // todo: move to a separate file
{
    FirstCharacter,
    NotFirstCharacter,
    LastCharacter,
    NotLastCharacter,
}

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use list pattern" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStringListPatternSuggestion : Highlighting // todo: move to a separate file
{
    const string SeverityId = "UseStringListPattern";

    UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        IBinaryExpression? binaryExpression) : base(message)
    {
        InvocationExpression = invocationExpression;
        InvokedExpression = invokedExpression;
        Kind = kind;
        BinaryExpression = binaryExpression;
    }

    public UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        char[] characters,
        IBinaryExpression? binaryExpression = null) : this(message, invocationExpression, invokedExpression, kind, binaryExpression)
        => Characters = characters;

    public UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        string valueArgument,
        IBinaryExpression? binaryExpression = null) : this(message, invocationExpression, invokedExpression, kind, binaryExpression)
        => ValueArgument = valueArgument;

    internal IInvocationExpression InvocationExpression { get; }

    internal IReferenceExpression InvokedExpression { get; }

    internal IBinaryExpression? BinaryExpression { get; }

    internal char[]? Characters { get; }

    internal ListPatternSuggestionKind Kind { get; }

    internal string? ValueArgument { get; }

    public override DocumentRange CalculateRange()
    {
        var startOffset = InvokedExpression.Reference.GetDocumentRange().StartOffset;

        return (BinaryExpression as ITreeNode ?? InvocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.BestPractice, "Use another method" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherMethodSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string otherMethodName,
    bool isNegated,
    string[] arguments,
    IBinaryExpression? binaryExpression = null) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "UseOtherMethod";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal string OtherMethodName => otherMethodName;

    internal bool IsNegated => isNegated;

    internal string[] Arguments => arguments;

    internal IBinaryExpression? BinaryExpression => binaryExpression;

    public override DocumentRange CalculateRange()
    {
        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return (BinaryExpression as ITreeNode ?? invocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The argument is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantArgumentHint(string message, ICSharpArgument argument) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "RedundantArgument";

    internal ICSharpArgument Argument => argument;

    public override DocumentRange CalculateRange() => argument.GetDocumentRange();
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The element is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantElementHint(string message, IInitializerElement element) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "RedundantElement";

    internal IInitializerElement Element => element;

    public override DocumentRange CalculateRange() => element.GetDocumentRange();
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Method invocation is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantMethodInvocationHint(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "RedundantMethodInvocation";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    public override DocumentRange CalculateRange()
    {
        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return invocationExpression.GetDocumentRange().SetStartTo(startOffset);
    }
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use the string property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStringPropertySuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string propertyName) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "UseStringProperty";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal string PropertyName => propertyName;

    public override DocumentRange CalculateRange()
        => invocationExpression.GetDocumentRange().SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset);
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use the range indexer" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseRangeIndexerSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string startIndexArgument,
    string endIndexArgument) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "UseRangeIndexer";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal string StartIndexArgument => startIndexArgument;

    internal string EndIndexArgument => endIndexArgument;

    public override DocumentRange CalculateRange()
        => invocationExpression.GetDocumentRange().SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset);
}

[QuickFix]
public sealed class PassSingleCharacterFix(PassSingleCharacterSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => highlighting.Character.IsPrintable() ? $"Replace with '{highlighting.Character}'" : "Replace with the single character'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Argument);

            var argument = ModificationUtil.ReplaceChild(
                highlighting.Argument,
                factory.CreateArgument(ParameterKind.UNKNOWN, highlighting.ParameterName, factory.CreateExpression($"'{highlighting.Character}'")));

            if (highlighting.AdditionalArgument is { })
            {
                var comma = ModificationUtil.AddChildAfter(argument, CSharpTokenType.COMMA.CreateTreeElement());
                ModificationUtil.AddChildAfter(
                    comma,
                    factory.CreateArgument(ParameterKind.UNKNOWN, factory.CreateExpression(highlighting.AdditionalArgument)));
            }

            if (highlighting.RedundantArgument is { })
            {
                if (highlighting
                        .RedundantArgument.PrevTokens()
                        .TakeWhile(t => t.Parent == highlighting.RedundantArgument.Parent)
                        .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
                {
                    ModificationUtil.DeleteChildRange(commaToken, highlighting.RedundantArgument);
                }
                else
                {
                    ModificationUtil.DeleteChild(highlighting.RedundantArgument);
                }
            }
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class PassSingleCharactersFix(PassSingleCharactersSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => highlighting.Characters.All(c => c.IsPrintable())
            ? $"Replace with {string.Join(", ", from c in highlighting.Characters select $"'{c}'").TrimToSingleLineWithMaxLength(120)}, respectively"
            : "Replace with the single characters";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            switch (highlighting)
            {
                case { Arguments: { } arguments, ParameterNames: { } parameterNames }:
                {
                    Debug.Assert(arguments is [_, _, ..]);
                    Debug.Assert(arguments.Length == parameterNames.Length);
                    Debug.Assert(arguments.Length == highlighting.Characters.Length);

                    var factory = CSharpElementFactory.GetInstance(arguments[0]);

                    for (var i = 0; i < arguments.Length; i++)
                    {
                        ModificationUtil.ReplaceChild(
                            arguments[i],
                            factory.CreateArgument(
                                ParameterKind.UNKNOWN,
                                parameterNames[i],
                                factory.CreateExpression($"'{highlighting.Characters[i]}'")));
                    }

                    if (highlighting.RedundantArgument is { })
                    {
                        if (highlighting
                                .RedundantArgument.PrevTokens()
                                .TakeWhile(t => t.Parent == highlighting.RedundantArgument.Parent)
                                .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
                        {
                            ModificationUtil.DeleteChildRange(commaToken, highlighting.RedundantArgument);
                        }
                        else
                        {
                            ModificationUtil.DeleteChild(highlighting.RedundantArgument);
                        }
                    }

                    break;
                }

                case { CollectionExpressionElements: { } collectionExpressionElements }:
                {
                    Debug.Assert(collectionExpressionElements.Length == highlighting.Characters.Length);

                    var factory = CSharpElementFactory.GetInstance(collectionExpressionElements[0]);

                    for (var i = 0; i < collectionExpressionElements.Length; i++)
                    {
                        ModificationUtil.ReplaceChild(
                            collectionExpressionElements[i],
                            factory.CreateCollectionExpressionElement(factory.CreateExpression($"'{highlighting.Characters[i]}'")));
                    }

                    break;
                }

                case { ArrayCreationExpression: { } arrayCreationExpression }:
                {
                    Debug.Assert(arrayCreationExpression.ArrayInitializer is { ElementInitializers: [_, ..] });

                    var factory = CSharpElementFactory.GetInstance(arrayCreationExpression);

                    var items = string.Join(", ", from c in highlighting.Characters select $"'{c}'");

                    ModificationUtil.ReplaceChild(arrayCreationExpression, factory.CreateExpression($$"""new[] { {{items}} }"""));

                    break;
                }
            }
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class UseExpressionResultFix(UseExpressionResultSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Replacement.TrimToSingleLineWithMaxLength(120)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil.ReplaceChild(highlighting.InvocationExpression, factory.CreateExpression($"{highlighting.Replacement}"));
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class UseStringListPatternFix(UseStringListPatternSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    string GetReplacement(bool forUI = false)
    {
        switch (highlighting)
        {
            case { Characters: { } characters }:
                var pattern = !forUI || characters.All(c => c.IsPrintable())
                    ? string.Join(" or ", from c in characters select $"'{c}'")
                    : "<pattern>";

                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [{pattern}, ..]",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [{pattern}, ..]",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., {pattern}]",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., {pattern}]",

                    _ => throw new NotSupportedException(),
                };

            case { ValueArgument: { } valueArgument }:
                if (forUI)
                {
                    valueArgument = valueArgument.TrimToSingleLineWithMaxLength(120);
                }

                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [var firstCharacter, ..] && firstCharacter == {valueArgument}",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [var firstCharacter, ..] || firstCharacter != {valueArgument}",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., var lastCharacter] && lastCharacter == {valueArgument}",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., var lastCharacter] || lastCharacter != {valueArgument}",

                    _ => throw new NotSupportedException(),
                };

            default: throw new NotSupportedException();
        }
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{GetReplacement(true)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {GetReplacement()})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class UseOtherMethodFix(UseOtherMethodSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var negation = highlighting.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.Arguments);

            return $"Replace with '{negation}{highlighting.OtherMethodName}({arguments.TrimToSingleLineWithMaxLength(120)})'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var negation = highlighting.IsNegated ? "!" : "";
            var arguments = string.Join(", ", highlighting.Arguments);

            ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression(
                        $"({negation}$0.{highlighting.OtherMethodName}({arguments}))",
                        highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class RemoveArgumentFix(RedundantArgumentHint highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove argument";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            if (highlighting
                    .Argument.PrevTokens()
                    .TakeWhile(t => t.Parent == highlighting.Argument.Parent)
                    .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
            {
                ModificationUtil.DeleteChildRange(commaToken, highlighting.Argument);
            }
            else
            {
                ModificationUtil.DeleteChild(highlighting.Argument);
            }
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class RemoveElementFix(RedundantElementHint highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove element";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            if (highlighting
                    .Element.PrevTokens()
                    .TakeWhile(t => t.Parent == highlighting.Element.Parent)
                    .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
            {
                ModificationUtil.DeleteChildRange(commaToken, highlighting.Element);
            }
            else
            {
                ModificationUtil.DeleteChild(highlighting.Element);
            }
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class UseStringPropertyFix(UseStringPropertySuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.PropertyName}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression($"$0.{highlighting.PropertyName}", highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class RemoveMethodInvocationFix(RedundantMethodInvocationHint highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove method invocation";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil
                .ReplaceChild(highlighting.InvocationExpression, factory.CreateExpression("($0)", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}

[QuickFix]
public sealed class UseRangeIndexerFix(UseRangeIndexerSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
        => $"Replace with '[{highlighting.StartIndexArgument.TrimToSingleLineWithMaxLength(120)}..{highlighting.EndIndexArgument.TrimToSingleLineWithMaxLength(120)}]'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var startIndex = highlighting.StartIndexArgument != "" ? $"({highlighting.StartIndexArgument})" : "";
            var endIndex = highlighting.EndIndexArgument != "" ? $"({highlighting.EndIndexArgument})" : "";

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"$0[{startIndex}..{endIndex}]", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveRangeIndexParentheses(factory);
        }

        return _ => { };
    }
}