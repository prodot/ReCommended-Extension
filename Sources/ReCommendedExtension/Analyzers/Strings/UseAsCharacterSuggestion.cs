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

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.BestPractice, "Use character" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseAsCharacterSuggestion(
    string message,
    ICSharpArgument argument,
    string? parameterName,
    char character,
    string? additionalArgument = null,
    ICSharpArgument? redundantArgument = null) : Highlighting(message)
{
    const string SeverityId = "UseAsCharacter";

    internal ICSharpArgument Argument { get; } = argument;

    internal string? ParameterName { get; } = parameterName;

    internal char Character { get; } = character;

    internal string? AdditionalArgument { get; } = additionalArgument;
    
    internal ICSharpArgument? RedundantArgument { get; } = redundantArgument;

    public override DocumentRange CalculateRange() => Argument.Value.GetDocumentRange();
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

    internal IInvocationExpression InvocationExpression { get; } = invocationExpression;

    internal string Replacement { get; } = replacement;

    public override DocumentRange CalculateRange() => InvocationExpression.GetDocumentRange();
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

    internal IInvocationExpression InvocationExpression { get; } = invocationExpression;

    internal IReferenceExpression InvokedExpression { get; } = invokedExpression;

    internal string OtherMethodName { get; } = otherMethodName;

    internal bool IsNegated { get; } = isNegated;

    internal string[] Arguments { get; } = arguments;

    internal IBinaryExpression? BinaryExpression { get; } = binaryExpression;

    public override DocumentRange CalculateRange()
    {
        var startOffset = InvokedExpression.Reference.GetDocumentRange().StartOffset;

        return (BinaryExpression as ITreeNode ?? InvocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Argument is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantArgumentSuggestion(string message, ICSharpArgument argument) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "RedundantArgument";

    internal ICSharpArgument Argument { get; } = argument;

    public override DocumentRange CalculateRange() => Argument.GetDocumentRange();
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
public sealed class RedundantMethodInvocationSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression) : Highlighting(message) // todo: move to a separate file
{
    const string SeverityId = "RedundantMethodInvocation";

    internal IInvocationExpression InvocationExpression { get; } = invocationExpression;

    internal IReferenceExpression InvokedExpression { get; } = invokedExpression;

    public override DocumentRange CalculateRange()
    {
        var startOffset = InvokedExpression.Reference.GetDocumentRange().StartOffset;

        return InvocationExpression.GetDocumentRange().SetStartTo(startOffset);
    }
}

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use string property" + ZoneMarker.Suffix,
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

    internal IInvocationExpression InvocationExpression { get; } = invocationExpression;

    internal IReferenceExpression InvokedExpression { get; } = invokedExpression;

    internal string PropertyName { get; } = propertyName;

    public override DocumentRange CalculateRange()
        => InvocationExpression.GetDocumentRange().SetStartTo(InvokedExpression.Reference.GetDocumentRange().StartOffset);
}

[QuickFix]
public sealed class UseAsCharacterFix(UseAsCharacterSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Character}'";

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
public sealed class UseExpressionResultFix(UseExpressionResultSuggestion highlighting) : QuickFixBase // todo: move to a separate file
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Replacement}'";

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
    string Replacement
    {
        get
        {
            switch (highlighting)
            {
                case { Characters: { } characters }:
                    var pattern = string.Join(" or ", from c in characters select $"'{c}'");

                    return highlighting.Kind switch
                    {
                        ListPatternSuggestionKind.FirstCharacter => $"is [{pattern}, ..]",
                        ListPatternSuggestionKind.NotFirstCharacter => $"is not [{pattern}, ..]",
                        ListPatternSuggestionKind.LastCharacter => $"is [.., {pattern}]",
                        ListPatternSuggestionKind.NotLastCharacter => $"is not [.., {pattern}]",

                        _ => throw new NotSupportedException(),
                    };

                case { ValueArgument: { } valueArgument }:
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
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{Replacement}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {Replacement})", highlighting.InvokedExpression.QualifierExpression))
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

            return $"Replace with '{negation}{highlighting.OtherMethodName}({arguments})'";
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
public sealed class RemoveArgumentFix(RedundantArgumentSuggestion highlighting) : QuickFixBase // todo: move to a separate file
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
public sealed class RemoveMethodInvocationFix(RedundantMethodInvocationSuggestion highlighting) : QuickFixBase // todo: move to a separate file
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