using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class AttributesOwnerDeclarationExtensions
{
    static readonly HashSet<string> wellKnownUnitTestingAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "MSTest.TestFramework", "Microsoft.VisualStudio.TestPlatform.TestFramework", "nunit.framework", "xunit.core",
    };

    [Pure]
    public static bool IsDeclaredInTestProject(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetProject() is { } project)
        {
            if (project.HasFlavour<MsTestProjectFlavor>())
            {
                return true;
            }

            if (project
                .GetAssemblyReferences(project.GetCurrentTargetFrameworkId())
                .Any(assemblyReference => assemblyReference is { } && wellKnownUnitTestingAssemblyNames.Contains(assemblyReference.Name)))
            {
                return true;
            }
        }

        return false;
    }

    [Pure]
    public static bool IsOnLocalFunctionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter parameter && parameter.ContainingParametersOwner.IsLocalFunction()
            || attributesOwnerDeclaration.DeclaredElement is ILocalFunctionDeclaration;
    }

    [Pure]
    public static bool IsOnLambdaExpressionWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100)
        {
            return false;
        }

        return attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: ILambdaExpression } or ILambdaExpression;
    }

    [Pure]
    public static bool IsOnAnonymousMethodWithUnsupportedAttributes(this IAttributesOwnerDeclaration attributesOwnerDeclaration)
        => attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: IAnonymousMethodExpression }
            or IAnonymousMethodExpression;

    [Pure]
    public static ITypeElement? TryGetAnnotationAttributeType(this IAttributesOwnerDeclaration attributesOwnerDeclaration, string attributeShortName)
        => attributesOwnerDeclaration
            .GetPsiServices()
            .GetComponent<CodeAnnotationsConfiguration>()
            .GetAttributeTypeForElement(attributesOwnerDeclaration, attributeShortName);

    [Pure]
    public static bool IsAnnotationProvided(this IAttributesOwnerDeclaration attributesOwnerDeclaration, string attributeShortName)
        => attributesOwnerDeclaration.TryGetAnnotationAttributeType(attributeShortName) is { };
}