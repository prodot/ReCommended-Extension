using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Application.platforms;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;

namespace ReCommendedExtension.Tests
{
    public abstract class ContextActionExecuteTestBase<T> : CSharpContextActionExecuteTestBase<T> where T : class, IContextAction
    {
        protected override IEnumerable<string> GetReferencedAssemblies(PlatformID platformId, TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(platformId, targetFrameworkId) ?? Enumerable.Empty<string>()).GetReferencedAssemblies()
                .Append(typeof(ValueTask<>).Assembly.Location);

        protected override string ExtraPath => "";
    }
}