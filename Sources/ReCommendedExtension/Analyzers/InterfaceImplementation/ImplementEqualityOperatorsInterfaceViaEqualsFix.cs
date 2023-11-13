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
public sealed class ImplementEqualityOperatorsInterfaceViaEqualsFix : ImplementEqualityOperatorsInterfaceFix
{
    public ImplementEqualityOperatorsInterfaceViaEqualsFix(ImplementEqualityOperatorsSuggestion highlighting) : base(highlighting) { }

    public override bool IsAvailable(IUserDataHolder cache) => !Highlighting.OperatorsAvailable;

    public override string Text
    {
        get
        {
            const string method = nameof(IEquatable<int>.Equals);

            var type = Highlighting.Declaration.DeclaredName;

            return $"Implement IEqualityOperators<{type}, {type}, bool> (use the {method} method)";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            AddBaseInterfaceDeclaration();

            Debug.Assert(Highlighting.Declaration.DeclaredElement is { });

            var factory = CSharpElementFactory.GetInstance(Highlighting.Declaration);

            string name;
            string body;

            switch (Highlighting.Declaration)
            {
                case IClassDeclaration:
                    var isNullableAnnotationsContextEnabled = Highlighting.Declaration.IsNullableAnnotationsContextEnabled();

                    name = Highlighting.Declaration.DeclaredElement.ShortName + (isNullableAnnotationsContextEnabled ? "?" : "");
                    body = $"(object{(isNullableAnnotationsContextEnabled ? "?" : "")})left == right || left is {{ }} && left.Equals(right);";
                    break;

                case IStructDeclaration:
                    name = Highlighting.Declaration.DeclaredElement.ShortName;
                    body = "left.Equals(right);";
                    break;

                default: throw new NotSupportedException();
            }

            var equalityOperator = Highlighting.Declaration.AddClassMemberDeclaration(
                (IOperatorDeclaration)factory.CreateTypeMemberDeclaration($"public static bool operator ==({name} left, {name} right) => {body}"));

            var inequalityOperator = Highlighting.Declaration.AddClassMemberDeclaration(
                (IOperatorDeclaration)factory.CreateTypeMemberDeclaration(
                    $"public static bool operator !=({name} left, {name} right) => !(left == right);"));

            ContextActionUtils.FormatWithDefaultProfile(equalityOperator);
            ContextActionUtils.FormatWithDefaultProfile(inequalityOperator);
        }

        return _ => { };
    }
}