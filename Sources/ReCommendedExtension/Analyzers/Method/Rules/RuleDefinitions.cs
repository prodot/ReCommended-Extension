using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.Analyzers.Method.Inspections;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Method.Rules;

internal static class RuleDefinitions
{
    /// <remarks>
    /// type → (method name → method overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Method>>> typeMethods = new(new ClrTypeNameEqualityComparer())
    {
        { PredefinedType.BOOLEAN_FQN, CreateBooleanMethods() },
        { PredefinedType.BYTE_FQN, CreateNumberMethods(Parameter.Byte) },
        { PredefinedType.SBYTE_FQN, CreateSignedIntegerMethods(Parameter.SByte) },
        { PredefinedType.SHORT_FQN, CreateSignedIntegerMethods(Parameter.Int16) },
        { PredefinedType.USHORT_FQN, CreateNumberMethods(Parameter.UInt16) },
        { PredefinedType.INT_FQN, CreateSignedIntegerMethods(Parameter.Int32) },
        { PredefinedType.UINT_FQN, CreateNumberMethods(Parameter.UInt32) },
        { PredefinedType.LONG_FQN, CreateSignedIntegerMethods(Parameter.Int64) },
        { PredefinedType.ULONG_FQN, CreateNumberMethods(Parameter.UInt64) },
        { ClrTypeNames.Int128, CreateSignedIntegerMethods(Parameter.Int128) },
        { ClrTypeNames.UInt128, CreateNumberMethods(Parameter.UInt128) },
        { PredefinedType.INTPTR_FQN, CreateSignedIntegerMethods(Parameter.IntPtr) },
        { PredefinedType.UINTPTR_FQN, CreateNumberMethods(Parameter.UIntPtr) },
        { PredefinedType.DECIMAL_FQN, CreateDecimalMethods() },
        { PredefinedType.DATETIME_FQN, CreateDateTimeMethods() },
        { PredefinedType.DATETIMEOFFSET_FQN, CreateDateTimeOffsetMethods() },
        { PredefinedType.TIMESPAN_FQN, CreateTimeSpanMethods() },
        { PredefinedType.DATE_ONLY_FQN, CreateDateOnlyMethods() },
        { PredefinedType.TIME_ONLY_FQN, CreateTimeOnlyMethods() },
        { PredefinedType.GUID_FQN, CreateGuidMethods() },
        { PredefinedType.CHAR_FQN, CreateCharMethods() },
        { PredefinedType.STRING_FQN, CreateStringMethods() },
        { PredefinedType.STRING_BUILDER_FQN, CreateStringBuilderMethods() },
        { PredefinedType.GENERIC_NULLABLE_FQN, CreateNullableMethods() },
    };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateNumberMethods(Parameter numberTypeParameter)
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [numberTypeParameter] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateSignedIntegerMethods(Parameter numberTypeParameter)
    {
        var methods = CreateNumberMethods(numberTypeParameter);

        methods.Add(
            "IsNegative", // todo: nameof(INumberBase<T>.IsNegative) when available
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentZero with { Operator = "<" }],
                },
            ]);

        methods.Add(
            "IsPositive", // todo: nameof(INumberBase<T>.IsPositive) when available
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentZero with { Operator = ">=" }],
                },
            ]);

        return methods;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateBooleanMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(bool.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Boolean] },
                        Inspections =
                        [
                            RedundantMethodInvocation.NonBooleanConstantWithTrue,
                            BinaryOperator.QualifierArgumentNonBooleanConstants with { Operator = "==" },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDecimalMethods()
    {
        var methods = CreateNumberMethods(Parameter.Decimal);

        methods.Add(
            nameof(decimal.Add),
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "+" }],
                },
            ]);

        methods.Add(
            nameof(decimal.Divide),
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "/" }],
                },
            ]);

        methods.Add(
            nameof(decimal.Multiply),
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "*" }],
                },
            ]);

        methods.Add(
            nameof(decimal.Remainder),
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "%" }],
                },
            ]);

        methods.Add(
            nameof(decimal.Subtract),
            [
                new Method
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "-" }],
                },
            ]);

        return methods;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateTimeMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTime.Add),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                nameof(DateTime.AddTicks),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
            {
                nameof(DateTime.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime, Parameter.DateTime], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                nameof(DateTime.Subtract),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateTimeOffsetMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTimeOffset.Add),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.AddTicks),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset, Parameter.DateTimeOffset], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.Subtract),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateTimeSpanMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(TimeSpan.Add),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                "Divide", // todo: nameof(TimeSpan.Divide) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "/" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "/" }],
                    },
                ]
            },
            {
                nameof(TimeSpan.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan, Parameter.TimeSpan], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                "Multiply", // todo: nameof(TimeSpan.Multiply) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "*" }],
                    },
                ]
            },
            {
                nameof(TimeSpan.Subtract),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateOnlyMethods()
        => new(StringComparer.Ordinal)
        {
            {
                "AddDays", // todo: nameof(DateOnly.AddDays) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] }, Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                "Equals", // todo: nameof(DateOnly.Equals) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateOnly] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateTimeOnlyMethods()
        => new(StringComparer.Ordinal)
        {
            {
                "Equals", // todo: nameof(TimeOnly.Equals) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeOnly] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateGuidMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Guid.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Guid] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateCharMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(char.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateStringMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(string.IndexOf),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char] },
                        Inspections =
                        [
                            OtherMethodInvocation.ComparingResultAsContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.Char],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.Char],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections =
                        [
                            OtherMethodInvocation.ComparingResultAsStartsWith with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.StartsWith), Parameters = [Parameter.String],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotStartsWith with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.StartsWith), Parameters = [Parameter.String],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsContainsWithCurrentCulture with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotContainsWithCurrentCulture with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.StringComparison] },
                        Inspections =
                        [
                            OtherMethodInvocation.ComparingResultAsContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.Char, Parameter.StringComparison],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.Char, Parameter.StringComparison],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections =
                        [
                            OtherMethodInvocation.ComparingResultAsStartsWith with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.StartsWith), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotStartsWith with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.StartsWith), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                            OtherMethodInvocation.ComparingResultAsNotContains with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.Contains), Parameters = [Parameter.String, Parameter.StringComparison],
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.IndexOfAny),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod =
                                new ReplacementMethod { Name = nameof(string.IndexOf), Parameters = [Parameter.Char] },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod =
                                new ReplacementMethod { Name = nameof(string.IndexOf), Parameters = [Parameter.Char, Parameter.Int32] },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.IndexOf), Parameters = [Parameter.Char, Parameter.Int32, Parameter.Int32],
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.LastIndexOfAny),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.LastIndexOf), Parameters = [Parameter.Char],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.LastIndexOf), Parameters = [Parameter.Char, Parameter.Int32],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.LastIndexOf),
                                    Parameters = [Parameter.Char, Parameter.Int32, Parameter.Int32],
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.PadLeft),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.PadRight),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.Replace),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.StringComparison] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStringsOrdinal],
                    },
                ]
            },
            {
                nameof(string.Substring),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateStringBuilderMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(StringBuilder.Append),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyString with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyCollection with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithRepeatCountZeroInArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyCollectionZeroZero with { IsPureMethod = false },
                        ],
                    },
                ]
            },
            {
                "AppendJoin", // todo: nameof(StringBuilder.AppendJoin) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.IEnumerableOfT], GenericParametersCount = 1 },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementIEnumerableOfTInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.StringArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsStringArrayInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.String],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ObjectArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsObjectArrayInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfString] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsReadOnlySpanOfStringInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.String],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfObject] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsReadOnlySpanOfObjectInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IEnumerableOfT], GenericParametersCount = 1 },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementIEnumerableOfTInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod { Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object] },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsStringArrayInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.String],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ObjectArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsObjectArrayInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfString] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsReadOnlySpanOfStringInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.String],
                                },
                            },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfObject] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false },
                            OtherMethodInvocation.SingleElementParamsReadOnlySpanOfObjectInArg1WithoutArg0 with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(StringBuilder.Append), Parameters = [Parameter.Object],
                                },
                            },
                        ],
                    },
                ]
            },
            {
                nameof(StringBuilder.Insert),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNullInArg1 with { IsPureMethod = false }],
                    },
                ]
            },
            {
                nameof(StringBuilder.Replace),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings with { IsPureMethod = false }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateNullableMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Nullable<int>.GetValueOrDefault),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [] },
                        Inspections = [BinaryOperator.NullableQualifierDefault with { Operator = "??", HighlightInvokedMethodOnly = true }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.T] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "??", HighlightInvokedMethodOnly = true }],
                    },
                ]
            },
        };

    [Pure]
    static Method? TryFindMethod(IReadOnlyCollection<Method> methods, IMethod resolvedMethod)
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

        foreach (var method in methods)
        {
            if (method.Signature.IsStatic == resolvedMethod.IsStatic
                && method.Signature.GenericParametersCount == resolvedMethod.TypeParametersCount
                && AreParametersMatching(method.Signature.Parameters, resolvedMethod.Parameters))
            {
                return method;
            }
        }

        return null;
    }

    [Pure]
    public static Method? TryGetMethod(ITypeElement type, IMethod resolvedMethod)
    {
        if (typeMethods.TryGetValue(type.GetClrName(), out var overloads) && overloads.TryGetValue(resolvedMethod.ShortName, out var methods))
        {
            return TryFindMethod(methods, resolvedMethod);
        }

        return null;
    }
}