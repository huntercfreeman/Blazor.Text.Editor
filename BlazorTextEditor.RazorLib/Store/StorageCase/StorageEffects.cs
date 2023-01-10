using BlazorALaCarte.Shared.Storage;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.StorageCase;

public class StorageEffects
{
    private readonly IState<TextEditorModelsCollection> _textEditorModelsCollectionWrap;
    private readonly IStorageProvider _storageProvider;

    public StorageEffects(
        ITextEditorServiceOptions textEditorServiceOptions,
        IState<TextEditorModelsCollection> textEditorModelsCollectionWrap,
        IStorageProvider storageProvider)
    {
        _textEditorModelsCollectionWrap = textEditorModelsCollectionWrap;
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
            ITextEditorService.LOCAL_STORAGE_GLOBAL_TEXT_EDITOR_OPTIONS_KEY, 
            optionsJsonString);
    }
}