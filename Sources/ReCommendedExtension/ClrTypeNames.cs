using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;

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

    public static readonly IClrTypeName MathF = new ClrTypeName("System.MathF");

    public static readonly IClrTypeName ContractClassForAttribute = GetClrTypeName<ContractClassForAttribute>();

    public static readonly IClrTypeName ContractClassAttribute = GetClrTypeName<ContractClassAttribute>();

    public static readonly IClrTypeName ContractInvariantMethodAttribute = GetClrTypeName<ContractInvariantMethodAttribute>();

    public static readonly IClrTypeName SuppressMessageAttribute = GetClrTypeName<SuppressMessageAttribute>();

    public static readonly IClrTypeName ExcludeFromCodeCoverageAttribute = GetClrTypeName<ExcludeFromCodeCoverageAttribute>();

    public static readonly IClrTypeName NotSupportedException = GetClrTypeName<NotSupportedException>();

    public static readonly IClrTypeName KeyNotFoundException = GetClrTypeName<KeyNotFoundException>();

    public static readonly IClrTypeName IEqualityComparerGeneric = GetClrTypeName(typeof(IEqualityComparer<>));

    public static readonly IClrTypeName IEqualityComparer = GetClrTypeName<IEqualityComparer>();

    public static readonly IClrTypeName IReadOnlyList = GetClrTypeName(typeof(IReadOnlyList<>));

    public static readonly IClrTypeName IReadOnlySet = GetClrTypeName(typeof(IReadOnlySet<>));

    public static readonly IClrTypeName DictionaryKeyCollection = GetClrTypeName(typeof(Dictionary<,>.KeyCollection));

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

    public static readonly IClrTypeName ConfigureAwaitOptions = new ClrTypeName("System.Threading.Tasks.ConfigureAwaitOptions");

    public static readonly IClrTypeName StringSplitOptions = GetClrTypeName<StringSplitOptions>();

    public static readonly IClrTypeName NumberStyles = GetClrTypeName<NumberStyles>();

    public static readonly IClrTypeName MidpointRounding = GetClrTypeName<MidpointRounding>();

    public static readonly IClrTypeName Lock = new ClrTypeName("System.Threading.Lock");

    public static readonly IClrTypeName Int128 = new ClrTypeName("System.Int128");

    public static readonly IClrTypeName UInt128 = new ClrTypeName("System.UInt128");

    public static readonly IClrTypeName Half = new ClrTypeName("System.Half");

    public static readonly IClrTypeName AppendInterpolatedStringHandler =
        new ClrTypeName("System.Text.StringBuilder.AppendInterpolatedStringHandler");

    public static readonly IClrTypeName MemoryExtensions_TryWriteInterpolatedStringHandler =
        new ClrTypeName("System.MemoryExtensions.TryWriteInterpolatedStringHandler");

    public static readonly IClrTypeName Utf8_TryWriteInterpolatedStringHandler =
        new ClrTypeName("System.Text.Unicode.Utf8.TryWriteInterpolatedStringHandler");

    public static readonly IClrTypeName AssertInterpolatedStringHandler = new ClrTypeName("System.Diagnostics.Debug.AssertInterpolatedStringHandler");

    public static readonly IClrTypeName WriteIfInterpolatedStringHandler =
        new ClrTypeName("System.Diagnostics.Debug.WriteIfInterpolatedStringHandler");

    public static readonly IClrTypeName TraceVerboseInterpolatedStringHandler =
        new ClrTypeName("System.Diagnostics.TraceSwitchExtensions.TraceVerboseInterpolatedStringHandler");

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClrType([NotNullWhen(true)] this ITypeElement? typeElement, IClrTypeName clrName)
        => typeElement is { } && typeElement.GetClrName().Equals(clrName);

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClrType([NotNullWhen(true)] this IType? type, IClrTypeName clrName) => type.GetTypeElement().IsClrType(clrName);

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypeElement? TryGetTypeElement(this IClrTypeName clrName, IPsiModule psiModule)
        => TypeElementUtil.GetTypeElementByClrName(clrName, psiModule);

    [JetBrains.Annotations.Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IType GetType(this IClrTypeName clrName, IPsiModule psiModule) => TypeFactory.CreateTypeByCLRName(clrName, psiModule);
}