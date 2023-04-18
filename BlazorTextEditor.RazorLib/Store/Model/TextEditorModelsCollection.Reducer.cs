using BlazorTextEditor.RazorLib.Model;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Model;

public partial class TextEditorModelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorModelsCollection ReduceRegisterAction(
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceInsertTextAction(
            TextEditorModelsCollection inModelsCollection,
            InsertTextAction insertTextAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == insertTextAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(insertTextAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceKeyboardEventAction(
            TextEditorModelsCollection inModelsCollection,
            KeyboardEventAction keyboardEventAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == keyboardEventAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(keyboardEventAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByMotionAction(
            TextEditorModelsCollection inModelsCollection,
            DeleteTextByMotionAction deleteTextByMotionAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByMotionAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByMotionAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByRangeAction(
            TextEditorModelsCollection inModelsCollection,
            DeleteTextByRangeAction deleteTextByRangeAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByRangeAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByRangeAction);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceReloadAction(
            TextEditorModelsCollection inModelsCollection,
            ReloadAction reloadAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == reloadAction.TextEditorModelKey);

            var nextTextEditor = new TextEditorModel(textEditor);
            nextTextEditor.SetContent(reloadAction.Content);
            
            nextTextEditor = textEditor.SetResourceData(
                nextTextEditor.ResourceUri,
                reloadAction.ResourceLastWriteTime);

            var nextList = inModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceSetResourceDataAction(
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDisposeAction(
            TextEditorModelsCollection inModelsCollection,
            DisposeAction disposeAction)
        {
            var textEditor = inModelsCollection.TextEditorList
                .Single(x => x.ModelKey == disposeAction.TextEditorModelKey);

            var nextList = inModelsCollection.TextEditorList
                .Remove(textEditor);

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceSetUsingRowEndingKindAction(
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

            return new TextEditorModelsCollection
            {
                TextEditorList = nextList
            };
        }
    }
}