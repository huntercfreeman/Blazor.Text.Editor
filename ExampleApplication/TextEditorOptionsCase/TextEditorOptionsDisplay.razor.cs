using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.TextEditorOptionsCase;

public partial class TextEditorOptionsDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorOptions TextEditorOptions { get; set; } = null!;
}