using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Add '." + ClrMethodsNames.ConfigureAwait + "(false)' to the await expression" + ZoneMarker.Suffix,
        Description = "Adds '." + ClrMethodsNames.ConfigureAwait + "(false)' to the await expression.")]
    public sealed class AddConfigureAwait : ContextActionBase
    {
        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        IUnaryExpression awaitedExpression;

        public AddConfigureAwait([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public override string Text => $"Add '{ClrMethodsNames.ConfigureAwait}(false)'";

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            var awaitExpression = provider.GetSelectedElement<IAwaitExpression>(true, false);

            if (awaitExpression != null && awaitExpression.AwaitKeyword == provider.SelectedElement)
            {
                awaitedExpression = awaitExpression.Task;

                if (awaitedExpression.IsConfigureAwaitAvailable())
                {
                    return true;
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
                        factory.CreateExpression($"$0.{ClrMethodsNames.ConfigureAwait}(false)", awaitedExpression));
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