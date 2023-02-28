using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.GlobalOptions;

public partial class TextEditorGlobalOptions
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetFontSizeAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetFontSizeAction setFontSizeAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    CommonOptions = inGlobalOptions.Options.CommonOptions with
                    {
                        FontSizeInPixels = setFontSizeAction.FontSizeInPixels
                    }
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetCursorWidthAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetCursorWidthAction setCursorWidthAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    CursorWidthInPixels = setCursorWidthAction.CursorWidthInPixels
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetHeightAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetHeightAction setHeightAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    TextEditorHeightInPixels = setHeightAction.HeightInPixels
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetThemeAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetThemeAction setThemeAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    CommonOptions = inGlobalOptions.Options.CommonOptions with
                    {
                        ThemeKey = setThemeAction.Theme.ThemeKey
                    }
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetKeymapAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetKeymapAction setKeymapAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    KeymapDefinition = setKeymapAction.KeymapDefinition
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetShowWhitespaceAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetShowWhitespaceAction setShowWhitespaceAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    ShowWhitespace = setShowWhitespaceAction.ShowWhitespace
                },
            };
        }

        [ReducerMethod]
        public static TextEditorGlobalOptions ReduceSetShowNewlinesAction(
            TextEditorGlobalOptions inGlobalOptions,
            SetShowNewlinesAction setShowNewlinesAction)
        {
            return new TextEditorGlobalOptions
            {
                Options = inGlobalOptions.Options with
                {
                    ShowWhitespace = setShowNewlinesAction.ShowNewlines
                },
            };
        }
    }
}