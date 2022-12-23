using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public record RemoveViewModelFromGroupAction(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey TextEditorViewModelKey);