using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTextEditor.RazorLib.Semantics;

public class SemanticFacts
{
    public const string CssClassString = "bte_semantic-presentation";

    public static readonly TextEditorPresentationKey PresentationKey = TextEditorPresentationKey.NewTextEditorPresentationKey();
    public static readonly TextEditorPresentationModel EmptyPresentationModel = new(
        PresentationKey,
        0,
        CssClassString,
        new TextEditorSemanticDecorationMapper(),
        ImmutableList<TextEditorTextSpan>.Empty);
}
