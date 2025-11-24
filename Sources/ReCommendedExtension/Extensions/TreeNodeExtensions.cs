using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class TreeNodeExtensions
{
    extension(ITreeNode treeNode)
    {
        [Pure]
        public bool MustDisposeResourceAttributeSupportsStructs()
            => treeNode
                .GetPsiServices()
                .CodeAnnotationsConfiguration.GetAttributeTypeForElement(treeNode, nameof(MustDisposeResourceAttribute))
                ?.GetAttributeInstances(PredefinedType.ATTRIBUTE_USAGE_ATTRIBUTE_CLASS, AttributesSource.Self)
                .FirstOrDefault(attributeInstance => attributeInstance.PositionParameterCount == 1
                    && attributeInstance.PositionParameter(0).ConstantValue is { Kind: ConstantValueKind.Enum } constantValue
                    && constantValue.Type.IsClrType(PredefinedType.ATTRIBUTE_TARGETS_ENUM)
                    && ((AttributeTargets)constantValue.IntValue & AttributeTargets.Struct) != 0) is { };
    }
}