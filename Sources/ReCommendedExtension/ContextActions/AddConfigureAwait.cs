using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        Name = "Add '." + nameof(Task.ConfigureAwait) + "(false)' to the await expression" + ZoneMarker.Suffix,
        Description = "Adds '." + nameof(Task.ConfigureAwait) + "(false)' to the await expression.")]
    public sealed class AddConfigureAwait : ContextActionBase
    {
        [NotNull]
        [ItemNotNull]
        readonly List<ICSharpExpression> expressions = new List<ICSharpExpression>();

        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        public AddConfigureAwait([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public override string Text => $"Add '{nameof(Task.ConfigureAwait)}(false)'";

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            if (expressions.Count > 0)
            {
                expressions.Clear();
            }

            switch (provider.GetSelectedElement<IAwaitReferencesOwner>())
            {
                case IAwaitExpression awaitExpression when awaitExpression.AwaitKeyword == provider.SelectedElement:
                    var awaitedExpression = awaitExpression.Task;
                    if (awaitedExpression.IsConfigureAwaitAvailable())
                    {
                        expressions.Add(awaitedExpression);
                    }
                    break;

                case IUsingStatement usingStatement when usingStatement.AwaitKeyword == provider.SelectedElement:
                    if (usingStatement.Expressions.Count > 0)
                    {
                        expressions.AddRange(usingStatement.Expressions);
                    }
                    if (usingStatement.VariableDeclarations.Count > 0)
                    {
                        expressions.AddRange(
                            from declaration in usingStatement.VariableDeclarations
                            let expressionInitializer = declaration.Initial as IExpressionInitializer
                            where expressionInitializer != null
                            select expressionInitializer.Value);
                    }
                    break;

                case IMultipleLocalVariableDeclaration variableDeclaration when variableDeclaration.AwaitKeyword == provider.SelectedElement:
                    if (variableDeclaration.UsingKeyword != null && variableDeclaration.Declarators.Count > 0)
                    {
                        expressions.AddRange(
                            from declarationMember in variableDeclaration.Declarators
                            let declaration = declarationMember as ILocalVariableDeclaration
                            where declaration != null
                            let expressionInitializer = declaration.Initial as IExpressionInitializer
                            where expressionInitializer != null
                            select expressionInitializer.Value);
                    }
                    break;

                case IForeachStatement foreachStatement when foreachStatement.AwaitKeyword == provider.SelectedElement:
                    expressions.Add(foreachStatement.Collection);
                    break;
            }

            return expressions.Count > 0;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(expressions.Count > 0);

            try
            {
                using (WriteLockCookie.Create())
                {
                    foreach (var expression in expressions)
                    {
                        var factory = CSharpElementFactory.GetInstance(expression);

                        ModificationUtil.ReplaceChild(expression, factory.CreateExpression($"$0.{nameof(Task.ConfigureAwait)}(false)", expression));
                    }
                }

                return _ => { };
            }
            finally
            {
                expressions.Clear();
            }
        }
    }
}