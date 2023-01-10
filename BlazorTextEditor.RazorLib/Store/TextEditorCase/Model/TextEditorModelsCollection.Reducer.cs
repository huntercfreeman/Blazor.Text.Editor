using BlazorTextEditor.RazorLib.Model;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;

public partial class TextEditorModelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorModelsCollection ReduceRegisterTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            RegisterTextEditorModelAction registerTextEditorModelAction)
        {
            if (previousTextEditorModelsCollection.TextEditorList
                .Any(x =>
                    x.ModelKey == registerTextEditorModelAction.TextEditorModel.ModelKey ||
                    x.ResourceUri == registerTextEditorModelAction.TextEditorModel.ResourceUri))
            {
                return previousTextEditorModelsCollection;
            }

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Add(registerTextEditorModelAction.TextEditorModel);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceForceRerenderAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            ForceRerenderAction forceRerenderAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == forceRerenderAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformForceRerenderAction(forceRerenderAction);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceInsertTextTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            InsertTextTextEditorModelAction insertTextTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == insertTextTextEditorModelAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(insertTextTextEditorModelAction);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceKeyboardEventTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            KeyboardEventTextEditorModelAction keyboardEventTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == keyboardEventTextEditorModelAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(keyboardEventTextEditorModelAction);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByMotionTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            DeleteTextByMotionTextEditorModelAction deleteTextByMotionTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByMotionTextEditorModelAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByMotionTextEditorModelAction);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDeleteTextByRangeTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            DeleteTextByRangeTextEditorModelAction deleteTextByRangeTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == deleteTextByRangeTextEditorModelAction.TextEditorModelKey);

            var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByRangeTextEditorModelAction);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceUndoEditAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            UndoEditAction undoEditAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == undoEditAction.TextEditorModelKey);

            var nextTextEditor = textEditor.UndoEdit();

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceRedoEditAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            RedoEditAction redoEditAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == redoEditAction.TextEditorModelKey);

            var nextTextEditor = textEditor.RedoEdit();

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceReloadTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            ReloadTextEditorModelAction reloadTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == reloadTextEditorModelAction.TextEditorModelKey);

            var nextTextEditor = new TextEditorModel(textEditor);
            nextTextEditor.SetContent(reloadTextEditorModelAction.Content);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetResourceDataAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetResourceDataAction textEditorSetResourceDataAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == textEditorSetResourceDataAction.TextEditorModelKey);

            var nextTextEditor = textEditor.SetResourceData(
                textEditorSetResourceDataAction.ResourceUri,
                textEditorSetResourceDataAction.ResourceLastWriteTime);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceDisposeTextEditorModelAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            DisposeTextEditorModelAction disposeTextEditorModelAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x => x.ModelKey == disposeTextEditorModelAction.TextEditorModelKey);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Remove(textEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetFontSizeAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetFontSizeAction textEditorSetFontSizeAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    FontSizeInPixels = textEditorSetFontSizeAction.FontSizeInPixels,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetCursorWidthAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetCursorWidthAction textEditorSetCursorWidthAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    CursorWidthInPixels = textEditorSetCursorWidthAction.CursorWidthInPixels,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetHeightAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetHeightAction textEditorSetHeightAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    HeightInPixels = textEditorSetHeightAction.HeightInPixels,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetThemeAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetThemeAction textEditorSetThemeAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    Theme = textEditorSetThemeAction.Theme,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetKeymapAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetKeymapAction textEditorSetKeymapAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    KeymapDefinition = textEditorSetKeymapAction.KeymapDefinition,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetShowWhitespaceAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetShowWhitespaceAction textEditorSetShowWhitespaceAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    ShowWhitespace = textEditorSetShowWhitespaceAction.ShowWhitespace,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetShowNewlinesAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetShowNewlinesAction textEditorSetShowNewlinesAction)
        {
            var nextTextEditorOptions = previousTextEditorModelsCollection
                    .GlobalTextEditorOptions with
                {
                    ShowNewlines = textEditorSetShowNewlinesAction.ShowNewlines,
                };

            return previousTextEditorModelsCollection with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorModelsCollection ReduceTextEditorSetUsingRowEndingKindAction(
            TextEditorModelsCollection previousTextEditorModelsCollection,
            TextEditorSetUsingRowEndingKindAction textEditorSetUsingRowEndingKindAction)
        {
            var textEditor = previousTextEditorModelsCollection.TextEditorList
                .Single(x =>
                    x.ModelKey == textEditorSetUsingRowEndingKindAction.TextEditorModelKey);

            var nextTextEditor = textEditor
                .SetUsingRowEndingKind(textEditorSetUsingRowEndingKindAction.RowEndingKind);

            var nextList = previousTextEditorModelsCollection.TextEditorList
                .Replace(textEditor, nextTextEditor);

            return previousTextEditorModelsCollection with
            {
                TextEditorList = nextList,
            };
        }
    }
}