using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MemberFinding;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Analyzers.ExpressionResult.Inspections;

internal sealed record Inspection
{
    public static Inspection NullToFalse { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.IsDefaultValueOrNull: true }] ? new ExpressionResultReplacements { Main = "false" } : null,
        Message = "The expression is always false.",
    };

    public static Inspection EmptyStringToTrue { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsStringConstant: "" }, ..] ? new ExpressionResultReplacements { Main = "true" } : null,
        Message = "The expression is always true.",
    };

    public static Inspection EmptyStringToZero { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsStringConstant: "" }, ..] ? new ExpressionResultReplacements { Main = "0" } : null,
        Message = "The expression is always 0.",
    };

    public static Inspection ZeroInArg1ToMinusOne { get; } = new()
    {
        TryGetReplacements = (_, args, _) => args is [_, { Value.AsInt32Constant: 0 }] ? new ExpressionResultReplacements { Main = "-1" } : null,
        Message = "The expression is always -1.",
    };

    public static Inspection ZeroInArg1ZeroOrOneInArg2ToMinusOne { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [_, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 or 1 }]
                ? new ExpressionResultReplacements { Main = "-1" }
                : null,
        Message = "The expression is always -1.",
    };

    public static Inspection EmptyCollectionToMinusOne { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsCollectionCreation.Count: 0 }] ? new ExpressionResultReplacements { Main = "-1" } : null,
        Message = "The expression is always -1.",
    };

    public static Inspection ZeroToEmptyString { get; } = new()
    {
        TryGetReplacements = (_, args, _) => args is [{ Value.AsInt32Constant: 0 }] ? new ExpressionResultReplacements { Main = "\"\"" } : null,
        Message = "The expression is always an empty string.",
    };

    public static Inspection EmptyCollectionInParamsArg1ToEmptyString { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [_] or [_, { Value.AsCollectionCreation.Count: 0 }]
                ? new ExpressionResultReplacements { Main = "\"\"" }
                : null,
        Message = "The expression is always an empty string.",
    };

    public static Inspection ZeroInArg2ZeroInArg3ToEmptyString { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [_, _, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                ? new ExpressionResultReplacements { Main = "\"\"" }
                : null,
        Message = "The expression is always an empty string.",
    };

    public static Inspection SingleElementCollectionInArg1OneInArg2ZeroInArg3ToEmptyString { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is
            [
                _, { Value.AsCollectionCreation.SingleExpressionElement: { } }, { Value.AsInt32Constant: 1 }, { Value.AsInt32Constant: 0 },
            ]
                ? new ExpressionResultReplacements { Main = "\"\"" }
                : null,
        Message = "The expression is always an empty string.",
    };

    public static Inspection SingleElementCollectionInArg1ZeroInArg2OneInArg3ToElement { get; } = new()
    {
        TryGetReplacements = (_, args, _)
            => args is
            [
                _,
                { Value.AsCollectionCreation.SingleExpressionElement : { } singleExpressionElement },
                { Value.AsInt32Constant: 0 },
                { Value.AsInt32Constant: 1 },
            ]
                ? new ExpressionResultReplacements { Main = singleExpressionElement.GetText() }
                : null,
        Message = "The expression is always the same as the passed collection element.",
    };

    [Pure]
    static ExpressionResultReplacements? TryGetReplacementsForSingleElementParamsCollectionInArg1(
        TreeNodeCollection<ICSharpArgument?> arguments,
        bool convertToString,
        Func<IType, bool> isCollectionTypeToExclude,
        InspectionContext context)
    {
        [Pure]
        static string CreateConversionToString(string item, CSharpLanguageLevel languageLevel)
        {
            if (languageLevel >= CSharpLanguageLevel.CSharp60)
            {
                return $"$\"{{{item}}}\"";
            }

            return $"({item}).{nameof(ToString)}()";
        }

        if (arguments is [_, var arg])
        {
            if (arg?.Value.AsCollectionCreation is { } collectionCreation)
            {
                // passed as an explicit collection creation

                if (collectionCreation.SingleExpressionElement is { } singleExpressionElement)
                {
                    return new ExpressionResultReplacements
                    {
                        Main = convertToString
                            ? CreateConversionToString(singleExpressionElement.GetText(), context.LanguageVersion)
                            : singleExpressionElement.GetText(),
                    };
                }
            }
            else
            {
                if (arg?.Value?.GetExpressionType().ToIType() is { } argType && !isCollectionTypeToExclude(argType))
                {
                    // passed not as an explicit collection creation (collection created by the "params" modifier)

                    return new ExpressionResultReplacements
                    {
                        Main = convertToString ? CreateConversionToString(arg.Value.GetText(), context.LanguageVersion) : arg.Value.GetText(),
                    };
                }
            }
        }

        return null;
    }

    public static Inspection SingleElementIEnumerableOfTInArg1ToElementConvertedToString { get; } = new()
    {
        TryGetReplacements =
            (_, args, context) => TryGetReplacementsForSingleElementParamsCollectionInArg1(args, true, _ => true, context), // not a "params" modifier
        Message = "The expression is always the same as the passed collection element converted to string.",
    };

    public static Inspection SingleElementParamsStringArrayInArg1ToElement { get; } = new()
    {
        TryGetReplacements =
            (_, args, context) => TryGetReplacementsForSingleElementParamsCollectionInArg1(
                args,
                false,
                type => type.IsGenericArrayOfString(),
                context),
        Message = "The expression is always the same as the passed collection element.",
    };

    public static Inspection SingleElementParamsReadOnlySpanOfStringInArg1ToElement { get; } = new()
    {
        TryGetReplacements =
            (_, args, context) => TryGetReplacementsForSingleElementParamsCollectionInArg1(
                args,
                false,
                type => type.IsReadOnlySpanOfString(),
                context),
        Message = "The expression is always the same as the passed collection element.",
    };

    public static Inspection SingleElementParamsObjectArrayInArg1ToElementConvertedToString { get; } = new()
    {
        TryGetReplacements =
            (_, args, context) => TryGetReplacementsForSingleElementParamsCollectionInArg1(
                args,
                true,
                type => type.IsGenericArrayOfObject(),
                context),
        Message = "The expression is always the same as the passed collection element converted to string.",
    };

    public static Inspection SingleElementParamsReadOnlySpanOfObjectInArg1ToElementConvertedToString { get; } = new()
    {
        TryGetReplacements =
            (_, args, context) => TryGetReplacementsForSingleElementParamsCollectionInArg1(
                args,
                true,
                type => type.IsReadOnlySpanOfObject(),
                context),
        Message = "The expression is always the same as the passed collection element converted to string.",
    };

    public static Inspection ToTypeCodeForBoolean { get; } = new()
    {
        TryGetReplacements = (_, _, _) => new ExpressionResultReplacements { Main = $"{nameof(TypeCode)}.{TypeCode.Boolean}" },
        Message = $"The expression is always a {nameof(TypeCode)} constant.",
    };

    public static Inspection ToTypeCodeForNumber { get; } = new()
    {
        TryGetReplacements =
            (_, _, context) => context.NumberInfo is { TypeCode: { } typeCode }
                ? new ExpressionResultReplacements { Main = $"{nameof(TypeCode)}.{typeCode}" }
                : null,
        Message = $"The expression is always a {nameof(TypeCode)} constant.",
    };

    public static Inspection ToTypeCodeForDateTime { get; } = new()
    {
        TryGetReplacements = (_, _, _) => new ExpressionResultReplacements { Main = $"{nameof(TypeCode)}.{TypeCode.DateTime}" },
        Message = $"The expression is always a {nameof(TypeCode)} constant.",
    };

    public static Inspection ToTypeCodeForChar { get; } = new()
    {
        TryGetReplacements = (_, _, _) => new ExpressionResultReplacements { Main = $"{nameof(TypeCode)}.{TypeCode.Char}" },
        Message = $"The expression is always a {nameof(TypeCode)} constant.",
    };

    public static Inspection ToTypeCodeForString { get; } = new()
    {
        TryGetReplacements = (_, _, _) => new ExpressionResultReplacements { Main = $"{nameof(TypeCode)}.{TypeCode.String}" },
        Message = $"The expression is always a {nameof(TypeCode)} constant.",
    };

    public static Inspection ZeroNonZeroToQuotientRemainder { get; } = new()
    {
        TryGetReplacements = (_, args, context) =>
        {
            if (args is [var arg0, var arg1]
                && context.NumberInfo is { IsZeroConstant: { }, IsNonZeroConstant: { }, CastZero: { } } numberInfo
                && numberInfo.IsZeroConstant(arg0?.Value)
                && numberInfo.IsNonZeroConstant(arg1?.Value))
            {
                if (context.TryGetTargetType(false).IsValueTuple(out var t1TypeArgument, out var t2TypeArgument)
                    && t1TypeArgument.IsClrType(numberInfo.ClrTypeName)
                    && t2TypeArgument.IsClrType(numberInfo.ClrTypeName))
                {
                    return new ExpressionResultReplacements { Main = "(0, 0)" };
                }

                var zero = numberInfo.CastZero(context.LanguageVersion);

                return new ExpressionResultReplacements { Main = $"(Quotient: {zero}, Remainder: {zero})" };
            }

            return null;
        },
        Message = "The expression is always (0, 0).",
    };

    public static Inspection NumberZeroToNumber { get; } = new()
    {
        TryGetReplacements = (_, args, context)
            => args is [{ Value: { } value }, { Value.AsInt32Constant: 0 }] && context.NumberInfo is { } numberInfo
                ? new ExpressionResultReplacements { Main = numberInfo.GetReplacementFromArgument(context.TryGetTargetType(false), value) }
                : null,
        Message = "The expression is always the same as the first argument.",
    };

    public static Inspection NumberMinMaxEqualToMinOrMax { get; } = new()
    {
        TryGetReplacements = (_, args, context) =>
        {
            if (args is [_, { Value: { } minArgValue }, { Value: { } maxArgValue }]
                && context.NumberInfo is { AreEqualConstants: { } } numberInfo
                && numberInfo.AreEqualConstants(minArgValue, maxArgValue))
            {
                var targetType = context.TryGetTargetType(false);

                var replacementMin = numberInfo.GetReplacementFromArgument(targetType, minArgValue);
                var replacementMax = numberInfo.GetReplacementFromArgument(targetType, maxArgValue);

                return new ExpressionResultReplacements
                {
                    Main = replacementMin, Alternative = replacementMax != replacementMin ? replacementMax : null,
                };
            }

            return null;
        },
        Message = "The expression is always the same as the second or third argument.",
    };

    public static Inspection NumberTypeMinTypeMaxToNumber { get; } = new()
    {
        TryGetReplacements = (_, args, context)
            => args is [{ Value: { } argValue }, { Value: { } minArgValue }, { Value: { } maxArgValue }]
            && context.NumberInfo is { AreMinMaxConstants: { } } numberInfo
            && numberInfo.AreMinMaxConstants(minArgValue, maxArgValue)
                ? new ExpressionResultReplacements { Main = numberInfo.GetReplacementFromArgument(context.TryGetTargetType(false), argValue) }
                : null,
        Message = "The expression is always the same as the first argument.",
    };

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Parameter names used to construct the property name.")]
    public static Inspection XYEqualToXOrY { get; } = new()
    {
        TryGetReplacements = (_, args, context) =>
        {
            if (args is [{ Value: { } xArgValue }, { Value: { } yArgValue }]
                && context.NumberInfo is { AreEqualConstants: { } } numberInfo
                && numberInfo.AreEqualConstants(xArgValue, yArgValue))
            {
                var targetType = context.TryGetTargetType(false);

                var replacementX = numberInfo.GetReplacementFromArgument(targetType, xArgValue);
                var replacementY = numberInfo.GetReplacementFromArgument(targetType, yArgValue);

                return new ExpressionResultReplacements { Main = replacementX, Alternative = replacementY != replacementX ? replacementY : null };
            }

            return null;
        },
        Message = "The expression is always the same as the first or second argument.",
    };

    public static Inspection Int32ZeroOrOneToZero { get; } = new()
    {
        TryGetReplacements = (_, args, _) => args is [{ Value.AsInt32Constant: 0 or 1 }] ? new ExpressionResultReplacements { Main = "0" } : null,
        Message = "The expression is always 0.",
    };

    public static Inspection Int64ZeroOrOneToZero { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsInt64Constant: 0 or 1 }] ? new ExpressionResultReplacements { Main = "0L" } : null,
        Message = "The expression is always 0.",
    };

    public static Inspection Int32ValueValueOrValueNextValueToValue { get; } = new()
    {
        TryGetReplacements = (_, args, context)
            => args is [{ Value.AsInt32Constant: { } minValue } minValueArg, { Value.AsInt32Constant: { } maxValue }]
            && (minValue == maxValue || minValue == unchecked(maxValue - 1))
                ? new ExpressionResultReplacements
                {
                    Main = NumberInfo.Int32.GetReplacementFromArgument(context.TryGetTargetType(false), minValueArg.Value),
                }
                : null,
        Message = "The expression is the same as the first argument.",
    };

    public static Inspection Int64ValueValueOrValueNextValueToValue { get; } = new()
    {
        TryGetReplacements = (_, args, context)
            => args is [{ Value.AsInt64Constant: { } minValue } minValueArg, { Value.AsInt64Constant: { } maxValue }]
            && (minValue == maxValue || minValue == unchecked(maxValue - 1))
                ? new ExpressionResultReplacements
                {
                    Main = NumberInfo.Int64.GetReplacementFromArgument(context.TryGetTargetType(false), minValueArg.Value),
                }
                : null,
        Message = "The expression is the same as the first argument.",
    };

    [Pure]
    static string CreateArray(IType itemType, string[] elements, InspectionContext context)
    {
        if (context.LanguageVersion >= CSharpLanguageLevel.CSharp120 && context.TryGetTargetType(true) is { })
        {
            return $"[{string.Join(", ", elements)}]";
        }

        if (elements is [])
        {
            Debug.Assert(CSharpLanguage.Instance is { });

            var type = itemType.GetPresentableName(CSharpLanguage.Instance);

            if (context.TryGetTypeElement(PredefinedType.ARRAY_FQN) is { } typeElement
                && typeElement.HasMethod(
                    new MethodSignature { Name = nameof(Array.Empty), Parameters = [], GenericParametersCount = 1, IsStatic = true }))
            {
                return $"{nameof(Array)}.{nameof(Array.Empty)}<{type}>()";
            }

            return $"new {type}[0]";
        }

        return $$"""new[] { {{string.Join(", ", elements)}} }""";
    }

    public static Inspection ArrayZeroToEmptyArray { get; } = new()
    {
        TryGetReplacements = (_, args, context) =>
        {
            if (args is [{ Value: { } arg0Value }, { Value.AsInt32Constant: 0 }])
            {
                var elementType = context.TypeArguments is [var typeArgument]
                    ? typeArgument
                    : CollectionTypeUtil.ElementTypeByCollectionType(arg0Value.Type(), arg0Value, false);

                if (elementType is { })
                {
                    return new ExpressionResultReplacements { Main = CreateArray(elementType, [], context) };
                }
            }

            return null;
        },
        Message = "The expression is always an empty array.",
    };

    public static Inspection ReadOnlySpanZeroToEmptyArray { get; } = new()
    {
        TryGetReplacements = (_, args, context) =>
        {
            if (args is [var arg0, { Value.AsInt32Constant: 0 }])
            {
                var elementType = context.TypeArguments is [var typeArgument]
                    ? typeArgument
                    : arg0 is { Value: { } arg0Value }
                        ? TypesUtil.GetTypeArgumentValue(arg0Value.Type(), 0)
                        : null;

                if (elementType is { })
                {
                    return new ExpressionResultReplacements { Main = CreateArray(elementType, [], context) };
                }
            }

            return null;
        },
        Message = "The expression is always an empty array.",
    };

    public static Inspection ZeroInArg1ToEmptyArray { get; } = new()
    {
        TryGetReplacements = (_, args, context)
            => args is [_, { Value.AsInt32Constant: 0 }, ..] && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
                ? new ExpressionResultReplacements { Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [], context) }
                : null,
        Message = "The expression is always an empty array.",
    };

    public static Inspection OneInArg1ToSingleElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => args is [_, { Value.AsInt32Constant: 1 }]
            && qualifier is { }
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [qualifier.GetText()], context),
                }
                : null,
        Message = "The expression is always an array with a single element.",
    };

    public static Inspection OneInArg1OptionalStringSplitOptionsInArg2ToSingleElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => qualifier is { }
            && args is [_, { Value.AsInt32Constant: 1 }, null or { Value.AsStringSplitOptionsConstant: StringSplitOptions.None }]
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [qualifier.GetText()], context),
                }
                : null,
        Message = "The expression is always an array with a single element.",
    };

    public static Inspection OneInArg1StringSplitOptionsInArg2ToSingleTrimmedElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => args is [_, { Value.AsInt32Constant: 1 }, { Value.AsStringSplitOptionsConstant: (StringSplitOptions)2 }] // todo: use StringSplitOptions.TrimEntries when available
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
            && stringTypeElement.HasMethod(new MethodSignature { Name = nameof(string.Trim), Parameters = [] })
            && qualifier is { }
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [$"{qualifier.GetText()}.Trim()"], context),
                }
                : null,
        Message = "The expression is always an array with a single trimmed element.",
    };

    public static Inspection NullOrEmptyStringInArg0OptionalStringSplitOptionsInLastArgToSingleElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => qualifier is { }
            && args is
            [
                { Value: { IsDefaultValueOrNull: true } or { AsStringConstant: "" } },
                ..,
                null or { Value.AsStringSplitOptionsConstant: StringSplitOptions.None },
            ]
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [qualifier.GetText()], context),
                }
                : null,
        Message = "The expression is always an array with a single element.",
    };

    public static Inspection NullOrEmptyStringInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => args is
            [
                { Value: { IsDefaultValueOrNull: true } or { AsStringConstant: "" } },
                ..,
                { Value.AsStringSplitOptionsConstant: (StringSplitOptions)2 }, // todo: use StringSplitOptions.TrimEntries when available
            ]
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
            && stringTypeElement.HasMethod(new MethodSignature { Name = nameof(string.Trim), Parameters = [] })
            && qualifier is { }
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [$"{qualifier.GetText()}.Trim()"], context),
                }
                : null,
        Message = "The expression is always an array with a single trimmed element.",
    };

    public static Inspection EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context)
            => qualifier is { }
            && args is
            [
                { Value.AsCollectionCreation: { Count: > 0 } collectionCreation },
                ..,
                { Value.AsStringSplitOptionsConstant: StringSplitOptions.None },
            ]
            && collectionCreation.AllElementsAsStringConstants.All(item => item == "")
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [qualifier.GetText()], context),
                }
                : null,
        Message = "The expression is always an array with a single element.",
    };

    public static Inspection EmptyStringCollectionInArg0StringSplitOptionsInLastArgToSingleTrimmedElementArray { get; } = new()
    {
        TryGetReplacements = (qualifier, args, context) => qualifier is { }
            && args is
            [
                { Value.AsCollectionCreation: { Count: > 0 } collectionCreation },
                ..,
                { Value.AsStringSplitOptionsConstant: (StringSplitOptions)2 }, // todo: use StringSplitOptions.TrimEntries when available
            ]
            && collectionCreation.AllElementsAsStringConstants.All(item => item == "")
            && context.TryGetTypeElement(PredefinedType.STRING_FQN) is { } stringTypeElement
            && stringTypeElement.HasMethod(new MethodSignature { Name = nameof(string.Trim), Parameters = [] })
                ? new ExpressionResultReplacements
                {
                    Main = CreateArray(TypeFactory.CreateType(stringTypeElement), [$"{qualifier.GetText()}.Trim()"], context),
                }
                : null,
        Message = "The expression is always an array with a single trimmed element.",
    };

    public static Inspection NonBooleanConstantWithFalseToInvertedQualifier { get; } = new()
    {
        TryGetReplacements =
            (qualifier, args, _) => qualifier is { AsBooleanConstant: null } && args is [{ Value.AsBooleanConstant: false }]
                ? new ExpressionResultReplacements { Main = $"!{qualifier.GetText()}" }
                : null,
        Message = "The expression is always the same as the inverted qualifier.",
    };

    public static Inspection TrueWithNonBooleanConstantToArg { get; } = new()
    {
        TryGetReplacements =
            (qualifier, args, _) => qualifier.AsBooleanConstant == true && args is [{ Value: { AsBooleanConstant: null } value }]
                ? new ExpressionResultReplacements { Main = $"{value.GetText()}" }
                : null,
        Message = "The expression is always the same as the argument.",
    };

    public static Inspection FalseWithNonBooleanConstantToInvertedArg { get; } = new()
    {
        TryGetReplacements =
            (qualifier, args, _) => qualifier.AsBooleanConstant == false && args is [{ Value: { AsBooleanConstant: null } value }]
                ? new ExpressionResultReplacements { Main = $"!({value.GetText()})" }
                : null,
        Message = "The expression is always the same as the inverted argument.",
    };

    public static Inspection ZeroToDateTimeMinValue { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsInt64Constant: 0 }]
                ? new ExpressionResultReplacements { Main = $"{nameof(DateTime)}.{nameof(DateTime.MinValue)}" }
                : null,
        Message = $"The expression is always {nameof(DateTime)}.{nameof(DateTime.MinValue)}.",
    };

    public static Inspection ZerosToTimeOnlyMinValue { get; } = new()
    {
        TryGetReplacements = (_, args, _)
            => args is [{ Value.AsInt64Constant: 0 }]
                or [{ Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                or [{ Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                or [{ Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                or
                [
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                ]
                ? new ExpressionResultReplacements { Main = "TimeOnly.MinValue" } // todo: nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
                : null,
        Message = "The expression is always TimeOnly.MinValue.", // todo: nameof(TimeOnly), nameof(TimeOnly.MinValue) when available
    };

    public static Inspection ZerosToTimeSpanZero { get; } = new()
    {
        TryGetReplacements = (_, args, _)
            => args is [{ Value.AsInt64Constant: 0 }]
                or [{ Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                or [{ Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }, { Value.AsInt32Constant: 0 }]
                or
                [
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                ]
                or
                [
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                    { Value.AsInt32Constant: 0 },
                ]
                ? new ExpressionResultReplacements { Main = $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}" }
                : null,
        Message = $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
    };

    public static Inspection Int32ZerosOptionalToTimeSpanZero { get; } = new()
    {
        TryGetReplacements = (_, args, _)
            => args is [{ Value.AsInt32Constant: 0 }]
                or
                [
                    { Value.AsInt32Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                ]
                or
                [
                    { Value.AsInt32Constant: 0 },
                    null or { Value.AsInt32Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                ]
                ? new ExpressionResultReplacements { Main = $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}" }
                : null,
        Message = $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
    };

    public static Inspection Int64ZerosOptionalToTimeSpanZero { get; } = new()
    {
        TryGetReplacements = (_, args, _)
            => args is [{ Value.AsInt64Constant: 0 }]
                or [{ Value.AsInt64Constant: 0 }, null or { Value.AsInt64Constant: 0 }]
                or [{ Value.AsInt64Constant: 0 }, null or { Value.AsInt64Constant: 0 }, null or { Value.AsInt64Constant: 0 }]
                or
                [
                    { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                    null or { Value.AsInt64Constant: 0 },
                ]
                ? new ExpressionResultReplacements { Main = $"{nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}" }
                : null,
        Message = $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.",
    };

    public static Inspection Int64MinValueToTimeSpanMinValue { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsInt64Constant: long.MinValue }]
                ? new ExpressionResultReplacements { Main = $"{nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}" }
                : null,
        Message = $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MinValue)}.",
    };

    public static Inspection Int64MaxValueToTimeSpanMaxValue { get; } = new()
    {
        TryGetReplacements =
            (_, args, _) => args is [{ Value.AsInt64Constant: long.MaxValue }]
                ? new ExpressionResultReplacements { Main = $"{nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}" }
                : null,
        Message = $"The expression is always {nameof(TimeSpan)}.{nameof(TimeSpan.MaxValue)}.",
    };

    public bool EnsureQualifierNotNull { get; init; }

    public bool EnsureFirstArgumentNotNull { get; init; }

    public Version? MinimumFrameworkVersion { get; init; }

    public required Func<ICSharpExpression?, TreeNodeCollection<ICSharpArgument?>, InspectionContext, ExpressionResultReplacements?> TryGetReplacements { get; init; }

    public required string Message { get; init; }
}