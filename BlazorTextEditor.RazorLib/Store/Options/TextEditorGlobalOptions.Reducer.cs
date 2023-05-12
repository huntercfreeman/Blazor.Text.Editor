using BlazorCommon.RazorLib.Misc;
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
                    },
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    },
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
                },
            };
        }
        
        [ReducerMethod]
        public static TextEditorOptionsState ReduceSetRenderStateKeyAction(
            TextEditorOptionsState inOptionsState,
            SetRenderStateKeyAction setRenderStateKeyAction)
        {
            return new TextEditorOptionsState
            {
                Options = inOptionsState.Options with
                {
                    RenderStateKey = setRenderStateKeyAction.RenderStateKey
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
                    CursorWidthInPixels = setCursorWidthAction.CursorWidthInPixels,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    TextEditorHeightInPixels = setHeightAction.HeightInPixels,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    },
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    KeymapDefinition = setKeymapAction.KeymapDefinition,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    ShowWhitespace = setShowWhitespaceAction.ShowWhitespace,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    ShowNewlines = setShowNewlinesAction.ShowNewlines,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
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
                    UseMonospaceOptimizations = setUseMonospaceOptimizationsAction.UseMonospaceOptimizations,
                    RenderStateKey = RenderStateKey.NewRenderStateKey(),
                },
            };
        }
    }
}