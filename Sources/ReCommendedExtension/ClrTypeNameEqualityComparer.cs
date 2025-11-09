using JetBrains.Metadata.Reader.API;

namespace ReCommendedExtension;

internal sealed class ClrTypeNameEqualityComparer : IEqualityComparer<IClrTypeName>
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