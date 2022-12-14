using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorFooter : TextEditorView
{
    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var textEditor = TextEditorStatesWrap.Value;
        var localTextEditorViewModel = ReplaceableTextEditorViewModel;

        if (textEditor is null ||
            localTextEditorViewModel is null)
            return;
        
        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(localTextEditorViewModel.TextEditorKey, rowEndingKind);
    }
}