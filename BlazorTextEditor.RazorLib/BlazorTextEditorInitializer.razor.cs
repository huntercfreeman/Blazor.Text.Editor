using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial  class BlazorTextEditorInitializer : ComponentBase
{
    [Inject]
    private ITextEditorOptions TextEditorOptions { get; set; } = null!;
}