using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts.Internal
{
    internal abstract class CSharpNumericTypeInfo
    {
        [CanBeNull]
        public static CSharpNumericTypeInfo TryCreate([NotNull] IType type)
        {
            if (type.IsInt())
            {
                return new CSharpNumericTypeInfo<int>(
                    true,
                    1,
                    "",
                    null,
                    (x, y) => x <= y,
                    value => value == 0,
                    value => value + 1,
                    value => value * 2,
                    value => value);
            }

            if (type.IsUint())
            {
                return new CSharpNumericTypeInfo<uint>(
                    false,
                    1u,
                    "u",
                    null,
                    (x, y) => x <= y,
                    value => value == 0u,
                    value => value + 1u,
                    value => value * 2u,
                    value => value);
            }

            if (type.IsLong())
            {
                return new CSharpNumericTypeInfo<long>(
                    true,
                    1L,
                    "L",
                    null,
                    (x, y) => x <= y,
                    value => value == 0L,
                    value => value + 1L,
                    value => value * 2L,
                    value => value);
            }

            if (type.IsUlong())
            {
                return new CSharpNumericTypeInfo<ulong>(
                    false,
                    1ul,
                    "ul",
                    null,
                    (x, y) => x <= y,
                    value => value == 0ul,
                    value => value + 1ul,
                    value => value * 2ul,
                    value => value);
            }

            if (type.IsByte())
            {
                return new CSharpNumericTypeInfo<byte>(
                    false,
                    1,
                    "",
                    null,
                    (x, y) => x <= y,
                    value => value == 0,
                    value => (byte)(value + 1),
                    value => (byte)(value * 2),
                    value => value);
            }

            if (type.IsSbyte())
            {
                return new CSharpNumericTypeInfo<sbyte>(
                    true,
                    1,
                    "",
                    null,
                    (x, y) => x <= y,
                    value => value == 0,
                    value => (sbyte)(value + 1),
                    value => (sbyte)(value * 2),
                    value => value);
            }

            if (type.IsShort())
            {
                return new CSharpNumericTypeInfo<short>(
                    true,
                    1,
                    "",
                    null,
                    (x, y) => x <= y,
                    value => value == 0,
                    value => (short)(value + 1),
                    value => (short)(value * 2),
                    value => value);
            }

            if (type.IsUshort())
            {
                return new CSharpNumericTypeInfo<ushort>(
                    false,
                    1,
                    "",
                    null,
                    (x, y) => x <= y,
                    value => value == 0,
                    value => (ushort)(value + 1),
                    value => (ushort)(value * 2),
                    value => value);
            }

            if (type.IsDecimal())
            {
                return new CSharpNumericTypeInfo<decimal>(
                    true,
                    1m,
                    "m",
                    null,
                    (x, y) => x <= y,
                    value => value == 0m,
                    null,
                    value => value * 2m,
                    value => (double)value);
            }

            if (type.IsDouble())
            {
                return new CSharpNumericTypeInfo<double>(
                    true,
                    1d,
                    "d",
                    $"double.{nameof(double.Epsilon)}",
                    (x, y) => x <= y,
                    value => Math.Abs(value - 0d) < double.Epsilon,
                    null,
                    value => value * 2d,
                    value => value);
            }

            if (type.IsFloat())
            {
                return new CSharpNumericTypeInfo<float>(
                    true,
                    1f,
                    "f",
                    $"float.{nameof(float.Epsilon)}",
                    (x, y) => x <= y,
                    value => Math.Abs(value - 0f) < float.Epsilon,
                    null,
                    value => value * 2f,
                    value => value);
            }

            return null;
        }

        protected CSharpNumericTypeInfo(bool isSigned, [CanBeNull] string epsilonLiteral, [NotNull] string literalSuffix)
        {
            LiteralSuffix = literalSuffix;
            IsSigned = isSigned;
            EpsilonLiteral = epsilonLiteral;
        }

        public bool IsSigned { get; }

        [CanBeNull]
        public string EpsilonLiteral { get; }

        [NotNull]
        public string LiteralSuffix { get; }

        [CanBeNull]
        public abstract EnumBetweenFirstAndLast.EnumContractInfo TryCreateEnumContractInfoForEnumBetweenFirstAndLast([NotNull] IList<IField> members);

        [CanBeNull]
        public abstract EnumFlags.EnumContractInfo TryCreateEnumFlags([NotNull] IList<IField> members);
    }
}