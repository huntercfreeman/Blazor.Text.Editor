using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public class StorageEffects
{
    private readonly IState<TextEditorStates> _textEditorStatesWrap;
    private readonly IJSRuntime _jsRuntime;
    
    public StorageEffects(
        ITextEditorServiceOptions textEditorServiceOptions,
        IState<TextEditorStates> textEditorStatesWrap,
        IJSRuntime jsRuntime)
    {
        _textEditorStatesWrap = textEditorStatesWrap;
        _jsRuntime = jsRuntime;
    }

    public record WriteGlobalTextEditorOptionsToLocalStorageAction(TextEditorOptions GlobalTextEditorOptions);
    
    [EffectMethod]
    public async Task HandleWriteGlobalTextEditorOptionsToLocalStorageAction(
        WriteGlobalTextEditorOptionsToLocalStorageAction writeGlobalTextEditorOptionsToLocalStorageAction,
        IDispatcher dispatcher)
    {
        var optionsJsonString = System.Text.Json.JsonSerializer
            .Serialize(writeGlobalTextEditorOptionsToLocalStorageAction.GlobalTextEditorOptions);
        
        await _jsRuntime.InvokeVoidAsync(
            "blazorTextEditor.localStorageSetItem",
            ITextEditorService.LocalStorageGlobalTextEditorOptionsKey,
            optionsJsonString);
    }
}