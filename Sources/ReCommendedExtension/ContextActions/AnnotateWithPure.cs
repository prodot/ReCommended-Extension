using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Annotate method with [Pure] attribute" + ZoneMarker.Suffix,
        Description = "Annotates a method with the [Pure] attribute.")]
    public sealed class AnnotateWithPure : AnnotateWithCodeAnnotation
    {
        public AnnotateWithPure([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName
        {
            get
            {
                Debug.Assert(PureAnnotationProvider.PureAttributeShortName != null);

                return PureAnnotationProvider.PureAttributeShortName;
            }
        }

        protected override string TextSuffix => "no observable state changes";

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule psiModule)
            => declaredElement is IMethod method &&
                (!method.ReturnType.IsVoid() || method.Parameters.Any(parameter => parameter.AssertNotNull().Kind == ParameterKind.OUTPUT)) &&
                method.Parameters.All(parameter => parameter.AssertNotNull().Kind != ParameterKind.REFERENCE);

        protected override IAttribute TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration)
            => ownerDeclaration.Attributes.FirstOrDefault(
                attribute => attribute.AssertNotNull().GetAttributeInstance().GetAttributeType().GetClrName().ShortName ==
                    MustUseReturnValueAnnotationProvider.MustUseReturnValueAttributeShortName);
    }
}