using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class DeclarationExtensions
{
    extension(IDeclaration declaration)
    {
        public bool OverridesInheritedMember
        {
            get
            {
                if (!declaration.IsValid())
                {
                    return false;
                }

                if (declaration.DeclaredElement is IOverridableMember overridableMember && overridableMember.GetImmediateSuperMembers().Any())
                {
                    return true;
                }

                if (declaration is { DeclaredElement: IParameter { ContainingParametersOwner: IOverridableMember parameterOverridableMember } }
                    && parameterOverridableMember.GetImmediateSuperMembers().Any())
                {
                    return true;
                }

                return false;
            }
        }
    }
}