using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

public record SetActiveViewModelOfGroupAction(
    TextEditorGroupKey TextEditorGroupKey,
    TextEditorViewModelKey TextEditorViewModelKey);