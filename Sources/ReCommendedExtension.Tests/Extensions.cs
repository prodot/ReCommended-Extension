using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Flavours;
using NUnit.Framework;

namespace ReCommendedExtension.Tests
{
    internal static class Extensions
    {
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