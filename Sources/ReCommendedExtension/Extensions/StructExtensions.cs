using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class StructExtensions
{
    extension(IStruct type)
    {
        public bool HasDisposeMethods
        {
            get
            {
                Debug.Assert(type.IsByRefLike);

                return type.Methods.Any(method => method.IsDisposeMethodByConvention
                    || method.IsDisposeAsyncMethodByConvention
                    || method
                        .GetAttributeInstances(false)
                        .Any(attribute => attribute.GetAttributeShortName() == nameof(HandlesResourceDisposalAttribute))
                    && !method.IsStatic
                    && method.GetAccessRights() is AccessRights.INTERNAL or AccessRights.PUBLIC);
            }
        }
    }
}