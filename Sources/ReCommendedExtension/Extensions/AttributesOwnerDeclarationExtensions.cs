using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using JetBrains.ReSharper.Psi;
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

    extension(IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
        [Pure]
        public bool IsDeclaredInTestProject()
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

        public bool IsOnLocalFunctionWithUnsupportedAttributes
        {
            get
            {
                if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90)
                {
                    return false;
                }

                return attributesOwnerDeclaration.DeclaredElement is IParameter parameter && parameter.ContainingParametersOwner.IsLocalFunction()
                    || attributesOwnerDeclaration.DeclaredElement is ILocalFunctionDeclaration;
            }
        }

        public bool IsOnLambdaExpressionWithUnsupportedAttributes
        {
            get
            {
                if (attributesOwnerDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp100)
                {
                    return false;
                }

                return attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: ILambdaExpression } or ILambdaExpression;
            }
        }

        public bool IsOnAnonymousMethodWithUnsupportedAttributes
            => attributesOwnerDeclaration.DeclaredElement is IParameter { ContainingParametersOwner: IAnonymousMethodExpression }
                or IAnonymousMethodExpression;

        [Pure]
        public ITypeElement? TryGetAnnotationAttributeType(string attributeShortName)
            => attributesOwnerDeclaration
                .GetPsiServices()
                .CodeAnnotationsConfiguration.GetAttributeTypeForElement(attributesOwnerDeclaration, attributeShortName);

        [Pure]
        public bool IsAnnotationProvided(string attributeShortName)
            => attributesOwnerDeclaration.TryGetAnnotationAttributeType(attributeShortName) is { };
    }
}