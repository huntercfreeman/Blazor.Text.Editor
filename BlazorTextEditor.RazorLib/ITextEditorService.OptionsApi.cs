using BlazorCommon.RazorLib.Dialog;
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
        public void OptionsSetCursorWidth(double cursorWidthInPixels);
        public void OptionsSetFontFamily(string? fontFamily);
        public void OptionsSetFontSize(int fontSizeInPixels);
        public Task OptionsSetFromLocalStorageAsync();
        public void OptionsSetHeight(int? heightInPixels);
        public void OptionsSetKeymap(KeymapDefinition foundKeymap);
        public void OptionsSetShowNewlines(bool showNewlines);
        public void OptionsSetUseMonospaceOptimizations(bool useMonospaceOptimizations);
        public void OptionsSetShowWhitespace(bool showWhitespace);
        /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
        public void OptionsSetTheme(ThemeRecord theme);
        public void OptionsShowSettingsDialog(bool isResizable = false, string? cssClassString = null);
        public void OptionsWriteToStorage();
    }

    public class OptionsApi : IOptionsApi
    {
        private readonly IDispatcher _dispatcher;
        private readonly IStorageService _storageService;
        private readonly ITextEditorService _textEditorService;

        public OptionsApi(
            IDispatcher dispatcher,
            IStorageService storageService,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _storageService = storageService;
            _textEditorService = textEditorService;
        }

        public void OptionsWriteToStorage()
        {
            _dispatcher.Dispatch(
                new StorageEffects.WriteToStorageAction(
                    _textEditorService.StorageKey,
                    _textEditorService.OptionsWrap.Value.Options));
        }

        public void OptionsShowSettingsDialog(
            bool isResizable = false,
            string? cssClassString = null)
        {
            var settingsDialog = new DialogRecord(
                DialogKey.NewDialogKey(),
                "Text Editor Settings",
                typeof(TextEditorSettings),
                null,
                cssClassString)
            {
                IsResizable = isResizable
            };

            _dispatcher.Dispatch(
                new DialogRecordsCollection.RegisterAction(
                    settingsDialog));
        }

        public void OptionsSetTheme(
            ThemeRecord theme)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetThemeAction(
                    theme));

            OptionsWriteToStorage();
        }

        public void OptionsSetShowWhitespace(
            bool showWhitespace)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetShowWhitespaceAction(
                    showWhitespace));

            OptionsWriteToStorage();
        }

        public void OptionsSetUseMonospaceOptimizations(
            bool useMonospaceOptimizations)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(
                    useMonospaceOptimizations));

            OptionsWriteToStorage();
        }

        public void OptionsSetShowNewlines(
            bool showNewlines)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetShowNewlinesAction(
                    showNewlines));

            OptionsWriteToStorage();
        }

        public void OptionsSetKeymap(
            KeymapDefinition foundKeymap)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetKeymapAction(
                    foundKeymap));

            OptionsWriteToStorage();
        }

        public void OptionsSetHeight(
            int? heightInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetHeightAction(
                    heightInPixels));

            OptionsWriteToStorage();
        }

        public async Task OptionsSetFromLocalStorageAsync()
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

                OptionsSetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
            }

            if (options.KeymapDefinition is not null)
            {
                var matchedKeymapDefinition = KeymapFacts.AllKeymapDefinitions
                    .FirstOrDefault(x =>
                        x.KeymapKey == options.KeymapDefinition.KeymapKey);

                OptionsSetKeymap(matchedKeymapDefinition ?? KeymapFacts.DefaultKeymapDefinition);
            }

            if (options.CommonOptions?.FontSizeInPixels is not null)
                OptionsSetFontSize(options.CommonOptions.FontSizeInPixels.Value);

            if (options.CursorWidthInPixels is not null)
                OptionsSetCursorWidth(options.CursorWidthInPixels.Value);

            if (options.TextEditorHeightInPixels is not null)
                OptionsSetHeight(options.TextEditorHeightInPixels.Value);

            if (options.ShowNewlines is not null)
                OptionsSetShowNewlines(options.ShowNewlines.Value);

            // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
            // for a first time user. This leads to a bad user experience since the proportional
            // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
            // from local storage.
            //
            // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);

            if (options.ShowWhitespace is not null)
                OptionsSetShowWhitespace(options.ShowWhitespace.Value);
        }

        public void OptionsSetFontSize(
            int fontSizeInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetFontSizeAction(
                    fontSizeInPixels));

            OptionsWriteToStorage();
        }

        public void OptionsSetFontFamily(
            string? fontFamily)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetFontFamilyAction(
                    fontFamily));

            OptionsWriteToStorage();
        }

        public void OptionsSetCursorWidth(
            double cursorWidthInPixels)
        {
            _dispatcher.Dispatch(
                new TextEditorOptionsState.SetCursorWidthAction(
                    cursorWidthInPixels));

            OptionsWriteToStorage();
        }
    }
}
