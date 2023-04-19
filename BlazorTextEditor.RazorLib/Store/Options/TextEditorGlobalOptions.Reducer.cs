using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Options;

public partial class TextEditorOptionsState
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetFontFamilyAction(
            TextEditorOptionsState inOptionsState,
            SetFontFamilyAction setFontFamilyAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    CommonOptions = inOptionsState.Options.CommonOptions with
                    {
                        FontFamily = setFontFamilyAction.FontFamily
                    }
                },
            };
        }
        
        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetFontSizeAction(
            TextEditorOptionsState inOptionsState,
            SetFontSizeAction setFontSizeAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    CommonOptions = inOptionsState.Options.CommonOptions with
                    {
                        FontSizeInPixels = setFontSizeAction.FontSizeInPixels
                    }
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetCursorWidthAction(
            TextEditorOptionsState inOptionsState,
            SetCursorWidthAction setCursorWidthAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    CursorWidthInPixels = setCursorWidthAction.CursorWidthInPixels
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetHeightAction(
            TextEditorOptionsState inOptionsState,
            SetHeightAction setHeightAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    TextEditorHeightInPixels = setHeightAction.HeightInPixels
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetThemeAction(
            TextEditorOptionsState inOptionsState,
            SetThemeAction setThemeAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    CommonOptions = inOptionsState.Options.CommonOptions with
                    {
                        ThemeKey = setThemeAction.Theme.ThemeKey
                    }
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetKeymapAction(
            TextEditorOptionsState inOptionsState,
            SetKeymapAction setKeymapAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    KeymapDefinition = setKeymapAction.KeymapDefinition
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetShowWhitespaceAction(
            TextEditorOptionsState inOptionsState,
            SetShowWhitespaceAction setShowWhitespaceAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    ShowWhitespace = setShowWhitespaceAction.ShowWhitespace
                },
            };
        }

        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetShowNewlinesAction(
            TextEditorOptionsState inOptionsState,
            SetShowNewlinesAction setShowNewlinesAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    ShowNewlines = setShowNewlinesAction.ShowNewlines
                },
            };
        }
        
        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetUseMonospaceOptimizationsAction(
            TextEditorOptionsState inOptionsState,
            SetUseMonospaceOptimizationsAction setUseMonospaceOptimizationsAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    UseMonospaceOptimizations = 
                        setUseMonospaceOptimizationsAction.UseMonospaceOptimizations
                },
            };
        }
    }
}