using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public static class JavaScriptWhitespace
{
    public static readonly ImmutableArray<string> WHITESPACE = new[]
    {
        " ",
        "\t",
        "\r",
        "\n",
    }.ToImmutableArray();
}