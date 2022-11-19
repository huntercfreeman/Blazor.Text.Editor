using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript;

public static class TypeScriptWhitespace
{
    public static readonly ImmutableArray<string> WHITESPACE = new[]
    {
        " ",
        "\t",
        "\r",
        "\n",
    }.ToImmutableArray();
}