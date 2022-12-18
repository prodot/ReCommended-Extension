using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Original type names used.")]
    internal static class ClrTypeNames
    {
        [NotNull]
        public static readonly IClrTypeName Math = new ClrTypeName(typeof(Math).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ContractClassForAttribute = new ClrTypeName(typeof(ContractClassForAttribute).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ContractClassAttribute = new ClrTypeName(typeof(ContractClassAttribute).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ContractInvariantMethodAttribute =
            new ClrTypeName(typeof(ContractInvariantMethodAttribute).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName SuppressMessageAttribute = new ClrTypeName(typeof(SuppressMessageAttribute).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ExcludeFromCodeCoverageAttribute =
            new ClrTypeName(typeof(ExcludeFromCodeCoverageAttribute).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName NotSupportedException = new ClrTypeName(typeof(NotSupportedException).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName KeyNotFoundException = new ClrTypeName(typeof(KeyNotFoundException).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName IEqualityComparerGeneric = new ClrTypeName(typeof(IEqualityComparer<>).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName IEqualityComparer = new ClrTypeName(typeof(IEqualityComparer).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName MemberInfo = new ClrTypeName(typeof(System.Reflection.MemberInfo).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ParameterInfo = new ClrTypeName(typeof(ParameterInfo).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName OutOfMemoryException = new ClrTypeName(typeof(OutOfMemoryException).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName StackOverflowException = new ClrTypeName(typeof(StackOverflowException).FullName.AssertNotNull());

        [NotNull]
#pragma warning disable 618 // obsolete type
        public static readonly IClrTypeName ExecutionEngineException = new ClrTypeName(typeof(ExecutionEngineException).FullName.AssertNotNull());
#pragma warning restore 618

        [NotNull]
        public static readonly IClrTypeName Binding = new ClrTypeName(typeof(Binding).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName MultiBinding = new ClrTypeName(typeof(MultiBinding).FullName.AssertNotNull());

        [NotNull]
        public static readonly IClrTypeName ValueTaskAwaiter = new ClrTypeName("System.Runtime.CompilerServices.ValueTaskAwaiter");

        [NotNull]
        public static readonly IClrTypeName GenericValueTaskAwaiter = new ClrTypeName("System.Runtime.CompilerServices.ValueTaskAwaiter`1");

        [NotNull]
        public static readonly IClrTypeName UnreachableException = new ClrTypeName("System.Diagnostics.UnreachableException");

        [JetBrains.Annotations.Pure]
        [ContractAnnotation("typeElement:null => false", true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsPredefinedTypeElement([CanBeNull] ITypeElement typeElement, [NotNull] IClrTypeName clrName)
            => typeElement != null && typeElement.GetClrName().Equals(clrName);

        [JetBrains.Annotations.Pure]
        [ContractAnnotation("type:null => false", true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClrType([CanBeNull] this IType type, [NotNull] IClrTypeName clrName)
            => type is IDeclaredType declaredType && IsPredefinedTypeElement(declaredType.GetTypeElement(), clrName);
    }
}