using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ReCommendedExtension.Tests
{
    internal static class Extensions
    {
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> GetReferencedAssemblies([NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies)
            => from assembly in baseReferencedAssemblies where assembly.IndexOf("preview", StringComparison.OrdinalIgnoreCase) == -1 select assembly;
    }
}