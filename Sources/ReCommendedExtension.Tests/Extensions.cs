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
        static readonly string[] pathFragmentsToExclude = { "preview", "-rc" };

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> GetReferencedAssemblies([NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies)
            =>
                from assembly in baseReferencedAssemblies
                where pathFragmentsToExclude.All(f => assembly.IndexOf(f, StringComparison.OrdinalIgnoreCase) == -1)
                select assembly;
    }
}