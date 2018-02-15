using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Windows.Data;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;

namespace ReCommendedExtension
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Original type names used.")]
    internal static class ClrTypeNames
    {
        [NotNull]
        internal static readonly IClrTypeName Math = new ClrTypeName(typeof(Math).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ContractClassForAttribute = new ClrTypeName(typeof(ContractClassForAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ContractClassAttribute = new ClrTypeName(typeof(ContractClassAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ContractInvariantMethodAttribute =
            new ClrTypeName(typeof(ContractInvariantMethodAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName SuppressMessageAttribute = new ClrTypeName(typeof(SuppressMessageAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName NotSupportedException = new ClrTypeName(typeof(NotSupportedException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName KeyNotFoundException = new ClrTypeName(typeof(KeyNotFoundException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName IEqualityComparerGeneric = new ClrTypeName(typeof(IEqualityComparer<>).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName IEqualityComparer = new ClrTypeName(typeof(IEqualityComparer).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName MemberInfo = new ClrTypeName(typeof(System.Reflection.MemberInfo).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ParameterInfo = new ClrTypeName(typeof(ParameterInfo).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName OutOfMemoryException = new ClrTypeName(typeof(OutOfMemoryException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName StackOverflowException = new ClrTypeName(typeof(StackOverflowException).FullName.AssertNotNull());

        [NotNull]
#pragma warning disable 618 // obsolete type
        internal static readonly IClrTypeName ExecutionEngineException = new ClrTypeName(typeof(ExecutionEngineException).FullName.AssertNotNull());
#pragma warning restore 618

        [NotNull]
        internal static readonly IClrTypeName MultiBinding = new ClrTypeName(typeof(MultiBinding).FullName.AssertNotNull());
    }
}