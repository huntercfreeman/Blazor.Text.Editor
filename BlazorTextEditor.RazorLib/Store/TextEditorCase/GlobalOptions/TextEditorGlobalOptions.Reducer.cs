using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.GlobalOptions;

public partial class TextEditorGlobalOptions
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetFontSizeAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetFontSizeAction setFontSizeAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    FontSizeInPixels = setFontSizeAction.FontSizeInPixels,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetCursorWidthAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetCursorWidthAction setCursorWidthAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    CursorWidthInPixels = setCursorWidthAction.CursorWidthInPixels,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetHeightAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetHeightAction setHeightAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    HeightInPixels = setHeightAction.HeightInPixels,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetThemeAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetThemeAction setThemeAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    Theme = textEditorSetThemeAction.Theme,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetKeymapAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetKeymapAction setKeymapAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    KeymapDefinition = textEditorSetKeymapAction.KeymapDefinition,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetShowWhitespaceAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetShowWhitespaceAction setShowWhitespaceAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    ShowWhitespace = textEditorSetShowWhitespaceAction.ShowWhitespace,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetShowNewlinesAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetShowNewlinesAction setShowNewlinesAction)
        {
            var nextTextEditorOptions = inGlobalOptions
                    .GlobalTextEditorOptions with
                {
                    ShowNewlines = textEditorSetShowNewlinesAction.ShowNewlines,
                };

            return inGlobalOptions with
            {
                GlobalTextEditorOptions = nextTextEditorOptions,
            };
        }
    }
}