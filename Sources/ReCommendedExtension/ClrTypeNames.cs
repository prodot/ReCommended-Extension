using System.Collections;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Original type names used.")]
internal static class ClrTypeNames
{
    [JetBrains.Annotations.Pure]
    static IClrTypeName GetClrTypeName(Type type)
    {
        var fullName = type.FullName;
        Debug.Assert(fullName is { });

        return new ClrTypeName(fullName);
    }

    [JetBrains.Annotations.Pure]
    static IClrTypeName GetClrTypeName<T>() => GetClrTypeName(typeof(T));

    public static readonly IClrTypeName Math = GetClrTypeName(typeof(Math));

    public static readonly IClrTypeName ContractClassForAttribute = GetClrTypeName<ContractClassForAttribute>();

    public static readonly IClrTypeName ContractClassAttribute = GetClrTypeName<ContractClassAttribute>();

    public static readonly IClrTypeName ContractInvariantMethodAttribute = GetClrTypeName<ContractInvariantMethodAttribute>();

    public static readonly IClrTypeName SuppressMessageAttribute = GetClrTypeName<SuppressMessageAttribute>();

    public static readonly IClrTypeName ExcludeFromCodeCoverageAttribute = GetClrTypeName<ExcludeFromCodeCoverageAttribute>();

    public static readonly IClrTypeName NotNullWhenAttribute = GetClrTypeName<NotNullWhenAttribute>();

    public static readonly IClrTypeName NotSupportedException = GetClrTypeName<NotSupportedException>();

    public static readonly IClrTypeName KeyNotFoundException = GetClrTypeName<KeyNotFoundException>();

    public static readonly IClrTypeName IEqualityComparerGeneric = GetClrTypeName(typeof(IEqualityComparer<>));

    public static readonly IClrTypeName IEqualityComparer = GetClrTypeName<IEqualityComparer>();

    public static readonly IClrTypeName IComparableGeneric = GetClrTypeName(typeof(IComparable<>));

    public static readonly IClrTypeName MemberInfo = GetClrTypeName<System.Reflection.MemberInfo>();

    public static readonly IClrTypeName ParameterInfo = GetClrTypeName<ParameterInfo>();

    public static readonly IClrTypeName OutOfMemoryException = GetClrTypeName<OutOfMemoryException>();

    public static readonly IClrTypeName StackOverflowException = GetClrTypeName<StackOverflowException>();

#pragma warning disable 618 // obsolete type
    public static readonly IClrTypeName ExecutionEngineException = GetClrTypeName<ExecutionEngineException>();
#pragma warning restore 618

    public static readonly IClrTypeName Binding = GetClrTypeName<Binding>();

    public static readonly IClrTypeName MultiBinding = GetClrTypeName<MultiBinding>();

    public static readonly IClrTypeName ValueTaskAwaiter = new ClrTypeName("System.Runtime.CompilerServices.ValueTaskAwaiter");

    public static readonly IClrTypeName GenericValueTaskAwaiter = new ClrTypeName("System.Runtime.CompilerServices.ValueTaskAwaiter`1");

    public static readonly IClrTypeName UnreachableException = new ClrTypeName("System.Diagnostics.UnreachableException");

    public static readonly IClrTypeName IEqualityOperators = new ClrTypeName("System.Numerics.IEqualityOperators`3");

    public static readonly IClrTypeName IComparisonOperators = new ClrTypeName("System.Numerics.IComparisonOperators`3");

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsPredefinedTypeElement([NotNullWhen(true)] ITypeElement? typeElement, IClrTypeName clrName)
        => typeElement is { } && typeElement.GetClrName().Equals(clrName);

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClrType([NotNullWhen(true)] this IType? type, IClrTypeName clrName)
        => type is IDeclaredType declaredType && IsPredefinedTypeElement(declaredType.GetTypeElement(), clrName);
}