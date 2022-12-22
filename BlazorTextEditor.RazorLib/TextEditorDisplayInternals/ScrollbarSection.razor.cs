using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditorDisplayInternals;

public partial class ScrollbarSection : ComponentBase
{
    [CascadingParameter]
    public TextEditorBase TextEditor { get; set; } = null!;
    [CascadingParameter]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [CascadingParameter]
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; set; } = null!;

    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;
}