using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class MethodExtensions
{
    [Pure]
    public static bool IsDisposeMethod(this IMethod method)
    {
        var disposableInterface = PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(method.Module);
        var disposeMethod = disposableInterface?.Methods.FirstOrDefault(m => m.ShortName == nameof(IDisposable.Dispose));

        return method.ContainingType is { }
            && method.ContainingType.IsDescendantOf(disposableInterface)
            && disposeMethod is { }
            && method.OverridesOrImplements(disposeMethod);
    }

    [Pure]
    public static bool IsDisposeAsyncMethod(this IMethod method)
    {
        var asyncDisposableInterface = PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(method.Module);
        var disposeAsyncMethod = asyncDisposableInterface?.Methods.FirstOrDefault(m => m.ShortName == "DisposeAsync"); // todo: use nameof(IAsyncDisposable.DisposeAsync)

        return method.ContainingType is { }
            && method.ContainingType.IsDescendantOf(asyncDisposableInterface)
            && disposeAsyncMethod is { }
            && method.OverridesOrImplements(disposeAsyncMethod);
    }

    [Pure]
    public static bool IsDisposeMethodByConvention(this IMethod method)
        => method is { ShortName: "Dispose", IsStatic: false, TypeParameters: [], Parameters: [] }
            && method.ReturnType.IsVoid()
            && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;

    [Pure]
    public static bool IsDisposeAsyncMethodByConvention(this IMethod method)
        => method is { ShortName: "DisposeAsync", IsStatic: false, TypeParameters: [], Parameters: [] }
            && method.ReturnType.IsValueTask()
            && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;
}