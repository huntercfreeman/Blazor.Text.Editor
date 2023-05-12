using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Misc;
using BlazorCommon.RazorLib.Storage;
using BlazorCommon.RazorLib.Store.DialogCase;
using BlazorCommon.RazorLib.Store.StorageCase;
using BlazorCommon.RazorLib.Theme;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Options;
using BlazorTextEditor.RazorLib.Store.Options;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IOptionsApi
    {
        public void SetCursorWidth(double cursorWidthInPixels);
        public void SetFontFamily(string? fontFamily);
        public void SetFontSize(int fontSizeInPixels);
        public Task SetFromLocalStorageAsync();
        public void SetHeight(int? heightInPixels);
        public void SetKeymap(KeymapDefinition foundKeymap);
        public void SetShowNewlines(bool showNewlines);
        public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations);
        public void SetShowWhitespace(bool showWhitespace);
        /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
        public void SetTheme(ThemeRecord theme);
        public void ShowSettingsDialog(bool? isResizableOverride = null, string? cssClassString = null);
        public void ShowFindDialog(bool? isResizableOverride = null, string? cssClassString = null);
        public void WriteToStorage();
        public void SetRenderStateKey(RenderStateKey renderStateKey);
    }

    public class OptionsApi : IOptionsApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly BlazorTextEditorOptions _blazorTextEditorOptions;
        private readonly IStorageService _storageService;
        private readonly ITextEditorService _textEditorService;

        public OptionsApi(
            IDispatcher dispatcher,
            BlazorTextEditorOptions blazorTextEditorOptions,
            IStorageService storageService,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _blazorTextEditorOptions = blazorTextEditorOptions;
            _storageService = storageService;
            _textEditorService = textEditorService;
        }

        public void WriteToStorage()
        {
            _dispatcher.Dispatch(
                new StorageEffects.WriteToStorageAction(
                    _textEditorService.StorageKey,
                    _textEditorService.OptionsWrap.Value.Options));
        }

        public void ShowSettingsDialog(
            bool? isResizableOverride = null,
            string? cssClassString = null)
        {
            var settingsDialog = new DialogRecord(
                DialogKey.NewDialogKey(),
                "Text Editor Settings",
                _blazorTextEditorOptions.SettingsComponentRendererType,
                null,
                cssClassString)
            {
                IsResizable = isResizableOverride ??
                    _blazorTextEditorOptions.SettingsDialogComponentIsResizable
            };

            _dispatcher.Dispatch(
                new DialogRecordsCollection.RegisterAction(
                    settingsDialog));
        }

        public void ShowFindDialog(
            bool? isResizableOverride = null,
            string? cssClassString = null)
        {
            var findDialog = new DialogRecord(
                DialogKey.NewDialogKey(),
                "Text Editor Find",
                _blazorTextEditorOptions.FindComponentRendererType,
                null,
                cssClassString)
            {
                IsResizable = isResizableOverride ??
                    _blazorTextEditorOptions.FindDialogComponentIsResizable
            };

            _dispatcher.Dispatch(
                new DialogRecordsCollection.RegisterAction(
                    findDialog));
        }

        public void SetTheme(
            ThemeRecord theme)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetThemeAction(
                    theme));

            WriteToStorage();
        }

        public void SetShowWhitespace(
            bool showWhitespace)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetShowWhitespaceAction(
                    showWhitespace));

            WriteToStorage();
        }

        public void SetUseMonospaceOptimizations(
            bool useMonospaceOptimizations)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(
                    useMonospaceOptimizations));

            WriteToStorage();
        }

        public void SetShowNewlines(
            bool showNewlines)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetShowNewlinesAction(
                    showNewlines));

            WriteToStorage();
        }

        public void SetKeymap(
            KeymapDefinition foundKeymap)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetKeymapAction(
                    foundKeymap));

            WriteToStorage();
        }

        public void SetHeight(
            int? heightInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetHeightAction(
                    heightInPixels));

            WriteToStorage();
        }

        public async Task SetFromLocalStorageAsync()
        {
            var optionsJsonString = await _storageService
                    .GetValue(_textEditorService.StorageKey)
                    as string;

            if (string.IsNullOrWhiteSpace(optionsJsonString))
                return;

            var options = System.Text.Json.JsonSerializer
                .Deserialize<TextEditorOptions>(optionsJsonString);

            if (options is null)
                return;

            if (options.CommonOptions?.ThemeKey is not null)
            {
                var matchedTheme = _textEditorService
                    .ThemeRecordsCollectionWrap.Value.ThemeRecordsList
                        .FirstOrDefault(x =>
                            x.ThemeKey == options.CommonOptions.ThemeKey);

                SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
            }

            if (options.KeymapDefinition is not null)
            {
                var matchedKeymapDefinition = KeymapFacts.AllKeymapDefinitions
                    .FirstOrDefault(x =>
                        x.KeymapKey == options.KeymapDefinition.KeymapKey);

                SetKeymap(matchedKeymapDefinition ?? KeymapFacts.DefaultKeymapDefinition);
            }

            if (options.CommonOptions?.FontSizeInPixels is not null)
                SetFontSize(options.CommonOptions.FontSizeInPixels.Value);

            if (options.CursorWidthInPixels is not null)
                SetCursorWidth(options.CursorWidthInPixels.Value);

            if (options.TextEditorHeightInPixels is not null)
                SetHeight(options.TextEditorHeightInPixels.Value);

            if (options.ShowNewlines is not null)
                SetShowNewlines(options.ShowNewlines.Value);

            // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
            // for a first time user. This leads to a bad user experience since the proportional
            // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
            // from local storage.
            //
            // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);

            if (options.ShowWhitespace is not null)
                SetShowWhitespace(options.ShowWhitespace.Value);
        }

        public void SetFontSize(
            int fontSizeInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetFontSizeAction(
                    fontSizeInPixels));

            WriteToStorage();
        }

        public void SetFontFamily(
            string? fontFamily)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetFontFamilyAction(
                    fontFamily));

            WriteToStorage();
        }

        public void SetCursorWidth(
            double cursorWidthInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetCursorWidthAction(
                    cursorWidthInPixels));

            WriteToStorage();
        }

        public void SetRenderStateKey(
            RenderStateKey renderStateKey)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetRenderStateKeyAction(
                    renderStateKey));
        }
    }
}
