using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    Group = "C#",
    Name = "Add contract: string is not null or empty" + ZoneMarker.Suffix,
    Description = "Adds a contract that a string is not null and not empty.")]
public sealed class StringNotNullAndNotEmpty(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    protected override bool IsAvailableForType(IType type) => type.IsString();

    protected override string GetContractTextForUI(string contractIdentifier) => $"!string.{nameof(string.IsNullOrEmpty)}({contractIdentifier})";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        => factory.CreateExpression($"!string.{nameof(string.IsNullOrEmpty)}($0)", contractExpression);

    protected override string TryGetAnnotationAttributeTypeName() => NullnessProvider.NotNullAttributeShortName;
}