using System.Globalization;
using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Argument;

[ElementProblemAnalyzer(typeof(ICSharpExpression), HighlightingTypes = [typeof(RedundantArgumentHint), typeof(RedundantArgumentRangeHint)])]
public sealed class ArgumentAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
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

    abstract record Issue
    {
        public required string Message { get; init; }
    }

    abstract record RedundantArgument : Issue
    {
        public static RedundantArgumentByPosition FormatProvider { get; } =
            new() { Condition = _ => true, Message = "Passing a format provider is redundant." };

        public static RedundantArgumentByPosition Discard { get; } =
            new() { Condition = arg => arg.IsDiscard(), Message = "Discarding is redundant." };

        public static RedundantArgumentByPosition Null { get; } =
            new() { Condition = arg => arg.Value.IsDefaultValue(), Message = "Passing null is redundant." };

        public static RedundantArgumentByPosition False { get; } = new()
        {
            Condition = arg => arg.Value.TryGetBooleanConstant() == false, Message = "Passing false is redundant.",
        };

        public static RedundantArgumentByPosition EmptyArray { get; } = new()
        {
            Condition = arg => CollectionCreation.TryFrom(arg.Value) is { Count: 0 }, Message = "Passing an empty array is redundant.",
        };

        public static RedundantArgumentByPosition ZeroInt32 { get; } = new()
        {
            Condition = arg => arg.Value.TryGetInt32Constant() == 0, Message = "Passing 0 is redundant.",
        };

        public static RedundantArgumentByPosition OneInt32 { get; } = new()
        {
            Condition = arg => arg.Value.TryGetInt32Constant() == 1, Message = "Passing 1 is redundant.",
        };

        public static RedundantArgumentByPosition ZeroInt64 { get; } = new()
        {
            Condition = arg => arg.Value.TryGetInt64Constant() == 0, Message = "Passing 0 is redundant.",
        };

        public static RedundantArgumentByPosition SpaceChar { get; } = new()
        {
            Condition = arg => arg.Value.TryGetCharConstant() == ' ', Message = "Passing ' ' is redundant.",
        };

        public static RedundantArgumentByPosition MaxValueInt32 { get; } = new()
        {
            Condition = arg => arg.Value.TryGetInt32Constant() == int.MaxValue, Message = $"Passing the int.{nameof(int.MaxValue)} is redundant.",
        };

        public static RedundantArgumentByPosition MaxValueInt64 { get; } = new()
        {
            Condition = arg => arg.Value.TryGetInt64Constant() == long.MaxValue,
            Message = $"Passing the long.{nameof(long.MaxValue)} is redundant.",
        };

        public static RedundantArgumentByPosition NumberStylesInteger { get; } = new()
        {
            Condition = arg => arg.Value.TryGetNumberStylesConstant() == NumberStyles.Integer,
            Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.",
        };

        public static RedundantArgumentByPosition NumberStylesNumber { get; } = new()
        {
            Condition = arg => arg.Value.TryGetNumberStylesConstant() == NumberStyles.Number,
            Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Number)} is redundant.",
        };

        public static RedundantArgumentByPosition NumberStylesFloat { get; } = new()
        {
            Condition = arg => arg.Value.TryGetNumberStylesConstant() == (NumberStyles.Float | NumberStyles.AllowThousands),
            Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Float)} | {nameof(NumberStyles)}.{nameof(NumberStyles.AllowThousands)} is redundant.",
        };

        public static RedundantArgumentByPosition MidpointRoundingToEven { get; } = new()
        {
            Condition = arg => arg.Value.TryGetMidpointRoundingConstant() == MidpointRounding.ToEven,
            Message = $"Passing {nameof(MidpointRounding)}.{nameof(MidpointRounding.ToEven)} is redundant.",
        };

        public static RedundantArgumentByPosition DateTimeKindUnspecified { get; } = new()
        {
            Condition = arg => arg.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified,
            Message = $"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.",
        };

        public static RedundantArgumentByPosition DateTimeStylesNone { get; } = new()
        {
            Condition = arg => arg.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None,
            Message = $"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.",
        };

        public static RedundantArgumentByPosition TimeSpanStylesNone { get; } = new()
        {
            Condition = arg => arg.Value.TryGetTimeSpanStylesConstant() == TimeSpanStyles.None,
            Message = $"Passing {nameof(TimeSpanStyles)}.{nameof(TimeSpanStyles.None)} is redundant.",
        };

        public static DuplicateArgument CharDuplicateArgument { get; } = new()
        {
            Selector = args =>
            {
                [Pure]
                IEnumerable<ICSharpArgument> Iterate()
                {
                    var set = new HashSet<char>(args.Count);

                    foreach (var argument in args)
                    {
                        if (argument?.Value.TryGetCharConstant() is { } character && !set.Add(character))
                        {
                            yield return argument;
                        }
                    }
                }

                return args is [_, _, ..] ? Iterate() : [];
            },
            Message = "The character is already passed.",
        };
    }

    sealed record RedundantArgumentByPosition : RedundantArgument
    {
        public int ParameterIndex { get; init; } = -1;

        public required Func<ICSharpArgument, bool> Condition { get; init; }

        public Func<TreeNodeCollection<ICSharpArgument?>, bool>? FurtherCondition { get; init; }

        public IReadOnlyList<Parameter>? ReplacementSignatureParameters { get; init; }
    }

    sealed record DuplicateArgument : RedundantArgument
    {
        public required Func<TreeNodeCollection<ICSharpArgument?>, IEnumerable<ICSharpArgument>> Selector { get; init; }
    }

    sealed record RedundantArgumentRange : Issue
    {
        public static RedundantArgumentRange TripleZero { get; } = new()
        {
            Condition = args => (args[0].Value.TryGetInt32Constant(), args[1].Value.TryGetInt32Constant(), args[2].Value.TryGetInt32Constant())
                == (0, 0, 0),
            Message = "Passing '0, 0, 0' is redundant.",
        };

        public static RedundantArgumentRange NullDateTimeStylesNone { get; } = new()
        {
            Condition = args => args[0].Value.IsDefaultValue() && args[1].Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None,
            Message = $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
        };

        public Range ParameterIndexRange { get; init; } = ..;

        public required Func<IReadOnlyList<ICSharpArgument>, bool> Condition { get; init; }
    }

    abstract record MemberSignature;

    sealed record ConstructorSignature : MemberSignature
    {
        public required IReadOnlyList<Parameter> Parameters { get; init; }
    }

    sealed record MethodSignature : MemberSignature
    {
        public required IReadOnlyList<Parameter> Parameters { get; init; }

        public bool IsStatic { get; init; }

        [NonNegativeValue]
        public int GenericParametersCount { get; init; }
    }

    sealed record Member
    {
        public required MemberSignature Signature { get; init; }

        public IReadOnlyCollection<RedundantArgument> RedundantArguments { get; init; } = [];

        public IReadOnlyCollection<RedundantArgumentRange> RedundantArgumentRanges { get; init; } = [];
    }

    /// <remarks>
    /// type → (member name (or "" for constructors) → member overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>> typeMembers = Initialize();

    [Pure]
    static Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>> Initialize()
    {
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
                            RedundantArguments = [defaultNumberStyles with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.NumberStyles, Parameter.IFormatProvider], IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                defaultNumberStyles with { ParameterIndex = 1 }, RedundantArgument.Null with { ParameterIndex = 2 },
                            ],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.IFormatProvider, outParameter], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, outParameter], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.IFormatProvider, outParameter], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter], IsStatic = true,
                            },
                            RedundantArguments = [defaultNumberStyles with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter],
                                IsStatic = true,
                            },
                            RedundantArguments = [defaultNumberStyles with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfByte, Parameter.NumberStyles, Parameter.IFormatProvider, outParameter],
                                IsStatic = true,
                            },
                            RedundantArguments = [defaultNumberStyles with { ParameterIndex = 1 }],
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
                        RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature = new MethodSignature { Parameters = [numberTypeParameter, Parameter.MidpointRounding], IsStatic = true },
                        RedundantArguments = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                    },
                    new Member
                    {
                        Signature =
                            new MethodSignature
                            {
                                Parameters = [numberTypeParameter, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true,
                            },
                        RedundantArguments =
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
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32], IsStatic = true },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Decimal, Parameter.MidpointRounding], IsStatic = true },
                            RedundantArguments = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.MidpointRounding], IsStatic = true },
                            RedundantArguments = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature
                                {
                                    Parameters = [Parameter.Decimal, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true,
                                },
                            RedundantArguments =
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
                            RedundantArguments =
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
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Single, Parameter.MidpointRounding], IsStatic = true },
                            RedundantArguments = [RedundantArgument.MidpointRoundingToEven with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.Single, Parameter.Int32, Parameter.MidpointRounding], IsStatic = true },
                            RedundantArguments =
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
            => new(StringComparer.Ordinal)
            {
                {
                    "", // constructors
                    [
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [Parameter.Int64, Parameter.DateTimeKind] },
                            RedundantArguments = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [Parameter.DateOnly, Parameter.TimeOnly, Parameter.DateTimeKind] },
                            RedundantArguments = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 2 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6)] },
                            RedundantArgumentRanges = [RedundantArgumentRange.TripleZero with { ParameterIndexRange = 3..6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6), Parameter.DateTimeKind] },
                            RedundantArguments = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6), Parameter.Calendar] },
                            RedundantArgumentRanges = [RedundantArgumentRange.TripleZero with { ParameterIndexRange = 3..6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7)] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.DateTimeKind] },
                            RedundantArguments =
                            [
                                RedundantArgument.ZeroInt32 with { ParameterIndex = 6 },
                                RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 7 },
                            ],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.Calendar] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature
                            {
                                Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.Calendar, Parameter.DateTimeKind],
                            },
                            RedundantArguments = [RedundantArgument.DateTimeKindUnspecified with { ParameterIndex = 8 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8)] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.Calendar] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.DateTimeKind] },
                            RedundantArguments =
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
                            RedundantArguments =
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 0 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.IFormatProvider] },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(DateTime.Parse),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                        },
                    ]
                },
                {
                    nameof(DateTime.ParseExact),
                    [
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 3 }],
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
                                Parameters =
                                [
                                    Parameter.String, Parameter.IFormatProvider, Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                        },
                    ]
                },
            };

        [Pure]
        static Dictionary<string, IReadOnlyCollection<Member>> CreateDateTimeOffsetMembers()
            => new(StringComparer.Ordinal)
            {
                {
                    "", // constructors
                    [
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 7), Parameter.TimeSpan] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 6 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.TimeSpan] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature
                            {
                                Parameters = [..Enumerable.Repeat(Parameter.Int32, 8), Parameter.Calendar, Parameter.TimeSpan],
                            },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 7 }],
                        },
                    ]
                },
                {
                    nameof(DateTimeOffset.Parse),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                                    Parameter.String,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeOffset with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                        },
                    ]
                },
                {
                    nameof(DateTimeOffset.ParseExact),
                    [
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 3 }],
                        },
                    ]
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
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 4)] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 0 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 5)] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 4 }],
                        },
                        new Member
                        {
                            Signature = new ConstructorSignature { Parameters = [..Enumerable.Repeat(Parameter.Int32, 6)] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 5 }],
                        },
                    ]
                },
                {
                    nameof(TimeSpan.Parse),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(TimeSpan.ParseExact),
                    [
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpanStyles],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.TimeSpanStyles],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
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
                                Parameters =
                                [
                                    Parameter.String, Parameter.IFormatProvider, Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                                    Parameter.TimeSpanStyles,
                                    Parameter.TimeSpan with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
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
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
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
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
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
                            RedundantArguments = [RedundantArgument.TimeSpanStylesNone with { ParameterIndex = 3 }],
                        },
                    ]
                },
            };

        [Pure]
        static Dictionary<string, IReadOnlyCollection<Member>> CreateDateOnlyMembers()
            => new(StringComparer.Ordinal)
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
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[2] == null,
                                    ParameterIndex = 1,
                                    ReplacementSignatureParameters = [Parameter.String],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 1..3 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            Signature =
                                new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.String, Parameter.String],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.String, Parameter.StringArray],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                                Parameters =
                                [
                                    Parameter.String, Parameter.IFormatProvider, Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
                        },
                    ]
                },
                {
                    "TryParseExact", // todo: nameof(DateOnly.TryParseExact) when available
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
                                    Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
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
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                                    Parameter.DateOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                    ]
                },
            };

        [Pure]
        static Dictionary<string, IReadOnlyCollection<Member>> CreateTimeOnlyMembers()
            => new(StringComparer.Ordinal)
            {
                {
                    "Add", // todo: nameof(TimeOnly.Add) when available
                    [
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.TimeSpan, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }],
                            },
                            RedundantArguments = [RedundantArgument.Discard with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    "AddHours", // todo: nameof(TimeOnly.AddHours) when available
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }] },
                            RedundantArguments = [RedundantArgument.Discard with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    "AddMinutes", // todo: nameof(TimeOnly.AddMinutes) when available
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Double, Parameter.Int32 with { Kind = ParameterKind.OUTPUT }] },
                            RedundantArguments = [RedundantArgument.Discard with { ParameterIndex = 1 }],
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
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[2] == null,
                                    ParameterIndex = 1,
                                    ReplacementSignatureParameters = [Parameter.String],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 1..3 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature { Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true },
                            RedundantArguments =
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
                            Signature =
                                new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.String, Parameter.String],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature =
                                new MethodSignature
                            {
                                Parameters = [Parameter.String, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.String, Parameter.StringArray],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.StringArray, Parameter.IFormatProvider, Parameter.DateTimeStyles,
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with
                                {
                                    FurtherCondition = args => args[3] == null,
                                    ParameterIndex = 2,
                                    ReplacementSignatureParameters = [Parameter.ReadOnlySpanOfChar, Parameter.StringArray],
                                },
                            ],
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                                Parameters =
                                [
                                    Parameter.String, Parameter.IFormatProvider, Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.Null with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.DateTimeStylesNone with { ParameterIndex = 2 }],
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
                                    Parameter.String,
                                    Parameter.String,
                                    Parameter.IFormatProvider,
                                    Parameter.DateTimeStyles,
                                    Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
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
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
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
                                    Parameter.TimeOnly with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArgumentRanges = [RedundantArgumentRange.NullDateTimeStylesNone with { ParameterIndexRange = 2..4 }],
                        },
                    ]
                },
            };

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
                            RedundantArguments = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
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
                                Parameters =
                                [
                                    Parameter.String, Parameter.IFormatProvider, Parameter.Guid with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.FormatProvider with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.ReadOnlySpanOfChar, Parameter.Boolean], IsStatic = true, GenericParametersCount = 1,
                            },
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.Type, Parameter.String, Parameter.Boolean], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 2 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters = [Parameter.Type, Parameter.ReadOnlySpanOfChar, Parameter.Boolean], IsStatic = true,
                            },
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature
                            {
                                Parameters =
                                [
                                    Parameter.ReadOnlySpanOfChar, Parameter.Boolean, Parameter.T with { Kind = ParameterKind.OUTPUT },
                                ],
                                IsStatic = true,
                                GenericParametersCount = 1,
                            },
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 1 }],
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
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.False with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.StringComparison] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(string.IndexOfAny),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(string.PadLeft),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                            RedundantArguments = [RedundantArgument.SpaceChar with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(string.PadRight),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                            RedundantArguments = [RedundantArgument.SpaceChar with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(string.Split),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                            RedundantArguments = [RedundantArgument.CharDuplicateArgument],
                        },
                    ]
                },
                {
                    nameof(string.Trim),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                            RedundantArguments =
                            [
                                RedundantArgument.Null with { ParameterIndex = 0 },
                                RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                                RedundantArgument.CharDuplicateArgument,
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
                            RedundantArguments =
                            [
                                RedundantArgument.Null with { ParameterIndex = 0 },
                                RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                                RedundantArgument.CharDuplicateArgument,
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
                            RedundantArguments =
                            [
                                RedundantArgument.Null with { ParameterIndex = 0 },
                                RedundantArgument.EmptyArray with { ParameterIndex = 0 },
                                RedundantArgument.CharDuplicateArgument,
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
                            RedundantArguments = [RedundantArgument.OneInt32 with { ParameterIndex = 1 }],
                        },
                    ]
                },
                {
                    nameof(StringBuilder.Insert),
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.String, Parameter.Int32] },
                            RedundantArguments = [RedundantArgument.OneInt32 with { ParameterIndex = 2 }],
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
                            RedundantArguments = [RedundantArgument.MaxValueInt32 with { ParameterIndex = 0 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Int32] },
                            RedundantArguments = [RedundantArgument.ZeroInt32 with { ParameterIndex = 0 }],
                        },
                    ]
                },
                {
                    "NextInt64", // todo: nameof(Random.NextInt64) when available
                    [
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                            RedundantArguments = [RedundantArgument.MaxValueInt64 with { ParameterIndex = 0 }],
                        },
                        new Member
                        {
                            Signature = new MethodSignature { Parameters = [Parameter.Int64, Parameter.Int64] },
                            RedundantArguments = [RedundantArgument.ZeroInt64 with { ParameterIndex = 0 }],
                        },
                    ]
                },
            };

        return new Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Member>>>(new ClrTypeNameEqualityComparer())
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
    }

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

    protected override void Run(ICSharpExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    IsStatic: false,
                    ContainingType: { } containingType,
                } resolvedConstructor
                && typeMembers.TryGetValue(containingType.GetClrName(), out var overloads)
                && overloads.TryGetValue("", out var members)
                && TryFindMember(members, resolvedConstructor) is { Signature: ConstructorSignature signature } constructor:
            {
                if (objectCreationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments)
                {
                    // redundant argument
                    foreach (var redundantArgument in constructor.RedundantArguments)
                    {
                        switch (redundantArgument)
                        {
                            case RedundantArgumentByPosition redundantArgumentByPosition:
                            {
                                Debug.Assert(redundantArgumentByPosition.ParameterIndex >= 0);

                                if (arguments[redundantArgumentByPosition.ParameterIndex] is { } argument
                                    && redundantArgumentByPosition.Condition(argument)
                                    && (redundantArgumentByPosition.FurtherCondition == null
                                        || redundantArgumentByPosition.FurtherCondition(arguments))
                                    && containingType.HasConstructor(
                                        new Extensions.MethodFinding.ConstructorSignature
                                        {
                                            Parameters = redundantArgumentByPosition.ReplacementSignatureParameters
                                                ?? signature.Parameters.WithoutElementAt(redundantArgumentByPosition.ParameterIndex),
                                        }))
                                {
                                    consumer.AddHighlighting(new RedundantArgumentHint(redundantArgument.Message, argument));
                                }
                                break;
                            }

                            case DuplicateArgument duplicateArgument:
                            {
                                foreach (var argument in duplicateArgument.Selector(arguments))
                                {
                                    consumer.AddHighlighting(new RedundantArgumentHint(redundantArgument.Message, argument));
                                }
                                break;
                            }
                        }
                    }

                    // redundant argument range
                    if (arguments.AsPositionalArguments() is [_, _, ..] positionalArguments)
                    {
                        foreach (var redundantArgumentRange in constructor.RedundantArgumentRanges)
                        {
                            var args = positionalArguments.GetSubrange(redundantArgumentRange.ParameterIndexRange);

                            if (redundantArgumentRange.Condition(args)
                                && containingType.HasConstructor(
                                    new Extensions.MethodFinding.ConstructorSignature
                                    {
                                        Parameters = signature.Parameters.WithoutElementsAt(redundantArgumentRange.ParameterIndexRange),
                                    }))
                            {
                                consumer.AddHighlighting(new RedundantArgumentRangeHint(redundantArgumentRange.Message, args));
                }
                        }
                    }
                }
                break;
            }

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference },
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, ContainingType: { } containingType,
                } resolvedMethod
                && typeMembers.TryGetValue(containingType.GetClrName(), out var overloads)
                && overloads.TryGetValue(resolvedMethod.ShortName, out var members)
                && TryFindMember(members, resolvedMethod) is { Signature: MethodSignature signature } method:
            {
                if (invocationExpression.TryGetArgumentsInDeclarationOrder() is [_, ..] arguments)
                {
                    // redundant argument
                    foreach (var redundantArgument in method.RedundantArguments)
                    {
                        switch (redundantArgument)
                        {
                            case RedundantArgumentByPosition redundantArgumentByPosition:
                            {
                                Debug.Assert(redundantArgumentByPosition.ParameterIndex >= 0);

                                if (arguments[redundantArgumentByPosition.ParameterIndex] is { } argument
                                    && redundantArgumentByPosition.Condition(argument)
                                    && (redundantArgumentByPosition.FurtherCondition == null
                                        || redundantArgumentByPosition.FurtherCondition(arguments))
                                    && containingType.HasMethod(
                                        new Extensions.MethodFinding.MethodSignature
                                        {
                                            Name = resolvedMethod.ShortName,
                                            Parameters =
                                                redundantArgumentByPosition.ReplacementSignatureParameters
                                                ?? signature.Parameters.WithoutElementAt(redundantArgumentByPosition.ParameterIndex),
                                            IsStatic = signature.IsStatic,
                                            GenericParametersCount = signature.GenericParametersCount,
                                        }))
                                {
                                    consumer.AddHighlighting(new RedundantArgumentHint(redundantArgument.Message, argument));
                                }
                                break;
                            }

                            case DuplicateArgument duplicateArgument:
                            {
                                foreach (var argument in duplicateArgument.Selector(arguments))
                                {
                                    consumer.AddHighlighting(new RedundantArgumentHint(redundantArgument.Message, argument));
                                }
                                break;
                            }
                        }
                    }

                    // redundant argument range
                    if (arguments.AsPositionalArguments() is [_, _, ..] positionalArguments)
                    {
                        foreach (var redundantArgumentRange in method.RedundantArgumentRanges)
                        {
                            var args = positionalArguments.GetSubrange(redundantArgumentRange.ParameterIndexRange);

                            if (redundantArgumentRange.Condition(args)
                                && containingType.HasMethod(
                                    new Extensions.MethodFinding.MethodSignature
                                    {
                                        Name = resolvedMethod.ShortName,
                                        Parameters = signature.Parameters.WithoutElementsAt(redundantArgumentRange.ParameterIndexRange),
                                        IsStatic = signature.IsStatic,
                                        GenericParametersCount = signature.GenericParametersCount,
                                    }))
                            {
                                consumer.AddHighlighting(new RedundantArgumentRangeHint(redundantArgumentRange.Message, args));
                            }
                        }
                    }
                }
                break;
            }
        }
    }
}