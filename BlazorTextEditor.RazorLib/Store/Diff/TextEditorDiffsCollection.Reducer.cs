using BlazorTextEditor.RazorLib.Diff;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Diff;

public partial class TextEditorDiffsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorDiffsCollection ReduceDisposeAction(
            TextEditorDiffsCollection inDiffsCollection,
            DisposeAction disposeAction)
        {
            var existingTextEditorDiff = inDiffsCollection.DiffModelsList
                .FirstOrDefault(x =>
                    x.DiffKey == disposeAction.DiffKey);

            if (existingTextEditorDiff is null)
                return inDiffsCollection;

            var nextList = inDiffsCollection.DiffModelsList
                .Remove(existingTextEditorDiff);

            return new TextEditorDiffsCollection
            {
                DiffModelsList = nextList
            };
        }
        
        [ReducerMethod]
        public static TextEditorDiffsCollection ReduceRegisterAction(
            TextEditorDiffsCollection inDiffsCollection,
            RegisterAction registerAction)
        {
            var existingTextEditorDiff = inDiffsCollection.DiffModelsList
                .FirstOrDefault(x =>
                    x.DiffKey == registerAction.DiffKey);

            if (existingTextEditorDiff is not null)
                return inDiffsCollection;

            var diff = new TextEditorDiffModel(
                registerAction.DiffKey,
                registerAction.BeforeViewModelKey,
                registerAction.AfterViewModelKey);

            var nextList = inDiffsCollection.DiffModelsList
                .Add(diff);

            return new TextEditorDiffsCollection
            {
                DiffModelsList = nextList
            };
        }
    }
}