using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.ContextActions
{
    public abstract class AnnotateWith<A> : AnnotateWith where A : Attribute
    {
        protected AnnotateWith([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override string AnnotationAttributeTypeName => typeof(A).Name;

        protected sealed override bool IsAttribute(IAttribute attribute)
            => attribute.GetAttributeInstance().GetAttributeType().GetClrName().FullName == typeof(A).FullName;

        protected sealed override Func<CSharpElementFactory, IAttribute> CreateAttributeFactoryIfAvailable(
            IAttributesOwnerDeclaration attributesOwnerDeclaration,
            IPsiModule psiModule,
            out IAttribute attributeToReplace)
        {
            attributeToReplace = null;

            if (CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement))
            {
                return factory =>
                {
                    Debug.Assert(factory != null);

                    var fullName = typeof(A).FullName;

                    Debug.Assert(fullName != null);

                    return factory.CreateAttribute(new SpecialAttributeInstance(new ClrTypeName(fullName), psiModule));
                };
            }

            return null;
        }

        protected abstract bool CanBeAnnotated([CanBeNull] IDeclaredElement declaredElement);
    }
}