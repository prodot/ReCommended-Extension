using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InternalConstructor;

[ElementProblemAnalyzer(typeof(IConstructorDeclaration), HighlightingTypes = [typeof(InternalConstructorVisibilitySuggestion)])]
public sealed class InternalConstructorVisibilityAnalyzer : ElementProblemAnalyzer<IConstructorDeclaration>
{
    /// <remarks>
    /// Only direct visibility modifiers are checked. The method returns <c>true</c> even if a public class is nested within a non-public class.
    /// </remarks>
    [Pure]
    static bool IsPublicSurfaceArea(ICSharpTypeDeclaration typeDeclaration)
        => typeDeclaration.GetAccessRights() is AccessRights.PUBLIC or AccessRights.PROTECTED or AccessRights.PROTECTED_OR_INTERNAL;

    protected override void Run(IConstructorDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!element.IsCSharp72Supported())
        {
            return;
        }

        var containingTypeDeclaration = element.GetContainingTypeDeclaration();
        if (containingTypeDeclaration is { IsAbstract: true } && element.GetAccessRights() == AccessRights.INTERNAL)
        {
            var tokenNode = element.ModifiersList.Modifiers.First(node => node.GetTokenType() == CSharpTokenType.INTERNAL_KEYWORD);

            if (IsPublicSurfaceArea(containingTypeDeclaration))
            {
                consumer.AddHighlighting(
                    new InternalConstructorVisibilitySuggestion(
                        "Make internal constructor in public abstract class 'private protected'.",
                        tokenNode,
                        element,
                        AccessRights.PROTECTED_AND_INTERNAL));
            }
            else
            {
                consumer.AddHighlighting(
                    new InternalConstructorVisibilitySuggestion(
                        "Make internal constructor in non-public abstract class 'protected'.",
                        tokenNode,
                        element,
                        AccessRights.PROTECTED));
            }
        }
    }
}