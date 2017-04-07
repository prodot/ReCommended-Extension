using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(Group = "C#", Name = "Annotate method with [LinqTunnel] attribute" + ZoneMarker.Suffix,
        Description = "Annotates a method with the [LinqTunnel] attribute.")]
    public sealed class AnnotateWithLinqTunnel : AnnotateWithCodeAnnotation
    {
        public AnnotateWithLinqTunnel([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName
        {
            get
            {
                Debug.Assert(LinqTunnelAnnotationProvider.LinqTunnelAttributeShortName != null);

                return LinqTunnelAnnotationProvider.LinqTunnelAttributeShortName;
            }
        }

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule module) =>
            declaredElement is IMethod method && method.ReturnType.IsGenericIEnumerable() &&
            method.Parameters.Any(p => p.AssertNotNull().Type.IsGenericIEnumerable());
    }
}