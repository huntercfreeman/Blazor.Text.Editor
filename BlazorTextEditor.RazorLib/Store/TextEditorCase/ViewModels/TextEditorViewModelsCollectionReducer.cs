using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public class TextEditorViewModelsCollectionReducer
{
    [ReducerMethod]
    public static TextEditorViewModelsCollection ReduceRegisterTextEditorViewModelAction(
        TextEditorViewModelsCollection previousTextEditorViewModelsCollection,
        RegisterTextEditorViewModelAction registerTextEditorViewModelAction)
    {
        var textEditorViewModel = previousTextEditorViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == registerTextEditorViewModelAction.TextEditorViewModelKey);

        if (textEditorViewModel is not null)
            return previousTextEditorViewModelsCollection;

        var viewModel = new TextEditorViewModel(
            registerTextEditorViewModelAction.TextEditorViewModelKey,
            registerTextEditorViewModelAction.TextEditorKey,
            registerTextEditorViewModelAction.TextEditorService);
        
        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Add(viewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }
}