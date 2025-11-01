using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Analyzers.ExpressionResult.Inspections;

internal readonly record struct InspectionContext
{
    readonly ICSharpInvocationInfo invocationInfo;
    readonly IList<IParameter> resolvedParameters;
    readonly ITypeElement containingType;

    public InspectionContext(ICSharpInvocationInfo invocationInfo, IList<IParameter> resolvedParameters, ITypeElement containingType)
    {
        Debug.Assert(invocationInfo is ICSharpExpression);

        this.invocationInfo = invocationInfo;
        this.resolvedParameters = resolvedParameters;
        this.containingType = containingType;
    }

    public CSharpLanguageLevel LanguageVersion => invocationInfo.GetLanguageVersion();

    public IList<IType> TypeArguments => invocationInfo.TypeArguments;

    public NumberInfo? NumberInfo
        => resolvedParameters is [var parameter, ..] ? NumberInfo.TryGet(parameter.Type) : NumberInfo.TryGet(TypeFactory.CreateType(containingType));

    [Pure]
    public IType? TryGetTargetType(bool forCollectionExpression) => ((ICSharpExpression)invocationInfo).TryGetTargetType(forCollectionExpression);

    [Pure]
    public ITypeElement? TryGetTypeElement(IClrTypeName clrTypeName) => clrTypeName.TryGetTypeElement(invocationInfo.PsiModule);
}