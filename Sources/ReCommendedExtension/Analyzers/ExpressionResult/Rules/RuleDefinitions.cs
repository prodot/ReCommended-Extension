using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.Analyzers.ExpressionResult.Inspections;
using ReCommendedExtension.Extensions.MemberFinding;

namespace ReCommendedExtension.Analyzers.ExpressionResult.Rules;

internal static class RuleDefinitions
{
    /// <remarks>
    /// type → (member name (or "" for constructors) → member overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>> typeMembers = new(new ClrTypeNameEqualityComparer())
    {
        { PredefinedType.BOOLEAN_FQN, CreateBooleanMembers() },
        { PredefinedType.BYTE_FQN, CreateIntegerMembers(Parameter.Byte) },
        { PredefinedType.SBYTE_FQN, CreateSignedIntegerMembers(Parameter.SByte) },
        { PredefinedType.SHORT_FQN, CreateSignedIntegerMembers(Parameter.Int16) },
        { PredefinedType.USHORT_FQN, CreateIntegerMembers(Parameter.UInt16) },
        { PredefinedType.INT_FQN, CreateSignedIntegerMembers(Parameter.Int32) },
        { PredefinedType.UINT_FQN, CreateIntegerMembers(Parameter.UInt32) },
        { PredefinedType.LONG_FQN, CreateSignedIntegerMembers(Parameter.Int64) },
        { PredefinedType.ULONG_FQN, CreateIntegerMembers(Parameter.UInt64) },
        { ClrTypeNames.Int128, CreateSignedIntegerMembers(Parameter.Int128) },
        { ClrTypeNames.UInt128, CreateIntegerMembers(Parameter.UInt128) },
        { PredefinedType.INTPTR_FQN, CreateSignedIntegerMembers(Parameter.IntPtr) },
        { PredefinedType.UINTPTR_FQN, CreateIntegerMembers(Parameter.UIntPtr) },
        { PredefinedType.DECIMAL_FQN, CreateNumberMembers(Parameter.Decimal) },
        { PredefinedType.DOUBLE_FQN, CreateFloatingPointNumberMembers(Parameter.Double) },
        { PredefinedType.FLOAT_FQN, CreateFloatingPointNumberMembers(Parameter.Single) },
        { ClrTypeNames.Half, CreateFloatingPointNumberMembers(Parameter.Half) },
        { ClrTypeNames.Math, CreateMathMembers() },
        { PredefinedType.DATETIME_FQN, CreateDateTimeMembers() },
        { PredefinedType.DATETIMEOFFSET_FQN, CreateDateTimeOffsetMembers() },
        { PredefinedType.TIMESPAN_FQN, CreateTimeSpanMembers() },
        { PredefinedType.DATE_ONLY_FQN, CreateDateOnlyMembers() },
        { PredefinedType.TIME_ONLY_FQN, CreateTimeOnlyMembers() },
        { PredefinedType.GUID_FQN, CreateGuidMembers() },
        { PredefinedType.CHAR_FQN, CreateCharMembers() },
        { PredefinedType.STRING_FQN, CreateStringMembers() },
        { ClrTypeNames.Random, CreateRandomMembers() },
    };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateBooleanMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(bool.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Boolean] },
                        Inspections =
                        [
                            Inspection.NonBooleanConstantWithFalseToInvertedQualifier,
                            Inspection.TrueWithNonBooleanConstantToArg,
                            Inspection.FalseWithNonBooleanConstantToInvertedArg,
                        ],
                    },
                    new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] },
                ]
            },
            {
                nameof(bool.GetTypeCode),
                [new Member { Signature = new MethodSignature { Parameters = [] }, Inspections = [Inspection.ToTypeCodeForBoolean] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateNumberMembers(Parameter numberTypeParameter)
        => new(StringComparer.Ordinal)
        {
            {
                "Clamp", // todo: nameof(INumber<T>.Clamp) when available
                [
                    new Member
                    {
                        Signature =
                            new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter, numberTypeParameter], IsStatic = true },
                        Inspections = [Inspection.NumberMinMaxEqualToMinOrMax, Inspection.NumberTypeMinTypeMaxToNumber],
                    },
                ]
            },
            {
                nameof(Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
            {
                nameof(IConvertible.GetTypeCode),
                [new Member { Signature = new MethodSignature { Parameters = [] }, Inspections = [Inspection.ToTypeCodeForNumber] }]
            },
            {
                "Max", // todo: nameof(INumber<T>.Max) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter], IsStatic = true },
                        Inspections = [Inspection.XYEqualToXOrY],
                    },
                ]
            },
            {
                "Min", // todo: nameof(INumber<T>.Min) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter], IsStatic = true },
                        Inspections = [Inspection.XYEqualToXOrY],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateIntegerMembers(Parameter numberTypeParameter)
    {
        var members = CreateNumberMembers(numberTypeParameter);

        members.Add(
            "DivRem", // todo: nameof(IBinaryInteger<T>.DivRem) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter], IsStatic = true },
                    Inspections = [Inspection.ZeroNonZeroToQuotientRemainder],
                },
            ]);

        members.Add(
            "RotateLeft", // todo: nameof(IBinaryInteger<T>.RotateLeft) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, Parameter.Int32], IsStatic = true },
                    Inspections = [Inspection.NumberZeroToNumber],
                },
            ]);

        members.Add(
            "RotateRight", // todo: nameof(IBinaryInteger<T>.RotateRight) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, Parameter.Int32], IsStatic = true },
                    Inspections = [Inspection.NumberZeroToNumber],
                },
            ]);

        return members;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateSignedIntegerMembers(Parameter numberTypeParameter)
    {
        var members = CreateIntegerMembers(numberTypeParameter);

        members.Add(
            "MaxMagnitude", // todo: nameof(INumberBase<T>.MaxMagnitude) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter], IsStatic = true },
                    Inspections = [Inspection.XYEqualToXOrY],
                },
            ]);

        members.Add(
            "MinMagnitude", // todo: nameof(INumberBase<T>.MinMagnitude) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter, numberTypeParameter], IsStatic = true },
                    Inspections = [Inspection.XYEqualToXOrY],
                },
            ]);

        return members;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateFloatingPointNumberMembers(Parameter numberTypeParameter)
        => CreateNumberMembers(numberTypeParameter);

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateMathMembers()
    {
        Parameter[] integerParameterTypes =
        [
            Parameter.Byte,
            Parameter.SByte,
            Parameter.Int16,
            Parameter.UInt16,
            Parameter.Int32,
            Parameter.UInt32,
            Parameter.Int64,
            Parameter.UInt64,
            Parameter.IntPtr,
            Parameter.UIntPtr,
        ];

        Parameter[] parameterTypes = [..integerParameterTypes, Parameter.Decimal];

        return new Dictionary<string, IReadOnlyCollection<Member>>(StringComparer.Ordinal)
        {
            {
                "Clamp", // todo: nameof(Math.Clamp) when available
                [
                    ..
                    from parameterType in parameterTypes
                    select new Member
                    {
                        Signature = new MethodSignature { Parameters = [parameterType, parameterType, parameterType], IsStatic = true },
                        Inspections = [Inspection.NumberMinMaxEqualToMinOrMax, Inspection.NumberTypeMinTypeMaxToNumber],
                    },
                ]
            },
            {
                nameof(Math.DivRem),
                [
                    ..
                    from parameterType in integerParameterTypes
                    select new Member
                    {
                        Signature = new MethodSignature { Parameters = [parameterType, parameterType], IsStatic = true },
                        Inspections = [Inspection.ZeroNonZeroToQuotientRemainder],
                    },
                ]
            },
            {
                nameof(Math.Max),
                [
                    ..
                    from parameterType in parameterTypes
                    select new Member
                    {
                        Signature = new MethodSignature { Parameters = [parameterType, parameterType], IsStatic = true },
                        Inspections = [Inspection.XYEqualToXOrY],
                    },
                ]
            },
            {
                nameof(Math.Min),
                [
                    ..
                    from parameterType in parameterTypes
                    select new Member
                    {
                        Signature = new MethodSignature { Parameters = [parameterType, parameterType], IsStatic = true },
                        Inspections = [Inspection.XYEqualToXOrY],
                    },
                ]
            },
        };
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.Int64] }, Inspections = [Inspection.ZeroToDateTimeMinValue],
                    },
                ]
            },
            {
                nameof(DateTime.Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
            {
                nameof(DateTime.GetTypeCode),
                [new Member { Signature = new MethodSignature { Parameters = [] }, Inspections = [Inspection.ToTypeCodeForDateTime] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeOffsetMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTimeOffset.Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeSpanMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.Int64] },
                        Inspections =
                        [
                            Inspection.ZerosToTimeSpanZero,
                            Inspection.Int64MinValueToTimeSpanMinValue,
                            Inspection.Int64MaxValueToTimeSpanMaxValue,
                        ],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 3)] },
                        Inspections = [Inspection.ZerosToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 4)] },
                        Inspections = [Inspection.ZerosToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 5)] },
                        Inspections = [Inspection.ZerosToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6)] },
                        Inspections = [Inspection.ZerosToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
            {
                nameof(TimeSpan.FromDays),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32], IsStatic = true },
                        Inspections = [Inspection.Int32ZerosOptionalToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.Int32, Parameter.Int32, ..Enumerable.Repeat(Parameter.Int64, 4)], IsStatic = true,
                        },
                        Inspections = [Inspection.Int32ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.FromHours),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32], IsStatic = true },
                        Inspections = [Inspection.Int32ZerosOptionalToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, ..Enumerable.Repeat(Parameter.Int64, 4)], IsStatic = true },
                        Inspections = [Inspection.Int32ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                "FromMicroseconds", // todo: nameof(TimeSpan.FromMicroseconds) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.FromMilliseconds),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64, Parameter.Int64], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.FromMinutes),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [..Enumerable.Repeat(Parameter.Int64, 4)], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.FromSeconds),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [..Enumerable.Repeat(Parameter.Int64, 3)], IsStatic = true },
                        Inspections = [Inspection.Int64ZerosOptionalToTimeSpanZero],
                    },
                ]
            },
            {
                nameof(TimeSpan.FromTicks),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64], IsStatic = true },
                        Inspections =
                        [
                            Inspection.ZerosToTimeSpanZero,
                            Inspection.Int64MinValueToTimeSpanMinValue,
                            Inspection.Int64MaxValueToTimeSpanMaxValue,
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateOnlyMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "Equals", // todo: nameof(DateOnly.Equals) when available
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeOnlyMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "", // constructors
                [
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.Int64] }, Inspections = [Inspection.ZerosToTimeOnlyMinValue],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [Parameter.Int32, Parameter.Int32] },
                        Inspections = [Inspection.ZerosToTimeOnlyMinValue],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 3)] },
                        Inspections = [Inspection.ZerosToTimeOnlyMinValue],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 4)] },
                        Inspections = [Inspection.ZerosToTimeOnlyMinValue],
                    },
                    new Member
                    {
                        Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 5)] },
                        Inspections = [Inspection.ZerosToTimeOnlyMinValue],
                    },
                ]
            },
            {
                "Equals", // todo: nameof(TimeOnly.Equals) when available
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateGuidMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Guid.Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateCharMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(char.Equals),
                [new Member { Signature = new MethodSignature { Parameters = [Parameter.Object] }, Inspections = [Inspection.NullToFalse] }]
            },
            {
                nameof(char.GetTypeCode),
                [new Member { Signature = new MethodSignature { Parameters = [] }, Inspections = [Inspection.ToTypeCodeForChar] }]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateStringMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(string.Contains),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.EndsWith),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.GetTypeCode),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [] },
                        Inspections = [Inspection.ToTypeCodeForString with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.IndexOf),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections = [Inspection.EmptyStringToZero with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections = [Inspection.EmptyStringToZero with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.IndexOfAny),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections = [Inspection.EmptyCollectionToMinusOne with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.Join),
                [
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.Char, Parameter.IEnumerableOfT], IsStatic = true, GenericParametersCount = 1,
                        },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementIEnumerableOfTInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.StringArray], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString, Inspection.SingleElementParamsStringArrayInArg1ToElement,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ObjectArray], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsObjectArrayInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfString], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsReadOnlySpanOfStringInArg1ToElement,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfObject], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsReadOnlySpanOfObjectInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature
                        {
                            Parameters = [Parameter.String, Parameter.IEnumerableOfT], IsStatic = true, GenericParametersCount = 1,
                        },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementIEnumerableOfTInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringArray], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString, Inspection.SingleElementParamsStringArrayInArg1ToElement,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ObjectArray], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsObjectArrayInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfString], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsReadOnlySpanOfStringInArg1ToElement,
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfObject], IsStatic = true },
                        Inspections =
                        [
                            Inspection.EmptyCollectionInParamsArg1ToEmptyString,
                            Inspection.SingleElementParamsReadOnlySpanOfObjectInArg1ToElementConvertedToString,
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.Char, Parameter.StringArray, Parameter.Int32, Parameter.Int32], IsStatic = true,
                            },
                        Inspections =
                        [
                            Inspection.ZeroInArg2ZeroInArg3ToEmptyString,
                            Inspection.SingleElementCollectionInArg1OneInArg2ZeroInArg3ToEmptyString,
                            Inspection.SingleElementCollectionInArg1ZeroInArg2OneInArg3ToElement,
                        ],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.Int32, Parameter.Int32], IsStatic = true,
                            },
                        Inspections =
                        [
                            Inspection.ZeroInArg2ZeroInArg3ToEmptyString,
                            Inspection.SingleElementCollectionInArg1OneInArg2ZeroInArg3ToEmptyString,
                            Inspection.SingleElementCollectionInArg1ZeroInArg2OneInArg3ToElement,
                        ],
                    },
                ]
            },
            {
                nameof(string.LastIndexOf),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [Inspection.ZeroInArg1ToMinusOne with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.LastIndexOfAny),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections = [Inspection.EmptyCollectionToMinusOne with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections = [Inspection.ZeroInArg1ToMinusOne with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections = [Inspection.ZeroInArg1ZeroOrOneInArg2ToMinusOne with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(string.Remove), [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections =
                        [
                            Inspection.ZeroToEmptyString with
                            {
                                MinimumFrameworkVersion = new Version(6, 0), // frameworks earlier than .NET 6 throw an exception for '"".Remove(0)'
                                EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.Split),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.NullOrEmptyStringInArg0OptionalStringSplitOptionsInLastArgToSingleElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                            Inspection.NullOrEmptyStringInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections =
                        [
                            Inspection.ZeroInArg1ToEmptyArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1ToSingleElementArray with { EnsureQualifierNotNull = true },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringArray, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                            Inspection.EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.ZeroInArg1ToEmptyArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1OptionalStringSplitOptionsInArg2ToSingleElementArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1StringSplitOptionsInArg2ToSingleTrimmedElementArray with { EnsureQualifierNotNull = true },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.ZeroInArg1ToEmptyArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1OptionalStringSplitOptionsInArg2ToSingleElementArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1StringSplitOptionsInArg2ToSingleTrimmedElementArray with { EnsureQualifierNotNull = true },
                            Inspection.NullOrEmptyStringInArg0OptionalStringSplitOptionsInLastArgToSingleElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                            Inspection.NullOrEmptyStringInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.ZeroInArg1ToEmptyArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1OptionalStringSplitOptionsInArg2ToSingleElementArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1StringSplitOptionsInArg2ToSingleTrimmedElementArray with { EnsureQualifierNotNull = true },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringArray, Parameter.Int32, Parameter.StringSplitOptions] },
                        Inspections =
                        [
                            Inspection.ZeroInArg1ToEmptyArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1OptionalStringSplitOptionsInArg2ToSingleElementArray with { EnsureQualifierNotNull = true },
                            Inspection.OneInArg1StringSplitOptionsInArg2ToSingleTrimmedElementArray with { EnsureQualifierNotNull = true },
                            Inspection.EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                            Inspection.EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray with
                            {
                                EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.StartsWith),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections = [Inspection.EmptyStringToTrue with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateRandomMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "GetHexString", // todo: nameof(Random.GetHexString) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Boolean] },
                        Inspections = [Inspection.ZeroToEmptyString with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                "GetItems", // todo: nameof(Random.GetItems) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TArray, Parameter.Int32], GenericParametersCount = 1 },
                        Inspections = [Inspection.ArrayZeroToEmptyArray with { EnsureQualifierNotNull = true, EnsureFirstArgumentNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfT, Parameter.Int32], GenericParametersCount = 1 },
                        Inspections = [Inspection.ReadOnlySpanZeroToEmptyArray with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                nameof(Random.Next),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [Inspection.Int32ZeroOrOneToZero with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Int32] },
                        Inspections = [Inspection.Int32ValueValueOrValueNextValueToValue with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
            {
                "NextInt64", // todo: nameof(Random.NextInt64) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [Inspection.Int64ZeroOrOneToZero with { EnsureQualifierNotNull = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64, Parameter.Int64] },
                        Inspections = [Inspection.Int64ValueValueOrValueNextValueToValue with { EnsureQualifierNotNull = true }],
                    },
                ]
            },
        };

    [Pure]
    static Member? TryFindMember(IReadOnlyCollection<Member> members, ITypeMember resolvedMember)
    {
        [Pure]
        static bool AreParametersMatching(IReadOnlyList<Parameter> parameters, IList<IParameter> resolvedParameters)
        {
            if (parameters.Count != resolvedParameters.Count)
            {
                return false;
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var resolvedParameter = resolvedParameters[i];

                if (parameter.Kind != resolvedParameter.Kind || !parameter.IsType(resolvedParameter.Type))
                {
                    return false;
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
                    resolvedConstructor.Parameters):

                case (MethodSignature methodSignature, IMethod resolvedMethod) when methodSignature.IsStatic == resolvedMethod.IsStatic
                    && methodSignature.GenericParametersCount == resolvedMethod.TypeParametersCount
                    && AreParametersMatching(methodSignature.Parameters, resolvedMethod.Parameters):
                {
                    return member;
                }
            }
        }

        return null;
    }

    [Pure]
    static Member? TryGetMember(ITypeElement type, string memberName, ITypeMember resolvedMember)
    {
        if (typeMembers.TryGetValue(type.GetClrName(), out var overloads) && overloads.TryGetValue(memberName, out var members))
        {
            return TryFindMember(members, resolvedMember);
        }

        return null;
    }

    [Pure]
    public static Member? TryGetConstructor(ITypeElement type, IConstructor resolvedConstructor)
    {
        if (TryGetMember(type, "", resolvedConstructor) is { Signature: ConstructorSignature } constructor)
        {
            return constructor;
        }

        return null;
    }

    [Pure]
    public static Member? TryGetMethod(ITypeElement type, IMethod resolvedMethod)
    {
        if (TryGetMember(type, resolvedMethod.ShortName, resolvedMethod) is { Signature: MethodSignature } method)
        {
            return method;
        }

        return null;
    }
}