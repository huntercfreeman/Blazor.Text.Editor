using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public record SetActiveViewModelOfGroupAction(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey TextEditorViewModelKey);