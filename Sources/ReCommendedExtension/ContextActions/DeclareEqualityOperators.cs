using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Declare IEqualityOperators<TSelf, TOther, TResult>" + ZoneMarker.Suffix,
    Description = "Declare IEqualityOperators<TSelf, TOther, TResult>.")]
public sealed class DeclareEqualityOperators : ContextActionBase
{
    readonly ICSharpContextActionDataProvider provider;

    IClassLikeDeclaration? classLikeDeclaration;
    ITypeElement? equalityOperatorsInterface;

    public DeclareEqualityOperators(ICSharpContextActionDataProvider provider) => this.provider = provider;

    [MemberNotNullWhen(true, nameof(classLikeDeclaration))]
    [MemberNotNullWhen(true, nameof(equalityOperatorsInterface))]
    public override bool IsAvailable(IUserDataHolder cache)
    {
        if (provider.GetSelectedElement<IClassLikeDeclaration>(true, false) is { } classLikeDeclaration
            && classLikeDeclaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && EquatableAnalyzer.TryGetEqualityOperatorsInterface(classLikeDeclaration.GetPsiModule()) is { } equalityOperatorsInterface
            && classLikeDeclaration.DeclaredElement is { })
        {
            var type = TypeFactory.CreateType(classLikeDeclaration.DeclaredElement);

            this.classLikeDeclaration = classLikeDeclaration switch
            {
                IClassDeclaration classDeclaration when classDeclaration.SuperTypes.Any(
                        baseType => baseType.IsIEquatable()
                            && baseType.TryGetGenericParameterTypes() is [{ } equatableType]
                            && equatableType.Equals(type))

                    // check if the class doesn't implement IEqualityOperators<T,T,bool> where T is the class
                    && !classDeclaration.SuperTypes.Any(
                        baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                            && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                            && leftOperandType.Equals(type)
                            && rightOperandType.Equals(type)
                            && resultType.IsBool()) => classDeclaration,

                IStructDeclaration structDeclaration when structDeclaration.SuperTypes.Any(
                        baseType => baseType.IsIEquatable()
                            && baseType.TryGetGenericParameterTypes() is [{ } equatableType]
                            && equatableType.Equals(type))

                    // check if the struct doesn't implement IEqualityOperators<T,T,bool> where T is the struct
                    && !structDeclaration.SuperTypes.Any(
                        baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                            && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                            && leftOperandType.Equals(type)
                            && rightOperandType.Equals(type)
                            && resultType.IsBool()) => structDeclaration,

                IRecordDeclaration recordDeclaration when !recordDeclaration.SuperTypes.Any(
                    baseType => DeclaredElementEqualityComparer.TypeElementComparer.Equals(baseType.GetTypeElement(), equalityOperatorsInterface)
                        && baseType.TryGetGenericParameterTypes() is [{ } leftOperandType, { } rightOperandType, { } resultType]
                        && leftOperandType.Equals(type)
                        && rightOperandType.Equals(type)
                        && resultType.IsBool()) => recordDeclaration,

                _ => null,
            };

            this.equalityOperatorsInterface = equalityOperatorsInterface;
        }
        else
        {
            this.classLikeDeclaration = null;
            this.equalityOperatorsInterface = null;
        }

        return this.classLikeDeclaration is { };
    }

    public override string Text
    {
        get
        {
            Debug.Assert(classLikeDeclaration is { });

            return $"Declare IEqualityOperators<{classLikeDeclaration.DeclaredName}, {classLikeDeclaration.DeclaredName}, bool> interface.";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(classLikeDeclaration is { DeclaredElement: { } });
            Debug.Assert(equalityOperatorsInterface is { });

            var psiModule = classLikeDeclaration.GetPsiModule();

            var type = TypeFactory.CreateType(classLikeDeclaration.DeclaredElement);

            classLikeDeclaration.AddSuperInterface(
                TypeFactory.CreateType(
                    equalityOperatorsInterface,
                    new IType[] { type, type, TypeFactory.CreateTypeByCLRName(PredefinedType.BOOLEAN_FQN, psiModule) }),
                false);

            return _ => { };
        }
        finally
        {
            classLikeDeclaration = null;
            equalityOperatorsInterface = null;
        }
    }
}