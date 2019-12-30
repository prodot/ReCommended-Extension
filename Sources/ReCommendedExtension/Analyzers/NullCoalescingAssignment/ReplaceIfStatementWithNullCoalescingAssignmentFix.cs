using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.NullCoalescingAssignment
{
    [QuickFix]
    public sealed class ReplaceIfStatementWithNullCoalescingAssignmentFix : QuickFixBase
    {
        [NotNull]
        readonly NullCoalescingAssignmentSuggestion highlighting;

        public ReplaceIfStatementWithNullCoalescingAssignmentFix([NotNull] NullCoalescingAssignmentSuggestion highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Replace with compound assignment";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.IfStatement);

                var assignment = factory.CreateStatement("$0 ??= $1;", highlighting.AssignmentDestination, highlighting.AssignedExpression);

                highlighting.IfStatement.ReplaceBy(assignment);

                return _ => { };
            }
        }
    }
}