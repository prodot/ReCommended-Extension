using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class PassSingleCharactersFix(PassSingleCharactersSuggestion highlighting) : QuickFixBase
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