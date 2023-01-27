using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Diff;

public partial class TextEditorDiffsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorDiffsCollection ReduceRegisterTextEditorDiffAction(
            TextEditorDiffsCollection inDiffsCollection,
            RegisterAction registerAction)
        {
            var existingTextEditorDiff = inDiffsCollection.DiffsList
                .FirstOrDefault(x =>
                    x.DiffKey ==
                    registerAction.DiffKey);

            if (existingTextEditorDiff is not null)
                return inDiffsCollection;

            var diff = new TextEditorDiff(
                registerAction.DiffKey,
                registerAction.BeforeViewModelKey,
                registerAction.AfterViewModelKey);

            var nextList = inDiffsCollection.DiffsList
                .Add(diff);

            return new TextEditorDiffsCollection
            {
                DiffsList = nextList
            };
        }
        
        [ReducerMethod]
        public static TextEditorDiffsCollection ReduceDisposeTextEditorDiffAction(
            TextEditorDiffsCollection inDiffsCollection,
            DisposeAction disposeAction)
        {
            var existingTextEditorDiff = inDiffsCollection.DiffsList
                .FirstOrDefault(x =>
                    x.DiffKey ==
                    disposeAction.DiffKey);

            if (existingTextEditorDiff is null)
                return inDiffsCollection;

            var nextList = inDiffsCollection.DiffsList
                .Remove(existingTextEditorDiff);

            return new TextEditorDiffsCollection
            {
                DiffsList = nextList
            };
        }
    }
}