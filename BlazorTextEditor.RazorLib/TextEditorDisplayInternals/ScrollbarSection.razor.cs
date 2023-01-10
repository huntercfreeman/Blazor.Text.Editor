using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.ViewModel;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class ScrollbarSection : ComponentBase
{
    [CascadingParameter]
    public TextEditorModel TextEditorModel { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
}