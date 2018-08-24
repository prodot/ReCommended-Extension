using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace ReCommendedExtension.Tests
{
    public abstract class ContextActionAvailabilityTestBase<T> : CSharpContextActionAvailabilityTestBase<T> where T : class, IContextAction
    {
        protected sealed override IEnumerable<string> GetReferencedAssemblies(TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(targetFrameworkId) ?? Enumerable.Empty<string>()).EnsureValueTaskAssembly();

        protected override string ExtraPath => "";
    }
}