﻿using JetBrains.Application.Progress;
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
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class PassSingleCharacterFix(PassSingleCharacterSuggestion highlighting) : QuickFixBase
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

            if (highlighting.RedundantArguments is [_, ..])
            {
                for (var i = highlighting.RedundantArguments.Length - 1; i >= 0; i--)
                {
                    var redundantArgument = highlighting.RedundantArguments[i];

                    if (redundantArgument
                            .PrevTokens()
                            .TakeWhile(t => t.Parent == redundantArgument.Parent)
                            .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
                    {
                        ModificationUtil.DeleteChildRange(commaToken, redundantArgument);
                    }
                    else
                    {
                        ModificationUtil.DeleteChild(redundantArgument);
                    }
                }
            }
        }

        return _ => { };
    }
}