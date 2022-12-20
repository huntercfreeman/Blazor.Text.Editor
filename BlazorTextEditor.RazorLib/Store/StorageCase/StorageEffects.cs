using BlazorALaCarte.Shared.Storage;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public class StorageEffects
{
    private readonly IState<TextEditorStates> _textEditorStatesWrap;
    private readonly IStorageProvider _storageProvider;

    public StorageEffects(
        ITextEditorServiceOptions textEditorServiceOptions,
        IState<TextEditorStates> textEditorStatesWrap,
        IStorageProvider storageProvider)
    {
        _textEditorStatesWrap = textEditorStatesWrap;
        _storageProvider = storageProvider;
    }

    public record WriteGlobalTextEditorOptionsToLocalStorageAction(TextEditorOptions GlobalTextEditorOptions);
    
    [EffectMethod]
    public async Task HandleWriteGlobalTextEditorOptionsToLocalStorageAction(
        WriteGlobalTextEditorOptionsToLocalStorageAction writeGlobalTextEditorOptionsToLocalStorageAction,
        IDispatcher dispatcher)
    {
        var optionsJsonString = System.Text.Json.JsonSerializer
            .Serialize(writeGlobalTextEditorOptionsToLocalStorageAction.GlobalTextEditorOptions);

        await _storageProvider.SetValue(
            ITextEditorService.LocalStorageGlobalTextEditorOptionsKey, 
            optionsJsonString);
    }
}