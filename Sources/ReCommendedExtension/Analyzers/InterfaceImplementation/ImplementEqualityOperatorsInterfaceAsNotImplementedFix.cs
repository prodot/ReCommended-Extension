using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[QuickFix]
public sealed class ImplementEqualityOperatorsInterfaceAsNotImplementedFix : ImplementEqualityOperatorsInterfaceFix
{
    public ImplementEqualityOperatorsInterfaceAsNotImplementedFix(ImplementEqualityOperatorsSuggestion highlighting) : base(highlighting) { }

    public override bool IsAvailable(IUserDataHolder cache) => !Highlighting.OperatorsAvailable;

    public override string Text
    {
        get
        {
            var type = Highlighting.Declaration.DeclaredName;

            return $"Implement IEqualityOperators<{type}, {type}, bool>";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            AddBaseInterfaceDeclaration();

            Debug.Assert(Highlighting.Declaration.DeclaredElement is { });

            var factory = CSharpElementFactory.GetInstance(Highlighting.Declaration);

            var name = Highlighting.Declaration switch
            {
                IClassDeclaration => Highlighting.Declaration.DeclaredElement.ShortName
                    + (Highlighting.Declaration.IsNullableAnnotationsContextEnabled() ? "?" : ""),

                IStructDeclaration => Highlighting.Declaration.DeclaredElement.ShortName,

                _ => throw new NotSupportedException(),
            };

            var equalityOperator = Highlighting.Declaration.AddClassMemberDeclaration(
                (IOperatorDeclaration)factory.CreateTypeMemberDeclaration(
                    $"public static bool operator ==({name} left, {name} right) => throw new NotImplementedException();"));

            var inequalityOperator = Highlighting.Declaration.AddClassMemberDeclaration(
                (IOperatorDeclaration)factory.CreateTypeMemberDeclaration(
                    $"public static bool operator !=({name} left, {name} right) => !(left == right);"));

            ContextActionUtils.FormatWithDefaultProfile(equalityOperator);
            ContextActionUtils.FormatWithDefaultProfile(inequalityOperator);
        }

        return _ => { };
    }
}