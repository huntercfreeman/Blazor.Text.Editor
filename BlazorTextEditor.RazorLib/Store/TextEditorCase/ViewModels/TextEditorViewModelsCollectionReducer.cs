using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Virtualization;
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
            registerTextEditorViewModelAction.TextEditorService,
            VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
            true);

        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Add(viewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }

    [ReducerMethod]
    public static TextEditorViewModelsCollection ReduceSetViewModelWithAction(
        TextEditorViewModelsCollection previousTextEditorViewModelsCollection,
        SetViewModelWithAction setViewModelWithAction)
    {
        var textEditorViewModel = previousTextEditorViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == setViewModelWithAction.TextEditorViewModelKey);

        if (textEditorViewModel is null)
            return previousTextEditorViewModelsCollection;

        var nextViewModel = setViewModelWithAction.WithFunc
            .Invoke(textEditorViewModel);

        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Replace(textEditorViewModel, nextViewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }
}