using BlazorTextEditor.RazorLib.Model;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;

public partial class TextEditorModelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorModelsCollection ReduceRegisterTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            RegisterAction registerAction)
        {
            if (inModelsCollection.TextEditorList
                .Any(x =>
                    x.ModelKey == registerAction.TextEditorModel.ModelKey ||
                    x.ResourceUri == registerAction.TextEditorModel.ResourceUri))
            {
                return inModelsCollection;
            }

            var nextList = inModelsCollection.TextEditorList
                .Add(registerAction.TextEditorModel);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceForceRerenderAction(
            TextEditorModelsCollection inModelsCollection,
            ForceRerenderAction forceRerenderAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == forceRerenderAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformForceRerenderAction(forceRerenderAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceInsertTextTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            InsertTextAction insertTextAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == insertTextAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(insertTextAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceKeyboardEventTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            KeyboardEventAction keyboardEventAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == keyboardEventAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(keyboardEventAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByMotionTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            DeleteTextByMotionAction deleteTextByMotionAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByMotionAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByMotionAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByRangeTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            DeleteTextByRangeAction deleteTextByRangeAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByRangeAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByRangeAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceUndoEditAction(
            TextEditorModelsCollection inModelsCollection,
            UndoEditAction undoEditAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == undoEditAction.TextEditorModelKey);

            var nextTextEditor = textEditor.UndoEdit();

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceRedoEditAction(
            TextEditorModelsCollection inModelsCollection,
            RedoEditAction redoEditAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == redoEditAction.TextEditorModelKey);

            var nextTextEditor = textEditor.RedoEdit();

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceReloadTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            ReloadAction reloadAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == reloadAction.TextEditorModelKey);

            var nextTextEditor = new TextEditorModel(textEditor);
            nextTextEditor.SetContent(reloadAction.Content);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetResourceDataAction(
            TextEditorModelsCollection inModelsCollection,
            SetResourceDataAction setResourceDataAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == setResourceDataAction.TextEditorModelKey);

            var nextTextEditor = textEditor.SetResourceData(
                setResourceDataAction.ResourceUri,
                setResourceDataAction.ResourceLastWriteTime);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDisposeTextEditorModelAction(
            TextEditorModelsCollection inModelsCollection,
            DisposeAction disposeAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == disposeAction.TextEditorModelKey);

            var nextList = inModelsCollection.TextEditorList
                .Remove(textEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetUsingRowEndingKindAction(
            TextEditorModelsCollection inModelsCollection,
            SetUsingRowEndingKindAction setUsingRowEndingKindAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x =>
                    x.ModelKey == setUsingRowEndingKindAction.TextEditorModelKey);

            var nextTextEditor = textEditor
                .SetUsingRowEndingKind(setUsingRowEndingKindAction.RowEndingKind);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return inModelsCollection with
            {
                TextEditorList = nextList,
            };
        }
    }
}