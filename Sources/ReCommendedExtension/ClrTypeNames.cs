using System.Collections;
using System.ComponentModel;
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
public static class ClrTypeNames
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

    internal static readonly IClrTypeName Math = GetClrTypeName(typeof(Math));

    internal static readonly IClrTypeName MathF = new ClrTypeName("System.MathF");

    internal static readonly IClrTypeName ContractClassForAttribute = GetClrTypeName<ContractClassForAttribute>();

    internal static readonly IClrTypeName ContractClassAttribute = GetClrTypeName<ContractClassAttribute>();

    internal static readonly IClrTypeName ContractInvariantMethodAttribute = GetClrTypeName<ContractInvariantMethodAttribute>();

    internal static readonly IClrTypeName SuppressMessageAttribute = GetClrTypeName<SuppressMessageAttribute>();

    internal static readonly IClrTypeName ExcludeFromCodeCoverageAttribute = GetClrTypeName<ExcludeFromCodeCoverageAttribute>();

    internal static readonly IClrTypeName NotSupportedException = GetClrTypeName<NotSupportedException>();

    internal static readonly IClrTypeName KeyNotFoundException = GetClrTypeName<KeyNotFoundException>();

    internal static readonly IClrTypeName IEqualityComparer = GetClrTypeName<IEqualityComparer>();

    internal static readonly IClrTypeName DictionaryKeyCollection = GetClrTypeName(typeof(Dictionary<,>.KeyCollection));

    internal static readonly IClrTypeName MemberInfo = GetClrTypeName<System.Reflection.MemberInfo>();

    internal static readonly IClrTypeName ParameterInfo = GetClrTypeName<ParameterInfo>();

    internal static readonly IClrTypeName OutOfMemoryException = GetClrTypeName<OutOfMemoryException>();

    internal static readonly IClrTypeName StackOverflowException = GetClrTypeName<StackOverflowException>();

#pragma warning disable 618 // obsolete type
    internal static readonly IClrTypeName ExecutionEngineException = GetClrTypeName<ExecutionEngineException>();
#pragma warning restore 618

    internal static readonly IClrTypeName Binding = GetClrTypeName<Binding>();

    internal static readonly IClrTypeName MultiBinding = GetClrTypeName<MultiBinding>();

    internal static readonly IClrTypeName ValueTaskAwaiter = new ClrTypeName("System.Runtime.CompilerServices.ValueTaskAwaiter");

    internal static readonly IClrTypeName UnreachableException = new ClrTypeName("System.Diagnostics.UnreachableException");

    internal static readonly IClrTypeName IEqualityOperators = new ClrTypeName("System.Numerics.IEqualityOperators`3");

    internal static readonly IClrTypeName IComparisonOperators = new ClrTypeName("System.Numerics.IComparisonOperators`3");

    internal static readonly IClrTypeName StringSplitOptions = GetClrTypeName<StringSplitOptions>();

    internal static readonly IClrTypeName NumberStyles = GetClrTypeName<NumberStyles>();

    internal static readonly IClrTypeName MidpointRounding = GetClrTypeName<MidpointRounding>();

    internal static readonly IClrTypeName TimeSpanStyles = GetClrTypeName<TimeSpanStyles>();

    internal static readonly IClrTypeName DateTimeKind = GetClrTypeName<DateTimeKind>();

    internal static readonly IClrTypeName DateTimeStyles = GetClrTypeName<DateTimeStyles>();

    internal static readonly IClrTypeName Calendar = GetClrTypeName<Calendar>();

    internal static readonly IClrTypeName Random = GetClrTypeName<Random>();

    internal static readonly IClrTypeName EditorBrowsableState = GetClrTypeName<EditorBrowsableState>();

    internal static readonly IClrTypeName Int128 = new ClrTypeName("System.Int128");

    internal static readonly IClrTypeName UInt128 = new ClrTypeName("System.UInt128");

    internal static readonly IClrTypeName Half = new ClrTypeName("System.Half");

    internal static readonly IClrTypeName AppendInterpolatedStringHandler =
        new ClrTypeName("System.Text.StringBuilder.AppendInterpolatedStringHandler");

    internal static readonly IClrTypeName MemoryExtensions_TryWriteInterpolatedStringHandler =
        new ClrTypeName("System.MemoryExtensions.TryWriteInterpolatedStringHandler");

    internal static readonly IClrTypeName Utf8_TryWriteInterpolatedStringHandler =
        new ClrTypeName("System.Text.Unicode.Utf8.TryWriteInterpolatedStringHandler");

    internal static readonly IClrTypeName AssertInterpolatedStringHandler =
        new ClrTypeName("System.Diagnostics.Debug.AssertInterpolatedStringHandler");

    internal static readonly IClrTypeName WriteIfInterpolatedStringHandler =
        new ClrTypeName("System.Diagnostics.Debug.WriteIfInterpolatedStringHandler");

    internal static readonly IClrTypeName TraceVerboseInterpolatedStringHandler =
        new ClrTypeName("System.Diagnostics.TraceSwitchExtensions.TraceVerboseInterpolatedStringHandler");

    internal static readonly IClrTypeName TextWriter = GetClrTypeName<TextWriter>();

    extension([NotNullWhen(true)] ITypeElement? typeElement)
    {
        [JetBrains.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsClrType(IClrTypeName clrName) => typeElement is { } && typeElement.GetClrName().Equals(clrName);
    }

    extension([NotNullWhen(true)] IType? type)
    {
        [JetBrains.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsClrType(IClrTypeName clrName) => type.GetTypeElement().IsClrType(clrName);
    }

    extension(IClrTypeName clrName)
    {
        [JetBrains.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ITypeElement? TryGetTypeElement(IPsiModule psiModule) => TypeElementUtil.GetTypeElementByClrName(clrName, psiModule);

        [JetBrains.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IType GetType(IPsiModule psiModule) => TypeFactory.CreateTypeByCLRName(clrName, psiModule);
    }
}