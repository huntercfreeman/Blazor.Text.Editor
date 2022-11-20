using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

public static class FSharpWhitespace
{
    public static readonly ImmutableArray<string> WHITESPACE = new[]
    {
        " ",
        "\t",
        "\r",
        "\n",
    }.ToImmutableArray();
}