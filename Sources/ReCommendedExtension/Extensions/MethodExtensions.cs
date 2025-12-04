using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class MethodExtensions
{
    extension(IMethod method)
    {
        public bool IsDisposeMethod
        {
            get
            {
                var disposableInterface = PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(method.Module);
                var disposeMethod = disposableInterface?.Methods.FirstOrDefault(m => m.ShortName == nameof(IDisposable.Dispose));

                return method.ContainingType is { }
                    && method.ContainingType.IsDescendantOf(disposableInterface)
                    && disposeMethod is { }
                    && method.OverridesOrImplements(disposeMethod);
            }
        }

        public bool IsDisposeAsyncMethod
        {
            get
            {
                var asyncDisposableInterface = PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(method.Module);
                var disposeAsyncMethod =
                    asyncDisposableInterface?.Methods.FirstOrDefault(m => m.ShortName == "DisposeAsync"); // todo: use nameof(IAsyncDisposable.DisposeAsync)

                return method.ContainingType is { }
                    && method.ContainingType.IsDescendantOf(asyncDisposableInterface)
                    && disposeAsyncMethod is { }
                    && method.OverridesOrImplements(disposeAsyncMethod);
            }
        }

        public bool IsDisposeMethodByConvention
            => method is { ShortName: "Dispose", IsStatic: false, TypeParameters: [], Parameters: [] }
                && method.ReturnType.IsVoid()
                && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;

        public bool IsDisposeAsyncMethodByConvention
            => method is { ShortName: "DisposeAsync", IsStatic: false, TypeParameters: [], Parameters: [] }
                && method.ReturnType.IsValueTask()
                && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC;
    }
}