using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorFooter : TextEditorView
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorDisplay? TextEditorDisplay { get; set; }
    [Parameter, EditorRequired]
    public TextEditorBase? TextEditorBase { get; set; }
    [Parameter]
    public string? FileExtension { get; set; }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        if (TextEditorBase is null)
            return;
        
        var textEditorKey = TextEditorBase.Key;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(textEditorKey, rowEndingKind);
    }
}