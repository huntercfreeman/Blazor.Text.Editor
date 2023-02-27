using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Diff;

public static class DiffFacts
{
    public const string CssClassString = "bte_diff-presentation";
    
    public static readonly TextEditorPresentationKey PresentationKey = TextEditorPresentationKey.NewTextEditorPresentationKey();
    public static readonly TextEditorPresentationModel EmptyPresentationModel = new(
        PresentationKey,
        0,
        CssClassString,
        new TextEditorDiffDecorationMapper(),
        ImmutableList<TextEditorTextSpan>.Empty);
}