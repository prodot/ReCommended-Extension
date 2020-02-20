using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Naming;
using JetBrains.ReSharper.Feature.Services.ParameterNameHints;
using JetBrains.ReSharper.Feature.Services.TypeNameHints;

namespace ReCommendedExtension.Tests.Providers
{
    internal sealed class HighlightingTypes
    {
        [NotNull]
        [ItemNotNull]
        readonly HashSet<Type> types = new HashSet<Type>();

        public HighlightingTypes()
        {
            Add<ArrangeTypeMemberModifiersWarning>();
            Add<ArrangeTypeModifiersWarning>();
            Add<InconsistentNamingWarning>();
            Add<UnusedMemberLocalWarning>();
            Add<UnusedParameterLocalWarning>();
            Add<UnusedFieldCompilerWarning>();
            Add<NotNullMemberIsNotInitializedWarning>();
            Add<NotAccessedParameterLocalWarning>();
            Add<AssignmentNotUsedWarning>();
            Add<ParameterValueIsOverriddenWarning>();
            Add<ParameterNameHintHighlighting>();
            Add<ParameterNameHintContextActionHighlighting>();
            Add<AsyncMethodWithoutAwaitWarning>();
            Add<MemberCanBeMadeStaticLocalWarning>();
            Add<TypeNameHintHighlighting>();
            Add<TypeNameHintContextActionHighlighting>();
            Add<UnusedLocalFunctionReturnValueWarning>();
            Add<LocalFunctionCanBeMadeStaticWarning>();
            Add<ReturnValueOfPureMethodIsNotUsedWarning>();
            Add<UnassignedGetOnlyAutoPropertyWarning>();
        }

        public void Add<T>() where T : IHighlighting => types.Add(typeof(T));

        [Pure]
        public bool Contains([NotNull] IHighlighting highlighting) => types.Contains(highlighting.GetType());
    }
}