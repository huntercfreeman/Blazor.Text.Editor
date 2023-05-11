using BlazorTextEditor.RazorLib.Character;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.ViewModel;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.ViewModel;

public partial class TextEditorViewModelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorViewModelsCollection ReduceRegisterAction(
            TextEditorViewModelsCollection inViewModelsCollection,
            RegisterAction registerAction)
        {
            var textEditorViewModel = inViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
                x.ViewModelKey == registerAction.TextEditorViewModelKey);

            if (textEditorViewModel is not null)
                return inViewModelsCollection;

            var viewModel = new TextEditorViewModel(
                registerAction.TextEditorViewModelKey,
                registerAction.TextEditorModelKey,
                registerAction.TextEditorService,
                VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
                true,
                false,
                false)
            {
                TextEditorStateChangedKey = 
                    TextEditorStateChangedKey.NewTextEditorStateChangedKey()
            };

            var nextViewModelsList = inViewModelsCollection.ViewModelsList
                .Add(viewModel);

            return new TextEditorViewModelsCollection
            {
                ViewModelsList = nextViewModelsList
            };
        }
        
        [ReducerMethod]
        public static TextEditorViewModelsCollection ReduceDisposeAction(
            TextEditorViewModelsCollection inViewModelsCollection,
            DisposeAction disposeAction)
        {
            var foundViewModel = inViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
                x.ViewModelKey == disposeAction.TextEditorViewModelKey);

            if (foundViewModel is null)
                return inViewModelsCollection;

            var nextViewModelsList = inViewModelsCollection.ViewModelsList
                .Remove(foundViewModel);

            return new TextEditorViewModelsCollection
            {
                ViewModelsList = nextViewModelsList
            };
        }

        [ReducerMethod]
        public static TextEditorViewModelsCollection ReduceSetViewModelWithAction(
            TextEditorViewModelsCollection inViewModelsCollection,
            SetViewModelWithAction setViewModelWithAction)
        {
            var textEditorViewModel = inViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
                x.ViewModelKey == setViewModelWithAction.TextEditorViewModelKey);

            if (textEditorViewModel is null)
                return inViewModelsCollection;

            var nextViewModel = setViewModelWithAction.WithFunc
                .Invoke(textEditorViewModel);

            var nextViewModelsList = inViewModelsCollection.ViewModelsList
                .Replace(textEditorViewModel, nextViewModel);

            return new TextEditorViewModelsCollection
            {
                ViewModelsList = nextViewModelsList
            };
        }
    }
}