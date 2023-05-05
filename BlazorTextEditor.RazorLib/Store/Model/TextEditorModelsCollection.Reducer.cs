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
                .SingleOrDefault(x => x.ModelKey == forceRerenderAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == insertTextAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == keyboardEventAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == deleteTextByMotionAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == deleteTextByRangeAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == undoEditAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == redoEditAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == reloadAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == setResourceDataAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x => x.ModelKey == disposeAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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
                .SingleOrDefault(x =>
                    x.ModelKey == setUsingRowEndingKindAction.TextEditorModelKey);

            if (textEditor is null)
                return inModelsCollection;

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