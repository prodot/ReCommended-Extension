using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Annotate parameter with [InstantHandle] attribute" + ZoneMarker.Suffix,
        Description = "Annotates a parameter with the [InstantHandle] attribute.")]
    public sealed class AnnotateWithInstantHandle : AnnotateWithCodeAnnotation
    {
        public AnnotateWithInstantHandle([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName
        {
            get
            {
                Debug.Assert(InstantHandleAnnotationProvider.InstantHandleAttributeShortName != null);

                return InstantHandleAnnotationProvider.InstantHandleAttributeShortName;
            }
        }

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule psiModule)
            => declaredElement is IParameter parameter &&
                (parameter.Type.IsGenericIEnumerable() ||
                    parameter.Type.IsIAsyncEnumerable() ||
                    parameter.Type.IsImplicitlyConvertibleTo(
                        new DeclaredTypeFromCLRName(PredefinedType.MULTICAST_DELEGATE_FQN, NullableAnnotation.Unknown, psiModule),
                        context.GetTypeConversionRule()));
    }
}