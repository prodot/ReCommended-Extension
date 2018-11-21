using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

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

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<string> EnsureMsTestAssembly([NotNull][ItemNotNull] this IEnumerable<string> baseReferencedAssemblies)
            => baseReferencedAssemblies.EnsureAssembly(typeof(TestMethodAttribute).Assembly);

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void PatchProjectAddMsTestProjectFlavor([NotNull] this IProject project)
        {
            // patch the project type guids (applying [TestFlavours("3AC096D0-A1C2-E12C-1390-A8335801FDAB")] doesn't work)

            var projectTypeGuids = project.ProjectProperties.ProjectTypeGuids.ToHashSet();
            if (projectTypeGuids.Add(MsTestProjectFlavor.MsTestProjectFlavorGuid))
            {
                var field = project.ProjectProperties.GetType()
                    .BaseType.GetField("myProjectTypeGuids", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(project.ProjectProperties, projectTypeGuids);
            }

            Assert.True(project.HasFlavour<MsTestProjectFlavor>());
        }
    }
}