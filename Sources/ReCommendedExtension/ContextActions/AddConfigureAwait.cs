using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
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
        readonly ICSharpContextActionDataProvider provider;

        [CanBeNull]
        ICSharpExpression expression;

        [CanBeNull]
        IUsingStatement usingStatementWithVariableDeclaration;

        public AddConfigureAwait([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public override string Text => $"Add '{nameof(Task.ConfigureAwait)}(false)'";

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            expression = null;
            usingStatementWithVariableDeclaration = null;

            switch (provider.GetSelectedElement<IAwaitReferencesOwner>())
            {
                case IAwaitExpression awaitExpression when awaitExpression.AwaitKeyword == provider.SelectedElement:
                    // await task
                    var awaitedExpression = awaitExpression.Task;
                    if (awaitedExpression.IsConfigureAwaitAvailable())
                    {
                        expression = awaitedExpression;
                    }
                    break;

                case IUsingStatement usingStatement when usingStatement.AwaitKeyword == provider.SelectedElement:
                    if (usingStatement.Expressions.Count == 1)
                    {
                        // await using (expression())
                        expression = usingStatement.Expressions[0];
                    }
                    if (usingStatement.VariableDeclarations.Count == 1 &&
                        usingStatement.VariableDeclarations[0] != null &&
                        (usingStatement.VariableDeclarations[0].IsVar || usingStatement.VariableDeclarations[0].TypeUsage != null) &&
                        usingStatement.Parent is IBlock)
                    {
                        // await using (var x = expression())
                        // await using (Type x = expression())
                        usingStatementWithVariableDeclaration = usingStatement;
                    }
                    break;

                case IMultipleLocalVariableDeclaration variableDeclaration when variableDeclaration.AwaitKeyword == provider.SelectedElement:
                    // await using var x = expression()
                    // => not supported
                    break;

                case IForeachStatement foreachStatement when foreachStatement.AwaitKeyword == provider.SelectedElement:
                    // await foreach (var item in collection)
                    expression = foreachStatement.Collection;
                    break;
            }

            return expression != null || usingStatementWithVariableDeclaration != null;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(expression != null || usingStatementWithVariableDeclaration != null);

            try
            {
                using (WriteLockCookie.Create())
                {
                    if (expression != null)
                    {
                        var factory = CSharpElementFactory.GetInstance(expression);

                        ModificationUtil.ReplaceChild(expression, factory.CreateExpression($"$0.{nameof(Task.ConfigureAwait)}(false)", expression));
                    }

                    if (usingStatementWithVariableDeclaration != null)
                    {
                        var factory = CSharpElementFactory.GetInstance(usingStatementWithVariableDeclaration);

                        var variableDeclaration = usingStatementWithVariableDeclaration.VariableDeclarations[0];
                        Debug.Assert(variableDeclaration != null);

                        usingStatementWithVariableDeclaration.RemoveVariableDeclaration(variableDeclaration);

                        string variableTypeDeclaration;
                        if (variableDeclaration.IsVar)
                        {
                            variableTypeDeclaration = "var";
                        }
                        else
                        {
                            Debug.Assert(variableDeclaration.TypeUsage != null);

                            variableTypeDeclaration = variableDeclaration.TypeUsage.GetText();
                        }

                        Debug.Assert(usingStatementWithVariableDeclaration.Parent is IBlock);

                        ((IBlock)usingStatementWithVariableDeclaration.Parent).AddStatementBefore(
                            factory.CreateStatement($"{variableTypeDeclaration} {variableDeclaration.GetText()};"),
                            usingStatementWithVariableDeclaration);

                        usingStatementWithVariableDeclaration.SetExpression(
                            factory.CreateExpression($"{variableDeclaration.DeclaredName}.{nameof(Task.ConfigureAwait)}(false)"));
                    }
                }

                return _ => { };
            }
            finally
            {
                expression = null;
                usingStatementWithVariableDeclaration = null;
            }
        }
    }
}