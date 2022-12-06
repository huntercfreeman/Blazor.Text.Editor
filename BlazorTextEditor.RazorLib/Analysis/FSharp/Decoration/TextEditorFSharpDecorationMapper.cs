using BlazorTextEditor.RazorLib.Decoration;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.Decoration;

public class TextEditorFSharpDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (FSharpDecorationKind)decorationByte;

        return decoration switch
        {
            FSharpDecorationKind.None => string.Empty,
            FSharpDecorationKind.Keyword => "bte_keyword",
            _ => string.Empty,
        };
    }
}
