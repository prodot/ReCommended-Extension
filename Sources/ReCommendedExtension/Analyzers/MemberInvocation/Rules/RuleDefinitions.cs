using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using ReCommendedExtension.Analyzers.MemberInvocation.Inspections;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

internal static class RuleDefinitions
{
    /// <remarks>
    /// type → (member name → member overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>> typeMembers = new(new ClrTypeNameEqualityComparer())
    {
        { PredefinedType.BOOLEAN_FQN, CreateBooleanMembers() },
        { PredefinedType.BYTE_FQN, CreateNumberMembers(Parameter.Byte) },
        { PredefinedType.SBYTE_FQN, CreateSignedIntegerMembers(Parameter.SByte) },
        { PredefinedType.SHORT_FQN, CreateSignedIntegerMembers(Parameter.Int16) },
        { PredefinedType.USHORT_FQN, CreateNumberMembers(Parameter.UInt16) },
        { PredefinedType.INT_FQN, CreateSignedIntegerMembers(Parameter.Int32) },
        { PredefinedType.UINT_FQN, CreateNumberMembers(Parameter.UInt32) },
        { PredefinedType.LONG_FQN, CreateSignedIntegerMembers(Parameter.Int64) },
        { PredefinedType.ULONG_FQN, CreateNumberMembers(Parameter.UInt64) },
        { ClrTypeNames.Int128, CreateSignedIntegerMembers(Parameter.Int128) },
        { ClrTypeNames.UInt128, CreateNumberMembers(Parameter.UInt128) },
        { PredefinedType.INTPTR_FQN, CreateSignedIntegerMembers(Parameter.IntPtr) },
        { PredefinedType.UINTPTR_FQN, CreateNumberMembers(Parameter.UIntPtr) },
        { PredefinedType.DECIMAL_FQN, CreateDecimalMembers() },
        { PredefinedType.DOUBLE_FQN, CreateDoubleMembers() },
        { PredefinedType.FLOAT_FQN, CreateSingleMembers() },
        { PredefinedType.DATETIME_FQN, CreateDateTimeMembers() },
        { PredefinedType.DATETIMEOFFSET_FQN, CreateDateTimeOffsetMembers() },
        { PredefinedType.TIMESPAN_FQN, CreateTimeSpanMembers() },
        { PredefinedType.DATE_ONLY_FQN, CreateDateOnlyMembers() },
        { PredefinedType.TIME_ONLY_FQN, CreateTimeOnlyMembers() },
        { PredefinedType.GUID_FQN, CreateGuidMembers() },
        { PredefinedType.CHAR_FQN, CreateCharMembers() },
        { PredefinedType.STRING_FQN, CreateStringMembers() },
        { PredefinedType.STRING_BUILDER_FQN, CreateStringBuilderMembers() },
        { PredefinedType.GENERIC_NULLABLE_FQN, CreateNullableMembers() },
    };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateNumberMembers(Parameter numberTypeParameter)
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [numberTypeParameter] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateSignedIntegerMembers(Parameter numberTypeParameter)
    {
        var members = CreateNumberMembers(numberTypeParameter);

        members.Add(
            "IsNegative", // todo: nameof(INumberBase<T>.IsNegative) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentZero with { Operator = "<" }],
                },
            ]);

        members.Add(
            "IsPositive", // todo: nameof(INumberBase<T>.IsPositive) when available
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [numberTypeParameter], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentZero with { Operator = ">=" }],
                },
            ]);

        return members;
    }

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
                            RedundantMethodInvocation.NonBooleanConstantWithTrue,
                            BinaryOperator.QualifierArgumentNonBooleanConstants with { Operator = "==" },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDecimalMembers()
    {
        var members = CreateNumberMembers(Parameter.Decimal);

        members.Add(
            nameof(decimal.Add),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "+" }],
                },
            ]);

        members.Add(
            nameof(decimal.Divide),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "/" }],
                },
            ]);

        members.Add(
            nameof(decimal.Multiply),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "*" }],
                },
            ]);

        members.Add(
            nameof(decimal.Negate),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal], IsStatic = true },
                    Inspections = [UnaryOperator.ArgumentDecimal with { Operator = "-" }],
                },
            ]);

        members.Add(
            nameof(decimal.Remainder),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "%" }],
                },
            ]);

        members.Add(
            nameof(decimal.Subtract),
            [
                new Member
                {
                    Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.Decimal], IsStatic = true },
                    Inspections = [BinaryOperator.ArgumentsDecimal with { Operator = "-" }],
                },
            ]);

        return members;
    }

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDoubleMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(double.IsNaN),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = $"double.{nameof(double.NaN)}",
                            },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateSingleMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(float.IsNaN),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Single], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = $"float.{nameof(float.NaN)}",
                            },
                        ],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTime.Add),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                nameof(DateTime.AddTicks),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
            {
                nameof(DateTime.Date),
                [
                    new Member
                    {
                        Signature = new PropertySignature(), Inspections = [PropertyOfDateTime.NowDate with { Name = nameof(DateTime.Today) }],
                    },
                ]
            },
            {
                nameof(DateTime.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime, Parameter.DateTime], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                nameof(DateTime.Subtract),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTime] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeOffsetMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTimeOffset.Add),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.AddTicks),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset, Parameter.DateTimeOffset], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                nameof(DateTimeOffset.Subtract),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateTimeOffset] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeSpanMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(TimeSpan.Add),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "+" }],
                    },
                ]
            },
            {
                "Divide", // todo: nameof(TimeSpan.Divide) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "/" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "/" }],
                    },
                ]
            },
            {
                nameof(TimeSpan.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan, Parameter.TimeSpan], IsStatic = true },
                        Inspections = [BinaryOperator.Arguments with { Operator = "==" }],
                    },
                ]
            },
            {
                "Multiply", // todo: nameof(TimeSpan.Multiply) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Double] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "*" }],
                    },
                ]
            },
            {
                nameof(TimeSpan.Negate),
                [new Member { Signature = new MethodSignature { Parameters = [] }, Inspections = [UnaryOperator.Qualifier with { Operator = "-" }] }]
            },
            {
                nameof(TimeSpan.Subtract),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeSpan] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "-" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateDateOnlyMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "AddDays", // todo: nameof(DateOnly.AddDays) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] }, Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                "Equals", // todo: nameof(DateOnly.Equals) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.DateOnly] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeOnlyMembers()
        => new(StringComparer.Ordinal)
        {
            {
                "Equals", // todo: nameof(TimeOnly.Equals) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.TimeOnly] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateGuidMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Guid.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Guid] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateCharMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(char.Equals),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "==" }],
                    },
                ]
            },
            {
                "IsAsciiDigit", // todo: nameof(char.IsAsciiDigit) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90, ParameterIndex = 0, Pattern = ">= '0' and <= '9'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiHexDigit", // todo: nameof(char.IsAsciiHexDigit) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = ">= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiHexDigitLower", // todo: nameof(char.IsAsciiHexDigitLower) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = ">= '0' and <= '9' or >= 'a' and <= 'f'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiHexDigitUpper", // todo: nameof(char.IsAsciiHexDigitUpper) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = ">= '0' and <= '9' or >= 'A' and <= 'F'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiLetter", // todo: nameof(char.IsAsciiLetter) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = ">= 'A' and <= 'Z' or >= 'a' and <= 'z'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiLetterLower", // todo: nameof(char.IsAsciiLetterLower) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90, ParameterIndex = 0, Pattern = ">= 'a' and <= 'z'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiLetterOrDigit", // todo: nameof(char.IsAsciiLetterOrDigit) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90,
                                ParameterIndex = 0,
                                Pattern = ">= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsAsciiLetterUpper", // todo: nameof(char.IsAsciiLetterUpper) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char], IsStatic = true },
                        Inspections =
                        [
                            Pattern.ByArgument with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp90, ParameterIndex = 0, Pattern = ">= 'A' and <= 'Z'",
                            },
                        ],
                    },
                ]
            },
            {
                "IsBetween", // todo: nameof(char.IsBetween) when available
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char, Parameter.Char], IsStatic = true },
                        Inspections = [Pattern.Arg0BetweenArg1Arg2 with { MinimumLanguageVersion = CSharpLanguageLevel.CSharp90 }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateStringMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(string.EndsWith),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char] },
                        Inspections =
                        [
                            Pattern.IsLastConstantCharacter with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                            Pattern.IsLastNonConstantCharacter with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections =
                        [
                            Pattern.IsLastCharacterByCaseSensitiveString with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                            Pattern.IsLastCharacterByCaseInsensitiveString with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.IndexOf),
                [
                    new Member
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
                            Pattern.IsFirstConstantCharacterWhenComparingToZero with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110,
                            },
                            Pattern.IsFirstConstantNonCharacterWhenComparingToZero with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110,
                            },
                        ],
                    },
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                        Inspections =
                        [
                            OtherMethodInvocation.SingleElementCollectionWithFurtherArguments with
                            {
                                ReplacementMethod = new ReplacementMethod
                                {
                                    Name = nameof(string.IndexOf), Parameters = [Parameter.Char, Parameter.Int32],
                                },
                            },
                        ],
                    },
                    new Member
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
                nameof(string.LastIndexOf), [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections =
                        [
                            PropertyOfString.Arg0Empty with
                            {
                                MinimumFrameworkVersion = new Version(5, 0), // frameworks earlier than .NET 5 do not return the string length for non-empty strings (s. https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/lastindexof-improved-handling-of-empty-values)
                                Name = nameof(string.Length),
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections =
                        [
                            PropertyOfString.Arg0Empty with
                            {
                                MinimumFrameworkVersion = new Version(5, 0), // frameworks earlier than .NET 5 do not return the string length for non-empty strings (s. https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/lastindexof-improved-handling-of-empty-values)
                                Name = nameof(string.Length),
                            },
                        ],
                    },
                ]
            },
            {
                nameof(string.LastIndexOfAny),
                [
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.PadRight),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.Remove),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RangeIndexer.FromStartToArg with { MinimumLanguageVersion = CSharpLanguageLevel.CSharp80 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Int32] },
                        Inspections = [RangeIndexer.FromArg1ToEndWhenArg0Zero with { MinimumLanguageVersion = CSharpLanguageLevel.CSharp80 }],
                    },
                ]
            },
            {
                nameof(string.Replace),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.StringComparison] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStringsOrdinal],
                    },
                ]
            },
            {
                nameof(string.StartsWith),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char] },
                        Inspections =
                        [
                            Pattern.IsFirstConstantCharacter with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                            Pattern.IsFirstNonConstantCharacter with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringComparison] },
                        Inspections =
                        [
                            Pattern.IsFirstCharacterByCaseSensitiveString with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
                            Pattern.IsFirstCharacterByCaseInsensitiveString with
                            {
                                MinimumLanguageVersion = CSharpLanguageLevel.CSharp110, EnsureQualifierNotNull = true,
                            },
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
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyString with { IsPureMethod = false },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyCollection with { IsPureMethod = false },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithRepeatCountZeroInArg1 with { IsPureMethod = false }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false, EnsureFirstArgumentNotNull = true },
                        ],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false, EnsureFirstArgumentNotNull = true },
                        ],
                    },
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
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
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNullInArg1 with { IsPureMethod = false }],
                    },
                ]
            },
            {
                nameof(StringBuilder.Replace),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars with { IsPureMethod = false }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings with { IsPureMethod = false }],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Member>> CreateNullableMembers()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(Nullable<int>.GetValueOrDefault),
                [
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [] },
                        Inspections = [BinaryOperator.NullableQualifierDefault with { Operator = "??", HighlightInvokedMethodOnly = true }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.T] },
                        Inspections = [BinaryOperator.QualifierArgument with { Operator = "??", HighlightInvokedMethodOnly = true }],
                    },
                ]
            },
            { nameof(Nullable<int>.HasValue), [new Member { Signature = new PropertySignature(), Inspections = [PropertyOfNullable.HasValue] }] },
            { nameof(Nullable<int>.Value), [new Member { Signature = new PropertySignature(), Inspections = [PropertyOfNullable.Value] }] },
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
                case (MethodSignature methodSignature, IMethod resolvedMethod) when methodSignature.IsStatic == resolvedMethod.IsStatic
                    && methodSignature.GenericParametersCount == resolvedMethod.TypeParametersCount
                    && AreParametersMatching(methodSignature.Parameters, resolvedMethod.Parameters):

                case (PropertySignature propertySignature, IProperty resolvedProperty) when propertySignature.IsStatic == resolvedProperty.IsStatic:
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
    public static Member? TryGetMethod(ITypeElement type, IMethod resolvedMethod)
    {
        if (TryGetMember(type, resolvedMethod.ShortName, resolvedMethod) is { Signature: MethodSignature } method)
        {
            return method;
        }

        return null;
    }

    [Pure]
    public static Member? TryGetProperty(ITypeElement type, IProperty resolvedProperty)
    {
        if (TryGetMember(type, resolvedProperty.ShortName, resolvedProperty) is { Signature: PropertySignature } property)
        {
            return property;
        }

        return null;
    }
}