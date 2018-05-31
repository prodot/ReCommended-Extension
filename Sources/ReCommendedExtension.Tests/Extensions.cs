using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ReCommendedExtension.Tests
{
    internal static class Extensions
    {
        [NotNull]
        [ItemNotNull]
        static IEnumerable<string> EnsureAssembly(
            [NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies,
            [NotNull] Assembly missingAssembly)
        {
            var assemblyLocation = missingAssembly.Location;
            var fileName = Path.GetFileName(assemblyLocation);

            var found = false;

            foreach (var assembly in baseReferencedAssemblies)
            {
                if (assembly.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                }

                yield return assembly;
            }

            if (!found)
            {
                yield return assemblyLocation;
            }
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> EnsureValueTaskAssembly([NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies)
            => baseReferencedAssemblies.EnsureAssembly(typeof(ValueTask<>).Assembly);

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> EnsureAnnotationsAssembly([NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies)
            => baseReferencedAssemblies.EnsureAssembly(typeof(NotNullAttribute).Assembly);
    }
}