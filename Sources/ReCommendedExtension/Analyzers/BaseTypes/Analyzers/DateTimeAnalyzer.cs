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
        typeof(UseDateTimePropertySuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(UseOtherArgumentSuggestion),
        typeof(RedundantMethodInvocationHint),
    ])]
public sealed class DateTimeAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles { get; } =
        [
            Parameter.String, Parameter.String, Parameter.IFormatProvider, Parameter.DateTimeStyles,
        ];

        public static IReadOnlyList<Parameter> String_String_IFormatProvider_DateTimeStyles_outDateTime { get; } =
        [
            Parameter.String,
            Parameter.String,
            Parameter.IFormatProvider,
            Parameter.DateTimeStyles,
            Parameter.DateTime with { Kind = ParameterKind.OUTPUT },
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
    /// <c>DateTime.ParseExact(s, "R", provider, style)</c> → <c>DateTime.ParseExact(s, "R", null, style)</c>
    /// </remarks>
    static void AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(
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
    /// <c>DateTime.ParseExact(s, [format], provider, style)</c> → <c>DateTime.ParseExact(s, format, provider, style)</c><para/>
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

                foreach (var (_, s) in collectionCreation.ElementsWithStringConstants)
                {
                    set.Add(s);
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

            foreach (var (_, s) in collectionCreation.ElementsWithStringConstants)
            {
                set.Add(s);
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

                foreach (var (_, s) in collectionCreation.ElementsWithStringConstants)
                {
                    set.Add(s);
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

            foreach (var (_, s) in collectionCreation.ElementsWithStringConstants)
            {
                set.Add(s);
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

                            case nameof(DateTime.ParseExact):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }], [
                                        _, { } formatArgument, { } providerArgument,
                                    ]) when sType.IsString() && formatType.IsString() && providerType.IsIFormatProvider():
                                        AnalyzeParseExact_String_String_IFormatProvider(consumer, formatArgument, providerArgument);
                                        break;

                                    case ([{ Type: var sType }, { Type: var formatType }, { Type: var providerType }, { Type: var styleType }], [
                                            _, { } formatArgument, { } providerArgument, _,
                                        ]) when sType.IsString()
                                        && formatType.IsString()
                                        && providerType.IsIFormatProvider()
                                        && styleType.IsDateTimeStyles():

                                        AnalyzeParseExact_String_String_IFormatProvider_DateTimeStyles(consumer, formatArgument, providerArgument);
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