using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;
using MethodSignature = ReCommendedExtension.Extensions.MethodFinding.MethodSignature;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class FractionalNumberAnalyzer<N>(NumberInfo<N> numberInfo) : NumberAnalyzer<N>(numberInfo) where N : struct
{
    /// <remarks>
    /// <c>T.Round(value, 0)</c> → <c>T.Round(value)</c> (.NET 7 for floating-point types or .NET Core 2.0 for <see cref="decimal"/>)
    /// </remarks>
    void AnalyzeRound_N_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument digitsOrDecimalsArgument,
        IClrTypeName containingType)
    {
        if (digitsOrDecimalsArgument.Value.TryGetInt32Constant() == 0
            && containingType.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Math.Round), Parameters = [new Parameter(t => t.IsClrType(NumberInfo.ClrTypeName))], IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", digitsOrDecimalsArgument));
        }
    }

    /// <remarks>
    /// <c>T.Round(value, MidpointRounding.ToEven)</c> → <c>T.Round(value)</c> (.NET 7 for floating-point types or .NET Core 2.0 for <see cref="decimal"/>)
    /// </remarks>
    void AnalyzeRound_N_MidpointRounding(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument modeArgument,
        IClrTypeName containingType)
    {
        if (modeArgument.Value.TryGetMidpointRoundingConstant() == MidpointRounding.ToEven
            && containingType.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Math.Round), Parameters = [new Parameter(t => t.IsClrType(NumberInfo.ClrTypeName))], IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(MidpointRounding)}.{nameof(MidpointRounding.ToEven)} is redundant.", modeArgument));
        }
    }

    /// <remarks>
    /// <c>T.Round(value, 0, mode)</c> → <c>T.Round(value, mode)</c> (.NET Core 2.0 for <see cref="decimal"/> or <c>MathF</c>)<para/>
    /// <c>T.Round(value, d, MidpointRounding.ToEven)</c> → <c>T.Round(value, d)</c> (.NET 7 for floating-point types or .NET Core 2.0 for <see cref="decimal"/> or <c>MathF</c>)
    /// </remarks>
    void AnalyzeRound_N_Int32_MidpointRounding(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument digitsArgument,
        ICSharpArgument modeArgument,
        IClrTypeName containingType)
    {
        if (digitsArgument.Value.TryGetInt32Constant() == 0
            && containingType.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Math.Round),
                    Parameters = [new Parameter(t => t.IsClrType(NumberInfo.ClrTypeName)), ..Parameters.MidpointRounding],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", digitsArgument));
        }

        if (modeArgument.Value.TryGetMidpointRoundingConstant() == MidpointRounding.ToEven
            && containingType.HasMethod(
                new MethodSignature
                {
                    Name = nameof(Math.Round),
                    Parameters = [new Parameter(t => t.IsClrType(NumberInfo.ClrTypeName)), ..Parameters.Int32],
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(MidpointRounding)}.{nameof(MidpointRounding.ToEven)} is redundant.", modeArgument));
        }
    }

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(NumberInfo.ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "Round": // todo: nameof(IFloatingPoint<T>.Round) when available
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var xType }, { Type: var digitsType }], [_, { } digitsArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && digitsType.IsInt():

                            AnalyzeRound_N_Int32(consumer, element, digitsArgument, NumberInfo.ClrTypeName);
                            break;

                        case ([{ Type: var xType }, { Type: var modeType }], [_, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && modeType.IsMidpointRounding():

                            AnalyzeRound_N_MidpointRounding(consumer, element, modeArgument, NumberInfo.ClrTypeName);
                            break;

                        case ([{ Type: var xType }, { Type: var digitsType }, { Type: var modeType }], [_, { } digitsArgument, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && digitsType.IsInt() && modeType.IsMidpointRounding():

                            AnalyzeRound_N_Int32_MidpointRounding(consumer, element, digitsArgument, modeArgument, NumberInfo.ClrTypeName);
                            break;
                    }
                    break;
            }
        }

        if (method.ContainingType.IsClrType(ClrTypeNames.Math) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case nameof(Math.Round):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var valueType }, { Type: var digitsOrDecimalsType }], [_, { } digitsOrDecimalsArgument])
                            when valueType.IsClrType(NumberInfo.ClrTypeName) && digitsOrDecimalsType.IsInt():

                            AnalyzeRound_N_Int32(consumer, element, digitsOrDecimalsArgument, ClrTypeNames.Math);
                            break;

                        case ([{ Type: var xType }, { Type: var modeType }], [_, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && modeType.IsMidpointRounding():

                            AnalyzeRound_N_MidpointRounding(consumer, element, modeArgument, ClrTypeNames.Math);
                            break;

                        case ([{ Type: var xType }, { Type: var digitsType }, { Type: var modeType }], [_, { } digitsArgument, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && digitsType.IsInt() && modeType.IsMidpointRounding():

                            AnalyzeRound_N_Int32_MidpointRounding(consumer, element, digitsArgument, modeArgument, ClrTypeNames.Math);
                            break;
                    }
                    break;
            }
        }

        if (method.ContainingType.IsClrType(ClrTypeNames.MathF) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "Round": // todo: nameof(MathF.Round) when available
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var valueType }, { Type: var digitsOrDecimalsType }], [_, { } digitsOrDecimalsArgument])
                            when valueType.IsClrType(NumberInfo.ClrTypeName) && digitsOrDecimalsType.IsInt():

                            AnalyzeRound_N_Int32(consumer, element, digitsOrDecimalsArgument, ClrTypeNames.MathF);
                            break;

                        case ([{ Type: var xType }, { Type: var modeType }], [_, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && modeType.IsMidpointRounding():

                            AnalyzeRound_N_MidpointRounding(consumer, element, modeArgument, ClrTypeNames.MathF);
                            break;

                        case ([{ Type: var xType }, { Type: var digitsType }, { Type: var modeType }], [_, { } digitsArgument, { } modeArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && digitsType.IsInt() && modeType.IsMidpointRounding():

                            AnalyzeRound_N_Int32_MidpointRounding(consumer, element, digitsArgument, modeArgument, ClrTypeNames.MathF);
                            break;
                    }
                    break;
            }
        }
    }
}