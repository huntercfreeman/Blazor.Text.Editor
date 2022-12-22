using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public record AddViewModelToGroupAction(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey TextEditorViewModelKey);