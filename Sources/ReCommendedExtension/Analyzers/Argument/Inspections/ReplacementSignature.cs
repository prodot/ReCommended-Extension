﻿using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record ReplacementSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }

    public required int ParameterIndex { get; init; }
}