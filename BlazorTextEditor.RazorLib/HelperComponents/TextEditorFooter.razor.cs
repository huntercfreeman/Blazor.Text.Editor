using BlazorTextEditor.RazorLib.Row;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorFooter : TextEditorView
{
    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var textEditor = TextEditorModelsCollectionWrap.Value;
        var localTextEditorViewModel = MutableReferenceToViewModel;

        if (textEditor is null ||
            localTextEditorViewModel is null)
            return;
        
        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.Model.SetUsingRowEndingKind(
                localTextEditorViewModel.ModelKey, rowEndingKind);
    }
    
    private string StyleMinWidthFromMaxLengthOfValue(int value)
    {
        var maxLengthOfValue = value
            .ToString()
            .Length;

        var padCharacterWidthUnits = 1;

        return $"min-width: calc(1ch * {maxLengthOfValue + padCharacterWidthUnits})";
    }
}