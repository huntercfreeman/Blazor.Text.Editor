using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Virtualization;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public record SetViewVirtualizationResultAction(
    TextEditorViewModelKey TextEditorViewModelKey, 
    VirtualizationResult<List<RichCharacter>> VirtualizationResult);