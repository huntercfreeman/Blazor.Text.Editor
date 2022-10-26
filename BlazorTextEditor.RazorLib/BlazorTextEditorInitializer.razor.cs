using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib;

public partial class BlazorTextEditorInitializer : ComponentBase
{
    [Inject]
    private ITextEditorServiceOptions TextEditorServiceOptions { get; set; } = null!;
}