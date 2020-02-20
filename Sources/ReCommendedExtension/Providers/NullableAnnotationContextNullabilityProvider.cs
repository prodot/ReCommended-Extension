using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Providers
{
    [PsiComponent]
    public sealed class NullableAnnotationContextNullabilityProvider : ICustomImplicitNullabilityProvider
    {
        [Pure]
        [ContractAnnotation("null => null", true)]
        static IDeclaration TryGetDeclarationWithEnabledNullableAnnotationContext(IDeclaredElement element)
        {
            switch (element)
            {
                case DelegateInvokeMethod delegateInvokeMethod:
                    return delegateInvokeMethod.Owner?.GetDeclarations().FirstOrDefault(d => d.IsNullableAnnotationsContextEnabled());

                case ITypeOwner _:
                case IFunction _:
                case ILocalFunction _:
                    return element.GetDeclarations().FirstOrDefault(d => d.IsNullableAnnotationsContextEnabled());

                default:
                    return null;
            }
        }

        public CodeAnnotationNullableValue? GetNullableAttribute(IDeclaredElement element)
        {
            var declaration = TryGetDeclarationWithEnabledNullableAnnotationContext(element);

            if (declaration != null)
            {
                return declaration.TryGetReSharperNullableAnnotation();
            }

            if ((element as IClrDeclaredElement)?.Module is IAssemblyPsiModule)
            {
                // element in referenced (compiled) assembly
                return element.TryGetReSharperNullableAnnotation();
            }

            return null;
        }

        public CodeAnnotationNullableValue? GetContainerElementNullableAttribute(IDeclaredElement element)
            => TryGetDeclarationWithEnabledNullableAnnotationContext(element)?.TryGetReSharperItemNullableAnnotation();

        public ICollection<IAttributeInstance> GetSpecialAttributeInstances(IClrDeclaredElement element) => Array.Empty<IAttributeInstance>();
    }
}