using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Data;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;

namespace ReCommendedExtension
{
    internal static class ClrTypeNames
    {
        [NotNull]
        internal static readonly IClrTypeName Contract = new ClrTypeName(typeof(Contract).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName Enumerable = new ClrTypeName(typeof(Enumerable).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName Guid = new ClrTypeName(typeof(Guid).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName TimeSpan = new ClrTypeName(typeof(TimeSpan).FullName.AssertNotNull());

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
        internal static readonly IClrTypeName DebuggerStepThroughAttribute =
            new ClrTypeName(typeof(DebuggerStepThroughAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName SuppressMessageAttribute = new ClrTypeName(typeof(SuppressMessageAttribute).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ArgumentException = new ClrTypeName(typeof(ArgumentException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ArgumentNullException = new ClrTypeName(typeof(ArgumentNullException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName ArgumentOutOfRangeException =
            new ClrTypeName(typeof(ArgumentOutOfRangeException).FullName.AssertNotNull());

        [NotNull]
        internal static readonly IClrTypeName MultiBinding = new ClrTypeName(typeof(MultiBinding).FullName.AssertNotNull());
    }
}