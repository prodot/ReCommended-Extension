using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [QuickFix]
    public sealed class CoerceValueRangeBoundary : QuickFixBase
    {
        [NotNull]
        readonly InvalidValueRangeBoundaryWarning highlighting;

        public CoerceValueRangeBoundary([NotNull] InvalidValueRangeBoundaryWarning highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                Debug.Assert(CSharpLanguage.Instance != null);

                var factory = CSharpElementFactory.GetInstance(highlighting.PositionParameter);

                string expression;
                switch (highlighting.Boundary)
                {
                    case ValueRangeBoundary.Lower:
                        expression = highlighting.TypeIsSigned
                            ? highlighting.Type.GetPresentableName(CSharpLanguage.Instance) + "." + nameof(int.MinValue)
                            : "0";
                        break;

                    case ValueRangeBoundary.Higher:
                        expression = highlighting.Type.GetPresentableName(CSharpLanguage.Instance) + "." + nameof(int.MaxValue);
                        break;

                    default: throw new NotSupportedException();
                }

                highlighting.PositionParameter.ReplaceBy(factory.CreateExpressionAsIs(expression));
            }

            return _ => { };
        }

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance != null);

                switch (highlighting.Boundary)
                {
                    case ValueRangeBoundary.Lower:
                        return highlighting.TypeIsSigned
                            ? $"Set the 'from' value to the '{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MinValue)}'"
                            : "Set the 'from' value to the '0'";

                    case ValueRangeBoundary.Higher:
                        return $"Set the 'to' value to the '{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MaxValue)}'";

                    default: throw new NotSupportedException();
                }
            }
        }
    }
}