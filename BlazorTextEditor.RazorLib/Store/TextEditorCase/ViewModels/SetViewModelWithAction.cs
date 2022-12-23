namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public record SetViewModelWithAction(
    TextEditorViewModelKey TextEditorViewModelKey, 
    Func<TextEditorViewModel, TextEditorViewModel> WithFunc);