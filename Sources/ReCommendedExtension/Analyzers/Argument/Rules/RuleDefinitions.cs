using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.Analyzers.Argument.Inspections;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Argument.Rules;

internal static class RuleDefinitions
{
    sealed class ClrTypeNameEqualityComparer : IEqualityComparer<IClrTypeName>
    {
        public bool Equals(IClrTypeName? x, IClrTypeName? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.FullName == y.FullName;
        }

        public int GetHashCode(IClrTypeName obj) => obj.FullName.GetHashCode();
    }

    /// <remarks>
    /// type → (member name (or "" for constructors) → member overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>> typeMembers = new(new ClrTypeNameEqualityComparer())
    {
        { PredefinedType.BYTE_FQN, CreateIntegerMembers(Parameter.Byte) },
        { PredefinedType.SBYTE_FQN, CreateIntegerMembers(Parameter.SByte) },
        { PredefinedType.SHORT_FQN, CreateIntegerMembers(Parameter.Int16) },
        { PredefinedType.USHORT_FQN, CreateIntegerMembers(Parameter.UInt16) },
        { PredefinedType.INT_FQN, CreateIntegerMembers(Parameter.Int32) },
        { PredefinedType.UINT_FQN, CreateIntegerMembers(Parameter.UInt32) },
        { PredefinedType.LONG_FQN, CreateIntegerMembers(Parameter.Int64) },
        { PredefinedType.ULONG_FQN, CreateIntegerMembers(Parameter.UInt64) },
        { ClrTypeNames.Int128, CreateIntegerMembers(Parameter.Int128) },
        { ClrTypeNames.UInt128, CreateIntegerMembers(Parameter.UInt128) },
        { PredefinedType.INTPTR_FQN, CreateIntegerMembers(Parameter.IntPtr) },
        { PredefinedType.UINTPTR_FQN, CreateIntegerMembers(Parameter.UIntPtr) },
        { PredefinedType.DECIMAL_FQN, CreateFractionalNumberMembers(RedundantArgument.NumberStylesNumber, Parameter.Decimal) },
        { PredefinedType.DOUBLE_FQN, CreateFloatingPointNumberMembers(Parameter.Double) },
        { PredefinedType.FLOAT_FQN, CreateFloatingPointNumberMembers(Parameter.Single) },
        { ClrTypeNames.Half, CreateFloatingPointNumberMembers(Parameter.Half) },
        { ClrTypeNames.Math, CreateMathMembers() },
        { ClrTypeNames.MathF, CreateMathFMembers() },
        { PredefinedType.DATETIME_FQN, CreateDateTimeMembers() },
        { PredefinedType.DATETIMEOFFSET_FQN, CreateDateTimeOffsetMembers() },
        { PredefinedType.TIMESPAN_FQN, CreateTimeSpanMembers() },
        { PredefinedType.DATE_ONLY_FQN, CreateDateOnlyMembers() },
        { PredefinedType.TIME_ONLY_FQN, CreateTimeOnlyMembers() },
        { PredefinedType.GUID_FQN, CreateGuidMembers() },
        { PredefinedType.ENUM_FQN, CreateEnumMembers() },
        { PredefinedType.STRING_FQN, CreateStringMembers() },
        { PredefinedType.STRING_BUILDER_FQN, CreateStringBuilderMembers() },
        { ClrTypeNames.Random, CreateRandomMembers() },
    };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateNumberMembers(
        RedundantArgumentByPosition defaultNumberStyles,
        Parameter numberTypeParameter)
    {
        var outParameter = numberTypeParameter with { Kind = ParameterKind.OUTPUT };

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "Parse", // todo: nameof(INumberBase<T>.Parse) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.NumberStyles], IsStatic = true },
                        Inspections = [defaultNumberStyles with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.NumberStyles, Parameter.IFormatProvider], IsStatic = true,
                        },
                        Inspections = [defaultNumberStyles with { ParameterIndex = 1 }, RedundantArgument.Null with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.NumberStyles, Parameter.IFormatProvider,
                                ],
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfByte, Parameter.NumberStyles, Parameter.IFormatProvider,
                                ],
                            },
                        ],
                    },
                ]
            },
            {
                "TryParse", // todo: nameof(IParsable<T>.TryParse) when available
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider, outParameter], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, outParameter], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.IFormatProvider, outParameter], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter], IsStatic = true,
                        },
                        Inspections = [defaultNumberStyles with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter],
                            IsStatic = true,
                        },
                        Inspections = [defaultNumberStyles with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter],
                            IsStatic = true,
                        },
                        Inspections = [defaultNumberStyles with { ParameterIndex = 1 }],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateIntegerMembers(Parameter numberTypeParameter)
        => CreateNumberMembers(RedundantArgument.NumberStylesInteger, numberTypeParameter);

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateFractionalNumberMembers(
        RedundantArgumentByPosition defaultNumberStyles,
        Parameter numberTypeParameter)
    {
        var methods = CreateNumberMembers(defaultNumberStyles, numberTypeParameter);

        methods.Add(
            "Round", // todo: nameof(IFloatingPoint<T>.Round) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, Parameter.Int32], IsStatic = true },
                    Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                },
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, Parameter.MidpointRounding], IsStatic = true },
                    Inspections = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                },
                new Member
                {
                    Signature =
                        new MethodSignature { Parameters = [numberTypeParameter, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true },
                    Inspections =
                    [
                        RedundantArgument.ZeroInt32 with { ParameterIndex = 1 },
                        RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 2 },
                    ],
                },
            ]);

        return methods;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateFloatingPointNumberMembers(Parameter numberTypeParameter)
        => CreateFractionalNumberMembers(RedundantArgument.NumberStylesFloat, numberTypeParameter);

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateMathMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Math.Round),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Int32], IsStatic = true },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32], IsStatic = true },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.MidpointRounding], IsStatic = true },
                        Inspections = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.MidpointRounding], IsStatic = true },
                        Inspections = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.Decimal, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 1 },
                            RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 2 },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.Double, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 1 },
                            RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 2 },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateMathFMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "Round", // todo: nameof(MathF.Round) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Single, Parameter.Int32], IsStatic = true },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Single, Parameter.MidpointRounding], IsStatic = true },
                        Inspections = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [Parameter.Single, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 1 },
                            RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 2 },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeMembers()
    {
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition is { });
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition is { });

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.Int64, Parameter.DateTimeKind] },
                        Inspections = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.DateOnly, Parameter.TimeOnly, Parameter.DateTimeKind] },
                        Inspections = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6)] },
                        Inspections = [RedundantArgumentRange.TripleZero with { ParameterIndexRange = 3..6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6), Parameter.DateTimeKind] },
                        Inspections = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6), Parameter.Calendar] },
                        Inspections = [RedundantArgumentRange.TripleZero with { ParameterIndexRange = 3..6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7)] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.DateTimeKind] },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 6 },
                            RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 7 },
                        ],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.Calendar] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature
                        {
                            Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.Calendar, Parameter.DateTimeKind],
                        },
                        Inspections = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 8 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8)] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.Calendar] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.DateTimeKind] },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 7 },
                            RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 8 },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new ConstructorSignature
                            {
                                Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.Calendar, Parameter.DateTimeKind],
                            },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 7 },
                            RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 9 },
                        ],
                    },
                ]
            },
            {
                nameof(DateTime.GetDateTimeFormats),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.IFormatProvider] },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.IFormatProvider] },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(DateTime.Parse),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                nameof(DateTime.ParseExact),
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.DateTimeStylesNone with { ParameterIndex = 3 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(DateTime.TryParse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateTime with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                nameof(DateTime.TryParseExact),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.DateTimeStyles,
                                        Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.StringArray,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeOffsetMembers()
    {
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition is { });
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition is { });

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.TimeSpan] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.TimeSpan] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature
                        {
                            Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.Calendar, Parameter.TimeSpan],
                        },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.Parse),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.ParseExact),
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.DateTimeStylesNone with { ParameterIndex = 3 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.TryParse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.TryParseExact),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.DateTimeStyles,
                                        Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.StringArray,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeSpanMembers()
    {
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantTimeSpanFormat.FurtherArgumentCondition is { });

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 4)] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 5)] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 4 }],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6)] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 5 }],
                    },
                ]
            },
            {
                nameof(TimeSpan.Parse),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(TimeSpan.ParseExact),
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantTimeSpanFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantTimeSpanFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpanStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 },
                            OtherArgument.NullFormatProviderForInvariantTimeSpanFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantTimeSpanFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider], IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider], ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.TimeSpanStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 },
                            RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpanStyles,
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.TimeSpanStyles,
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(TimeSpan.TryParse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(TimeSpan.TryParseExact),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            OtherArgument.NullFormatProviderForInvariantTimeSpanFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantTimeSpanFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.StringArray,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpanStyles,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 },
                            OtherArgument.NullFormatProviderForInvariantTimeSpanFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantTimeSpanFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpanStyles,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.TimeSpanStyles,
                                    Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 },
                            RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.TimeSpanStyles,
                                        Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.StringArray,
                                Parameter.IFormatProvider,
                                Parameter.TimeSpanStyles,
                                Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 },
                            RedundantCollectionElement.StringTimeSpanFormats with { ParameterIndex = 1 },
                        ],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateOnlyMembers()
    {
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition is { });
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition is { });

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "Parse", // todo: nameof(DateOnly.Parse) when available
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles], IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[2] == null,
                                ParameterIndex = 1,
                                ReplacementSignatureParameters = [Parameter.String],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 1..3 },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                            },
                        ],
                    },
                ]
            },
            {
                "ParseExact", // todo: nameof(DateOnly.ParseExact) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringArray], IsStatic = true },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature { Parameters = [Parameter.String, Parameter.String], ParameterIndex = 1 },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray], IsStatic = true },
                        Inspections = [RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.String, Parameter.String],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.String, Parameter.StringArray],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
            {
                "TryParse", // todo: nameof(DateOnly.TryParse) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                "TryParseExact", // todo: nameof(DateOnly.TryParseExact) when available
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT }],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.DateTimeStyles,
                                        Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringDateTimeFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeOnlyMembers()
    {
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition is { });
        Debug.Assert(OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition is { });

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "Add", // todo: nameof(TimeOnly.Add) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }] },
                        Inspections = [RedundantArgument.Discard with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                "AddHours", // todo: nameof(TimeOnly.AddHours) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }] },
                        Inspections = [RedundantArgument.Discard with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                "AddMinutes", // todo: nameof(TimeOnly.AddMinutes) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }] },
                        Inspections = [RedundantArgument.Discard with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                "Parse", // todo: nameof(TimeOnly.Parse) when available
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles], IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[2] == null,
                                ParameterIndex = 1,
                                ReplacementSignatureParameters = [Parameter.String],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 1..3 },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                ParameterIndex = 1,
                                ReplacementSignatureParameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                            },
                        ],
                    },
                ]
            },
            {
                "ParseExact", // todo: nameof(TimeOnly.ParseExact) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringArray], IsStatic = true },
                        Inspections =
                        [
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature { Parameters = [Parameter.String, Parameter.String], ParameterIndex = 1 },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray], IsStatic = true },
                        Inspections = [RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.String, Parameter.String],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.String, Parameter.StringArray],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgument.Null with
                            {
                                FurtherCondition = args => args[3] == null,
                                ParameterIndex = 2,
                                ReplacementSignatureParameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray],
                            },
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
            {
                "TryParse", // todo: nameof(TimeOnly.TryParse) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.Null with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                "TryParseExact", // todo: nameof(TimeOnly.TryParseExact) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT }],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String, Parameter.String, Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.String,
                                Parameter.String,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormat.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.DateTimeStyles,
                                Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.String,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.SingleCollectionElement with
                            {
                                ParameterIndex = 1,
                                ReplacementSignature =
                                new ReplacementSignature
                                {
                                    Parameters =
                                    [
                                        Parameter.String,
                                        Parameter.String,
                                        Parameter.IFormatProvider,
                                        Parameter.DateTimeStyles,
                                        Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                    ],
                                    ParameterIndex = 1,
                                },
                            },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar,
                                    Parameter.StringArray,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                        Inspections =
                        [
                            RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 },
                            RedundantCollectionElement.StringTimeOnlyFormats with { ParameterIndex = 1 },
                            OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection with
                            {
                                ParameterIndex = 2,
                                FurtherArgumentCondition =
                                OtherArgument.NullFormatProviderForInvariantDateTimeOnlyFormatCollection.FurtherArgumentCondition with
                                {
                                    ParameterIndex = 1,
                                },
                            },
                        ],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateGuidMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Guid.Parse),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                        Inspections = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(Guid.TryParse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IFormatProvider, Parameter.Guid with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.IFormatProvider,
                                Parameter.Guid with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateEnumMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Enum.Parse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.Boolean], IsStatic = true, GenericParametersCount = 1,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.Boolean], IsStatic = true, GenericParametersCount = 1,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Type, Parameter.String, Parameter.Boolean], IsStatic = true },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.Type, Parameter.ReadOnlySpanOfChar, Parameter.Boolean], IsStatic = true,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 2 }],
                    },
                ]
            },
            {
                nameof(Enum.TryParse),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.Boolean, Parameter.T with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                            GenericParametersCount = 1,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.Boolean, Parameter.T with { Kind = ParameterKind.OUTPUT }],
                            IsStatic = true,
                            GenericParametersCount = 1,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.Type,
                                Parameter.String,
                                Parameter.Boolean,
                                Parameter.Object with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 2 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters =
                            [
                                Parameter.Type,
                                Parameter.ReadOnlySpanOfChar,
                                Parameter.Boolean,
                                Parameter.Object with { Kind = ParameterKind.OUTPUT },
                            ],
                            IsStatic = true,
                        },
                        Inspections = [RedundantArgument.False with { ParameterIndex = 2 }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateStringMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(string.IndexOf),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.StringComparison] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(string.IndexOfAny),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantArgument.ZeroInt32 with { ParameterIndex = 1 },
                            RedundantCollectionElement.Char with { ParameterIndex = 0 },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                ]
            },
            {
                nameof(string.LastIndexOfAny),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                ]
            },
            {
                nameof(string.PadLeft),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantArgument.SpaceChar with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(string.PadRight),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantArgument.SpaceChar with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(string.Split),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections = [RedundantArgument.CharDuplicateArgument, RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.StringSplitOptions] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections = [RedundantCollectionElement.Char with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringArray, Parameter.StringSplitOptions] },
                        Inspections = [RedundantCollectionElement.String with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringArray, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections = [RedundantCollectionElement.String with { ParameterIndex = 0 }],
                    },
                ]
            },
            {
                nameof(string.Trim),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantArgument.Null with { ParameterIndex = 0 },
                            RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                            RedundantArgument.CharDuplicateArgument,
                            RedundantCollectionElement.Char with { ParameterIndex = 0 },
                        ],
                    },
                ]
            },
            {
                nameof(string.TrimEnd),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantArgument.Null with { ParameterIndex = 0 },
                            RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                            RedundantArgument.CharDuplicateArgument,
                            RedundantCollectionElement.Char with { ParameterIndex = 0 },
                        ],
                    },
                ]
            },
            {
                nameof(string.TrimStart),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantArgument.Null with { ParameterIndex = 0 },
                            RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                            RedundantArgument.CharDuplicateArgument,
                            RedundantCollectionElement.Char with { ParameterIndex = 0 },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateStringBuilderMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(StringBuilder.Append),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [RedundantArgument.OneInt32 with { ParameterIndex = 1 }],
                    },
                ]
            },
            {
                nameof(StringBuilder.Insert),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.String, Parameter.Int32] },
                        Inspections = [RedundantArgument.OneInt32 with { ParameterIndex = 2 }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateRandomMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Random.Next),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantArgument.MaxValueInt32 with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Int32] },
                        Inspections = [RedundantArgument.ZeroInt32 with { ParameterIndex = 0 }],
                    },
                ]
            },
            {
                "NextInt64", // todo: nameof(Random.NextInt64) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantArgument.MaxValueInt64 with { ParameterIndex = 0 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64, Parameter.Int64] },
                        Inspections = [RedundantArgument.ZeroInt64 with { ParameterIndex = 0 }],
                    },
                ]
            },
        };

    [Pure]
    static Member? TryFindMember(IReadOnlyCollection<Member> members, ITypeMember resolvedMember, out int? parameterIndexOfParams)
    {
        [Pure]
        static bool AreParametersMatching(IReadOnlyList<Parameter> parameters, IList<IParameter> resolvedParameters, out int? parameterIndexOfParams)
        {
            parameterIndexOfParams = null;

            if (parameters.Count != resolvedParameters.Count)
            {
                return false;
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                Debug.Assert(parameterIndexOfParams == null);

                var parameter = parameters[i];
                var resolvedParameter = resolvedParameters[i];

                if (parameter.Kind != resolvedParameter.Kind || !parameter.IsType(resolvedParameter.Type))
                {
                    return false;
                }

                if (resolvedParameter.IsParams)
                {
                    parameterIndexOfParams = i;
                }
            }

            return true;
        }

        foreach (var member in members)
        {
            switch (member.Signature, resolvedMember)
            {
                case (ConstructorSignature constructorSignature, IConstructor resolvedConstructor) when AreParametersMatching(
                    constructorSignature.Parameters,
                    resolvedConstructor.Parameters,
                    out parameterIndexOfParams):

                case (MethodSignature methodSignature, IMethod resolvedMethod) when methodSignature.IsStatic == resolvedMethod.IsStatic
                    && methodSignature.GenericParametersCount == resolvedMethod.TypeParametersCount
                    && AreParametersMatching(methodSignature.Parameters, resolvedMethod.Parameters, out parameterIndexOfParams):
                {
                    return member;
                }
            }
        }

        parameterIndexOfParams = null;
        return null;
    }


    [Pure]
    static Member? TryGetMember(ITypeElement type, string memberName, ITypeMember resolvedMember, out int? parameterIndexOfParams)
    {
        if (typeMembers.TryGetValue(type.GetClrName(), out var overloads) && overloads.TryGetValue(memberName, out var members))
        {
            return TryFindMember(members, resolvedMember, out parameterIndexOfParams);
        }

        parameterIndexOfParams = null;
        return null;
    }

    [Pure]
    public static Member? TryGetConstructor(ITypeElement type, IConstructor resolvedConstructor, out int? parameterIndexOfParams)
    {
        if (TryGetMember(type, "", resolvedConstructor, out parameterIndexOfParams) is { Signature: ConstructorSignature } constructor)
        {
            return constructor;
        }

        parameterIndexOfParams = null;
        return null;
    }

    [Pure]
    public static Member? TryGetMethod(ITypeElement type, IMethod resolvedMethod, out int? parameterIndexOfParams)
    {
        if (TryGetMember(type, resolvedMethod.ShortName, resolvedMethod, out parameterIndexOfParams) is { Signature: MethodSignature } method)
        {
            return method;
        }

        parameterIndexOfParams = null;
        return null;
    }
}