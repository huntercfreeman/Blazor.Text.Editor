using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Commands;

public partial class TextEditorCommandDebugDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorCommand TextEditorCommand { get; set; } = null!;
}