using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = $"Replace '{nameof(Task)}' return type with 'ValueTask'" + ZoneMarker.Suffix, // todo: use nameof(ValueTask)
    Description = $"Replaces '{nameof(Task)}' return type with 'ValueTask' (or vice versa) of async methods.")] // todo: use nameof(ValueTask)
public sealed class ToggleReturnTypeOfAsyncMethods(ICSharpContextActionDataProvider provider) : ContextAction<IUserTypeUsage>(provider)
{
    ICSharpDeclaration? declaration;
    IType? replacementReturnType;

    [Pure]
    static IType? TryGetReplacementType(
        IDeclaredType returnType,
        ITypeElement? taskType,
        ITypeElement? genericTaskType,
        ITypeElement? valueTaskType,
        ITypeElement? genericValueTaskType)
    {
        if (returnType.IsTask() && valueTaskType is { })
        {
            return TypeFactory.CreateType(valueTaskType);
        }

        if (returnType.IsGenericTask() && genericValueTaskType is { } && returnType.TryGetGenericParameterTypes() is [{ } taskResultType])
        {
            return TypeFactory.CreateType(genericValueTaskType, [taskResultType]);
        }

        if (returnType.IsValueTask() && taskType is { })
        {
            return TypeFactory.CreateType(taskType);
        }

        if (returnType.IsGenericValueTask() && genericTaskType is { } && returnType.TryGetGenericParameterTypes() is [{ } valueTaskResultType])
        {
            return TypeFactory.CreateType(genericTaskType, [valueTaskResultType]);
        }

        return null;
    }

    [MemberNotNullWhen(true, nameof(declaration))]
    [MemberNotNullWhen(true, nameof(replacementReturnType))]
    protected override bool IsAvailable(IUserTypeUsage selectedElement)
    {
        var taskType = PredefinedType.TASK_FQN.TryGetTypeElement(PsiModule);
        var genericTaskType = PredefinedType.GENERIC_TASK_FQN.TryGetTypeElement(PsiModule);
        var valueTaskType = PredefinedType.VALUE_TASK_FQN.TryGetTypeElement(PsiModule);
        var genericValueTaskType = PredefinedType.GENERIC_VALUE_TASK_FQN.TryGetTypeElement(PsiModule);

        switch (selectedElement)
        {
            case { Parent: IMethodDeclaration { DeclaredElement: { IsAsync: true, ReturnType: IDeclaredType returnType } } methodDeclaration }
                when !methodDeclaration.OverridesInheritedMember()
                && TryGetReplacementType(returnType, taskType, genericTaskType, valueTaskType, genericValueTaskType) is { } replacementType:

                declaration = methodDeclaration;
                replacementReturnType = replacementType;
                return true;

            case
                {
                    Parent: ILocalFunctionDeclaration
                    {
                        DeclaredElement: { IsAsync: true, ReturnType: IDeclaredType returnType },
                    } localFunctionDeclaration,
                }
                when TryGetReplacementType(returnType, taskType, genericTaskType, valueTaskType, genericValueTaskType) is { } replacementType:

                declaration = localFunctionDeclaration;
                replacementReturnType = replacementType;
                return true;
        }

        return false;
    }

    public override string Text
    {
        get
        {
            Debug.Assert(replacementReturnType is { });
            Debug.Assert(CSharpLanguage.Instance is { });

            return $"Replace with '{replacementReturnType.GetPresentableName(CSharpLanguage.Instance)}'";
        }
    }

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(declaration is IMethodDeclaration or ILocalFunctionDeclaration);
            Debug.Assert(replacementReturnType is { });

            var typeUsage = CSharpElementFactory.GetInstance(declaration).CreateTypeUsage(replacementReturnType, declaration);

            switch (declaration)
            {
                case IMethodDeclaration methodDeclaration:
                    methodDeclaration.SetTypeUsage(typeUsage);
                    break;

                case ILocalFunctionDeclaration localFunctionDeclaration:
                    localFunctionDeclaration.SetTypeUsage(typeUsage);
                    break;
            }

            return null;
        }
        finally
        {
            declaration = null;
            replacementReturnType = null;
        }
    }
}