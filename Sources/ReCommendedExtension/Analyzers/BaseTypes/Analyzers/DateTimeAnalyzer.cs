using System.Globalization;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(ICSharpExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(RedundantArgumentHint),
        typeof(RedundantArgumentRangeHint),
        typeof(UseDateTimePropertySuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantElementHint),
        typeof(RedundantMethodInvocationHint),
    ])]
public sealed class DateTimeAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> Int64 { get; } = [Parameter.Int64];

        public static IReadOnlyList<Parameter> Char { get; } = [Parameter.Char];

        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> String_IFormatProvider { get; } = [Parameter.String, Parameter.IFormatProvider];

        public static IReadOnlyList<Parameter> DateOnly_TimeOnly { get; } = [Parameter.DateOnly, Parameter.TimeOnly];

        public static IReadOnlyList<Parameter> String_outDateTime { get; } =
        [
            Parameter.String, Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_outDateTime { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_IFormatProvider_outDateTime { get; } =
        [
            Parameter.String, Parameter.IFormatProvider, Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider_outDateTime { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32 { get; } = [Parameter.Int32, Parameter.Int32, Parameter.Int32];

        public static IReadOnlyList<Parameter> ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.ReadOnlySpanOfChar, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Calendar { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Calendar,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles_outDateTime { get; } =
        [
            Parameter.String,
            Parameter.String,
            Parameter.IFormatProvider,
            Parameter.DateTimeStyles,
            Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32 { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.DateTimeKind,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Calendar { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Calendar,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32 { get; } =
        [
            Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32, Parameter.Int32,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Calendar,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32 { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.DateTimeKind,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Calendar,
            Parameter.DateTimeKind,
        ];

        public static IReadOnlyList<Parameter> Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar { get; } =
        [
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Int32,
            Parameter.Calendar,
        ];
    }

    /// <remarks>
    /// <c>new DateTime(0)</c> → <c>DateTime.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int64(IHighlightingConsumer consumer, IObjectCreationExpression objectCreationExpression, ICSharpArgument ticksArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement() && ticksArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(DateTime)}.{nameof(DateTime.MinValue)}.",
                    objectCreationExpression,
                    $"{nameof(DateTime)}.{nameof(DateTime.MinValue)}"));
        }
    }

    /// <remarks>
    /// <c>new DateTime(ticks, DateTimeKind.Unspecified)</c> → <c>new DateTime(ticks)</c>
    /// </remarks>
    static void Analyze_Ctor_Int64_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int64 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(date, time, DateTimeKind.Unspecified)</c> → <c>new DateTime(date, time)</c>
    /// </remarks>
    static void Analyze_Ctor_DateOnly_TimeOnly_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.DateOnly_TimeOnly },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, 0, 0, 0)</c> → <c>new DateTime(year, month, day)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument,
        ICSharpArgument secondArgument)
    {
        if ((hourArgument.Value.TryGetInt32Constant(), minuteArgument.Value.TryGetInt32Constant(), secondArgument.Value.TryGetInt32Constant())
            == (0, 0, 0)
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentRangeHint("Passing '0, 0, 0' is redundant.", hourArgument, secondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified)</c> → <c>new DateTime(year, month, day, hour, minute, second)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, 0, 0, 0, calendar)</c> → <c>new DateTime(year, month, day, calendar)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument hourArgument,
        ICSharpArgument minuteArgument,
        ICSharpArgument secondArgument)
    {
        if ((hourArgument.Value.TryGetInt32Constant(), minuteArgument.Value.TryGetInt32Constant(), secondArgument.Value.TryGetInt32Constant())
            == (0, 0, 0)
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Calendar },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentRangeHint("Passing '0, 0, 0' is redundant.", hourArgument, secondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, 0)</c> → <c>new DateTime(year, month, day, hour, minute, second)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument millisecondArgument)
    {
        if (millisecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", millisecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond)</c><para/>
    /// <c>new DateTime(year, month, day, hour, minute, second, 0, kind)</c> → <c>new DateTime(year, month, day, hour, minute, second, kind)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument millisecondArgument,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }

        if (millisecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", millisecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, 0, calendar)</c> → <c>new DateTime(year, month, day, hour, minute, second, calendar)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument millisecondArgument)
    {
        if (millisecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Calendar },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", millisecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, calendar, DateTimeKind.Unspecified)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, calendar)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, 0)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument)
    {
        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, DateTimeKind.Unspecified)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, microsecond)</c><para/>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, 0, kind)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, kind)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32 },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }

        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, calendar)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument)
    {
        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
    }

    /// <remarks>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, calendar, DateTimeKind.Unspecified)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, calendar)</c><para/>
    /// <c>new DateTime(year, month, day, hour, minute, second, millisecond, 0, calendar, kind)</c> → <c>new DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind)</c>
    /// </remarks>
    static void Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind(
        IHighlightingConsumer consumer,
        IObjectCreationExpression objectCreationExpression,
        ICSharpArgument microsecondArgument,
        ICSharpArgument kindArgument)
    {
        if (kindArgument.Value.TryGetDateTimeKindConstant() == DateTimeKind.Unspecified
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.", kindArgument));
        }

        if (microsecondArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.DATETIME_FQN.HasConstructor(
                new ConstructorSignature { Parameters = Parameters.Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind },
                objectCreationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", microsecondArgument));
        }
    }

    /// <remarks>
    /// <c>dateTime.Add(value)</c> → <c>dateTime + value</c>
    /// </remarks>
    static void AnalyzeAdd_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '+' operator.",
                    invocationExpression,
                    "+",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTime.AddTicks(0)</c> → <c>dateTime</c>
    /// </remarks>
    static void AnalyzeAddTicks_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(DateTime.AddTicks)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>DateTime.Now.Date</c> → <c>DateTime.Today</c>
    /// </remarks>
    static void AnalyzeDate(IHighlightingConsumer consumer, IReferenceExpression referenceExpression)
    {
        if (!referenceExpression.IsPropertyAssignment()
            && !referenceExpression.IsWithinNameofExpression()
            && referenceExpression.QualifierExpression is IReferenceExpression
            {
                Reference: var reference, QualifierExpression: var qualifierExpression,
            }
            && reference.Resolve().DeclaredElement is IProperty
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                IsStatic: true,
                ShortName: nameof(DateTime.Now),
            } property
            && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN)
            && PredefinedType.DATETIME_FQN.HasProperty(
                new PropertySignature { Name = nameof(DateTime.Today), IsStatic = true },
                referenceExpression.GetPsiModule()))
        {
            consumer.AddHighlighting(
                new UseDateTimePropertySuggestion(
                    $"Use the '{nameof(DateTime.Today)}' property.",
                    reference,
                    qualifierExpression,
                    referenceExpression,
                    nameof(DateTime.Today)));
        }
    }

    /// <remarks>
    /// <c>dateTime.Equals(value)</c> → <c>dateTime == value</c>
    /// </remarks>
    static void AnalyzeEquals_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTime.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>DateTime.Equals(t1, t2)</c> → <c>t1 == t2</c>
    /// </remarks>
    static void AnalyzeEquals_DateTime_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument t1Argument,
        ICSharpArgument t2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement() && t1Argument.Value is { } && t2Argument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    t1Argument.Value.GetText(),
                    t2Argument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTime.GetDateTimeFormats(null)</c> → <c>dateTime.GetDateTimeFormats()</c>
    /// </remarks>
    static void AnalyzeGetDateTimeFormats_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTime.GetDateTimeFormats), Parameters = [] },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>dateTime.GetDateTimeFormats(format, null)</c> → <c>dateTime.GetDateTimeFormats(format)</c>
    /// </remarks>
    static void AnalyzeGetDateTimeFormats_Char_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTime.GetDateTimeFormats), Parameters = Parameters.Char },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>dateTime.GetTypeCode()</c> → <c>TypeCode.DateTime</c>
    /// </remarks>
    static void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.DateTime)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.DateTime)}"));
        }
    }

    /// <remarks>
    /// <c>DateTime.Parse(s, null)</c> → <c>DateTime.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTime.Parse), Parameters = Parameters.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.Parse(s, provider, DateTimeStyles.None)</c> → <c>DateTime.Parse(s, provider)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTime.Parse), Parameters = Parameters.String_IFormatProvider, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.Parse(s, null)</c> → <c>DateTime.Parse(s)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.Parse), Parameters = Parameters.ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.ParseExact(s, "R", provider)</c> → <c>DateTime.ParseExact(s, "R", null)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !providerArgument.Value.IsDefaultValue()
            && providerArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    providerArgument,
                    providerArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateTime.ParseExact(s, "R", provider, style)</c> → <c>DateTime.ParseExact(s, "R", null, style)</c><para/>
    /// <c>DateTime.ParseExact(s, format, provider, DateTimeStyles.None)</c> → <c>DateTime.ParseExact(s, format, provider)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument,
        ICSharpArgument styleArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !providerArgument.Value.IsDefaultValue()
            && providerArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    providerArgument,
                    providerArgument.NameIdentifier?.Name,
                    "null"));
        }

        if (styleArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.ParseExact), Parameters = Parameters.String_String_IFormatProvider, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.ParseExact(s, [format], provider, style)</c> → <c>DateTime.ParseExact(s, format, provider, style)</c><para/>
    /// <c>DateTime.ParseExact(s, ["R", "r"], provider, style)</c> → <c>DateTime.ParseExact(s, ["R"], provider, style)</c><para/>
    /// <c>DateTime.ParseExact(s, ["R", "s"], provider, style)</c> → <c>DateTime.ParseExact(s, ["R", "s"], null, style)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.ParseExact),
                    Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles,
                    IsStatic = true,
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName, _, _] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                if (collectionCreation.AllElementsAreStringConstants)
                {
                    set.Remove("o");
                    set.Remove("O");
                    set.Remove("r");
                    set.Remove("R");
                    set.Remove("s");
                    set.Remove("u");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                providerArgument,
                                providerArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateTime.ParseExact(s, ["R", "r"], provider, style)</c> → <c>DateTime.ParseExact(s, ["R"], provider, style)</c> (.NET Core 2.1)<para/>
    /// <c>DateTime.ParseExact(s, ["R", "s"], provider, style)</c> → <c>DateTime.ParseExact(s, ["R", "s"], null, style)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }

            if (collectionCreation.AllElementsAreStringConstants)
            {
                set.Remove("o");
                set.Remove("O");
                set.Remove("r");
                set.Remove("R");
                set.Remove("s");
                set.Remove("u");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            providerArgument,
                            providerArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    /// <remarks>
    /// <c>dateTime.Subtract(value)</c> → <c>dateTime - value</c>
    /// </remarks>
    static void AnalyzeSubtract_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateTime.Subtract(value)</c> → <c>dateTime - value</c>
    /// </remarks>
    static void AnalyzeSubtract_TimeSpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '-' operator.",
                    invocationExpression,
                    "-",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParse(s, null, out result)</c> → <c>DateTime.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature { Name = nameof(DateTime.TryParse), Parameters = Parameters.String_outDateTime, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParse(s, null, out result)</c> → <c>DateTime.TryParse(s, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.TryParse), Parameters = Parameters.ReadOnlySpanOfChar_outDateTime, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParse(s, provider, DateTimeStyles.None, out result)</c> → <c>DateTime.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.TryParse), Parameters = Parameters.String_IFormatProvider_outDateTime, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParse(s, provider, DateTimeStyles.None, out result)</c> → <c>DateTime.TryParse(s, provider, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument stylesArgument)
    {
        if (stylesArgument.Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None
            && PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.TryParse), Parameters = Parameters.ReadOnlySpanOfChar_IFormatProvider_outDateTime, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.", stylesArgument));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParseExact(s, "R", provider, style, out result)</c> → <c>DateTime.TryParseExact(s, "R", null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateTime(
        IHighlightingConsumer consumer,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        if (formatArgument.Value.TryGetStringConstant() is "o" or "O" or "r" or "R" or "s" or "u"
            && !providerArgument.Value.IsDefaultValue()
            && providerArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseOtherArgumentSuggestion(
                    "The format provider is ignored (pass null instead).",
                    providerArgument,
                    providerArgument.NameIdentifier?.Name,
                    "null"));
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParseExact(s, [format], provider, style, out result)</c> → <c>DateTime.TryParseExact(s, format, provider, style, out result)</c><para/>
    /// <c>DateTime.TryParseExact(s, ["R", "r"], provider, style, out result)</c> → <c>DateTime.TryParseExact(s, ["R"], provider, style, out result)</c><para/>
    /// <c>DateTime.TryParseExact(s, ["R", "s"], provider, style, out result)</c> → <c>DateTime.TryParseExact(s, ["R", "s"], null, style, out result)</c>
    /// </remarks>
    static void AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateTime(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        switch (CollectionCreation.TryFrom(formatsArgument.Value))
        {
            case { Count: 1 } collectionCreation when PredefinedType.DATETIME_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(DateTime.TryParseExact),
                    Parameters = Parameters.String_String_IFormatProvider_DateTimeStyles_outDateTime,
                    IsStatic = true,
                },
                formatsArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherArgumentSuggestion(
                        "The only collection element should be passed directly.",
                        formatsArgument,
                        parameterNames is [_, var formatParameterName, _, _, _] ? formatParameterName : null,
                        collectionCreation.SingleElement.GetText()));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                {
                    if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                        || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                        || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                        || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                        continue;
                    }

                    if (s != "" && !set.Add(s))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                    }
                }

                if (collectionCreation.AllElementsAreStringConstants)
                {
                    set.Remove("o");
                    set.Remove("O");
                    set.Remove("r");
                    set.Remove("R");
                    set.Remove("s");
                    set.Remove("u");

                    if (set.Count == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherArgumentSuggestion(
                                "The format provider is ignored (pass null instead).",
                                providerArgument,
                                providerArgument.NameIdentifier?.Name,
                                "null"));
                    }
                }

                break;
        }
    }

    /// <remarks>
    /// <c>DateTime.TryParseExact(s, ["R", "r"], provider, style, out result)</c> → <c>DateTime.TryParseExact(s, ["R"], provider, style, out result)</c> (.NET Core 2.1)<para/>
    /// <c>DateTime.TryParseExact(s, ["R", "s"], provider, style, out result)</c> → <c>DateTime.TryParseExact(s, ["R", "s"], null, style, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateTime(
        IHighlightingConsumer consumer,
        ICSharpArgument formatsArgument,
        ICSharpArgument providerArgument)
    {
        if (CollectionCreation.TryFrom(formatsArgument.Value) is { Count: > 1 } collectionCreation)
        {
            var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

            foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
            {
                if (s is "o" or "O" && (set.Contains("o") || set.Contains("O"))
                    || s is "r" or "R" && (set.Contains("r") || set.Contains("R"))
                    || s is "m" or "M" && (set.Contains("m") || set.Contains("M"))
                    || s is "y" or "Y" && (set.Contains("y") || set.Contains("Y")))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The equivalent string is already passed.", element));
                    continue;
                }

                if (s != "" && !set.Add(s))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                }
            }

            if (collectionCreation.AllElementsAreStringConstants)
            {
                set.Remove("o");
                set.Remove("O");
                set.Remove("r");
                set.Remove("R");
                set.Remove("s");
                set.Remove("u");

                if (set.Count == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherArgumentSuggestion(
                            "The format provider is ignored (pass null instead).",
                            providerArgument,
                            providerArgument.NameIdentifier?.Name,
                            "null"));
                }
            }
        }
    }

    protected override void Run(ICSharpExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } constructor
                && constructor.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (constructor.Parameters, objectCreationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([{ Type: var ticksType }], [{ } ticksArgument]) when ticksType.IsLong():
                        Analyze_Ctor_Int64(consumer, objectCreationExpression, ticksArgument);
                        break;

                    case ([{ Type: var ticksType }, { Type: var kindType }], [_, { } kindArgument])
                        when ticksType.IsLong() && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int64_DateTimeKind(consumer, objectCreationExpression, kindArgument);
                        break;

                    case ([{ Type: var dateType }, { Type: var timeType }, { Type: var kindType }], [_, _, { } kindArgument])
                        when dateType.IsDateOnly() && timeType.IsTimeOnly() && kindType.IsDateTimeKind():

                        Analyze_Ctor_DateOnly_TimeOnly_DateTimeKind(consumer, objectCreationExpression, kindArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                        ], [_, _, _, { } hourArgument, { } minuteArgument, { } secondArgument])
                        when yearType.IsInt() && monthType.IsInt() && dayType.IsInt() && hourType.IsInt() && minuteType.IsInt() && secondType.IsInt():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32(
                            consumer,
                            objectCreationExpression,
                            hourArgument,
                            minuteArgument,
                            secondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var kindType },
                        ], [_, _, _, _, _, _, { } kindArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(consumer, objectCreationExpression, kindArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var calendarType },
                        ], [_, _, _, { } hourArgument, { } minuteArgument, { } secondArgument, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && calendarType.IsCalendar():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(
                            consumer,
                            objectCreationExpression,
                            hourArgument,
                            minuteArgument,
                            secondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                        ], [_, _, _, _, _, _, { } millisecondArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32(consumer, objectCreationExpression, millisecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var kindType },
                        ], [_, _, _, _, _, _, { } millisecondArgument, { } kindArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(
                            consumer,
                            objectCreationExpression,
                            millisecondArgument,
                            kindArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var calendarType },
                        ], [_, _, _, _, _, _, { } millisecondArgument, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && calendarType.IsCalendar():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(consumer, objectCreationExpression, millisecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var calendarType },
                            { Type: var kindType },
                        ], [_, _, _, _, _, _, _, _, { } kindArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && calendarType.IsCalendar()
                        && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind(
                            consumer,
                            objectCreationExpression,
                            kindArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32(consumer, objectCreationExpression, microsecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                            { Type: var kindType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument, { } kindArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt()
                        && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_DateTimeKind(
                            consumer,
                            objectCreationExpression,
                            microsecondArgument,
                            kindArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                            { Type: var calendarType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument, _])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt()
                        && calendarType.IsCalendar():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar(
                            consumer,
                            objectCreationExpression,
                            microsecondArgument);
                        break;

                    case ([
                            { Type: var yearType },
                            { Type: var monthType },
                            { Type: var dayType },
                            { Type: var hourType },
                            { Type: var minuteType },
                            { Type: var secondType },
                            { Type: var millisecondType },
                            { Type: var microsecondType },
                            { Type: var calendarType },
                            { Type: var kindType },
                        ], [_, _, _, _, _, _, _, { } microsecondArgument, _, { } kindArgument])
                        when yearType.IsInt()
                        && monthType.IsInt()
                        && dayType.IsInt()
                        && hourType.IsInt()
                        && minuteType.IsInt()
                        && secondType.IsInt()
                        && millisecondType.IsInt()
                        && microsecondType.IsInt()
                        && calendarType.IsCalendar()
                        && kindType.IsDateTimeKind():

                        Analyze_Ctor_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Int32_Calendar_DateTimeKind(
                            consumer,
                            objectCreationExpression,
                            microsecondArgument,
                            kindArgument);
                        break;
                }
                break;

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression,
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
                } method
                && method.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTime.Add):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsTimeSpan():
                                        AnalyzeAdd_TimeSpan(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.AddTicks):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsLong():
                                        AnalyzeAddTicks_Int64(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateTime():
                                        AnalyzeEquals_DateTime(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;

                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.GetDateTimeFormats):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var providerType }], [{ } providerArgument]) when providerType.IsIFormatProvider():
                                        AnalyzeGetDateTimeFormats_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var formatType }, { Type: var providerType }], [_, { } providerArgument])
                                        when formatType.IsChar() && providerType.IsIFormatProvider():

                                        AnalyzeGetDateTimeFormats_Char_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.GetTypeCode):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([], []): AnalyzeGetTypeCode(consumer, invocationExpression); break;
                                }
                                break;

                            case nameof(DateTime.Subtract):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateTime():
                                        AnalyzeSubtract_DateTime(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;

                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsTimeSpan():
                                        AnalyzeSubtract_TimeSpan(consumer, invocationExpression, invokedExpression, valueArgument);
                                        break;
                                }
                                break;
                        }
                        break;

                    case (_, { IsStatic: true }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTime.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var t1Type }, { Type: var t2Type }], [{ } t1Argument, { } t2Argument])
                                        when t1Type.IsDateTime() && t2Type.IsDateTime():

                                        AnalyzeEquals_DateTime_DateTime(consumer, invocationExpression, t1Argument, t2Argument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.Parse):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                        when sType.IsString() && providerType.IsIFormatProvider():

                                        AnalyzeParse_String_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var stylesType }], [_, _, { } stylesArgument])
                                        when sType.IsString() && providerType.IsIFormatProvider() && stylesType.IsDateTimeStyles():

                                        AnalyzeParse_String_IFormatProvider_DateTimeStyles(consumer, invocationExpression, stylesArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }], [_, { } providerArgument])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider():

                                        AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, invocationExpression, providerArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.ParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }], [
                                        _, { } formatArgument, { } providerArgument,
                                    ]) when sType.IsString() && formatType.IsString() && providerType.IsIFormatProvider():
                                        AnalyzeParseExact_String_String_IFormatProvider(consumer, formatArgument, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatArgument, { } providerArgument, { } styleArgument,
                                        ]) when sType.IsString()
                                        && formatType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatArgument,
                                            providerArgument,
                                            styleArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatsArgument, { } providerArgument, _,
                                        ]) when sType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatsType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatsArgument, { } providerArgument, _,
                                        ]) when sType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles(
                                            consumer,
                                            formatsArgument,
                                            providerArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.TryParse):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsDateTime():

                                        AnalyzeTryParse_String_IFormatProvider_DateTime(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, { } providerArgument, _])
                                        when sType.IsReadOnlySpanOfChar() && providerType.IsIFormatProvider() && resultType.IsDateTime():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTime(consumer, invocationExpression, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var stylesType }, { Type: var resultType }], [
                                            _, _, { } stylesArgument, _,
                                        ]) when sType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTime():

                                        AnalyzeTryParse_String_IFormatProvider_DateTimeStyles_DateTime(
                                            consumer,
                                            invocationExpression,
                                            stylesArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var providerType }, { Type: var stylesType }, { Type: var resultType }], [
                                            _, _, { } stylesArgument, _,
                                        ]) when sType.IsReadOnlySpanOfChar()
                                        && providerType.IsIFormatProvider()
                                        && stylesType.IsDateTimeStyles()
                                        && resultType.IsDateTime():

                                        AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_DateTimeStyles_DateTime(
                                            consumer,
                                            invocationExpression,
                                            stylesArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.TryParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([
                                            { Type: var sType },
                                            { Type: var formatType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatArgument, { } providerArgument, _, _])
                                        when sType.IsString()
                                        && formatType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsDateTime():

                                        AnalyzeTryParseExact_String_String_IFormatProvider_DateTimeStyles_DateTime(
                                            consumer,
                                            formatArgument,
                                            providerArgument);
                                        break;

                                    case ([
                                            { Type: var sType },
                                            { Type: var formatsType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } providerArgument, _, _]) when sType.IsString()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsDateTime():

                                        AnalyzeTryParseExact_String_StringArray_IFormatProvider_DateTimeStyles_DateTime(
                                            consumer,
                                            invocationExpression,
                                            formatsArgument,
                                            providerArgument);
                                        break;

                                    case ([
                                            { Type: var sType },
                                            { Type: var formatsType },
                                            { Type: var providerType },
                                            { Type: var styleType },
                                            { Type: var resultType },
                                        ], [_, { } formatsArgument, { } providerArgument, _, _]) when sType.IsReadOnlySpanOfChar()
                                        && formatsType.IsGenericArrayOfString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles()
                                        && resultType.IsDateTime():

                                        AnalyzeTryParseExact_ReadOnlyCSpanOfChar_StringArray_IFormatProvider_DateTimeStyles_DateTime(
                                            consumer,
                                            formatsArgument,
                                            providerArgument);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                break;

            case IReferenceExpression { Reference: var reference } referenceExpression
                when reference.Resolve().DeclaredElement is IProperty
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } property
                && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (property.ShortName)
                {
                    case nameof(DateTime.Date): AnalyzeDate(consumer, referenceExpression); break;
                }
                break;
        }
    }
}