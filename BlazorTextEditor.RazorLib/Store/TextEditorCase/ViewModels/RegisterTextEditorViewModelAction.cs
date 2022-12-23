using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public record RegisterTextEditorViewModelAction(
    TextEditorViewModelKey TextEditorViewModelKey, 
    TextEditorKey TextEditorKey,
    ITextEditorService TextEditorService);