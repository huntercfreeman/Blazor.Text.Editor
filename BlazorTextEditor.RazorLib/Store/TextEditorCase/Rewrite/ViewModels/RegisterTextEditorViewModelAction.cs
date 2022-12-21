using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

public record RegisterTextEditorViewModelAction(
    TextEditorKey TextEditorKey,
    TextEditorViewModelKey TextEditorViewModelKey, 
    Func<TextEditorBase> GetTextEditorBaseFunc, 
    IJSRuntime JsRuntime);