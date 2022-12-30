using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public partial class VimGrammarTokenDebugDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public VimGrammarToken VimGrammarToken { get; set; } = null!;
}