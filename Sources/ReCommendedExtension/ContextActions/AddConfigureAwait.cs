using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Add '.ConfigureAwait(false)' to the await expression" + ZoneMarker.Suffix,
        Description = "Adds '.ConfigureAwait(false)' to the await expression.")]
    public sealed class AddConfigureAwait : ContextActionBase
    {
        const string configureAwaitMethodName = "ConfigureAwait";

        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        IUnaryExpression awaitedExpression;

        public AddConfigureAwait([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public override string Text => $"Add '{configureAwaitMethodName}(false)'";

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            var awaitExpression = provider.GetSelectedElement<IAwaitExpression>(true, false);

            if (awaitExpression != null && awaitExpression.AwaitKeyword == provider.SelectedElement)
            {
                awaitedExpression = awaitExpression.Task;
                var typeElement = (awaitedExpression?.Type() as IDeclaredType)?.GetTypeElement();
                if (typeElement != null)
                {
                    var hasConfigureAwaitMethod = typeElement.Methods.Any(
                        method =>
                        {
                            Debug.Assert(method != null);

                            if (method.ShortName == configureAwaitMethodName && method.Parameters.Count == 1)
                            {
                                Debug.Assert(method.Parameters[0] != null);

                                return method.Parameters[0].Type.IsBool();
                            }

                            return false;
                        });

                    if (hasConfigureAwaitMethod)
                    {
                        return true;
                    }
                }
            }

            awaitedExpression = null;

            return false;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(awaitedExpression != null);

            try
            {
                using (WriteLockCookie.Create())
                {
                    var factory = CSharpElementFactory.GetInstance(awaitedExpression);

                    ModificationUtil.ReplaceChild(
                        awaitedExpression,
                        factory.CreateExpression($"$0.{configureAwaitMethodName}(false)", awaitedExpression));
                }

                return _ => { };
            }
            finally
            {
                awaitedExpression = null;
            }
        }
    }
}