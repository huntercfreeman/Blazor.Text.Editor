using BlazorCommon.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IViewModelApi
    {
        public void Dispose(TextEditorViewModelKey textEditorViewModelKey);
        public Task<ElementMeasurementsInPixels> MeasureElementInPixelsAsync(string elementId);
        public Task<CharacterWidthAndRowHeight> MeasureCharacterWidthAndRowHeightAsync(string measureCharacterWidthAndRowHeightElementId, int countOfTestCharacters);
        public TextEditorViewModel? FindOrDefault(TextEditorViewModelKey textEditorViewModelKey);
        public Task FocusPrimaryCursorAsync(string primaryCursorContentId);
        public string? GetAllText(TextEditorViewModelKey textEditorViewModelKey);
        public TextEditorModel? FindBackingModelOrDefault(TextEditorViewModelKey textEditorViewModelKey);
        public Task MutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public Task MutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public void Register(TextEditorViewModelKey textEditorViewModelKey, TextEditorModelKey textEditorModelKey);
        public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels);
        public Task SetScrollPositionAsync(string bodyElementId, string gutterElementId, double? scrollLeftInPixels, double? scrollTopInPixels);
        public void With(TextEditorViewModelKey textEditorViewModelKey, Func<TextEditorViewModel, TextEditorViewModel> withFunc);
    }

    public class ViewModelApi : IViewModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDispatcher _dispatcher;
        private readonly BlazorTextEditorOptions _blazorTextEditorOptions;

        // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
        private readonly IJSRuntime _jsRuntime;

        public ViewModelApi(
            IDispatcher dispatcher,
            BlazorTextEditorOptions blazorTextEditorOptions,
            IJSRuntime jsRuntime,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _blazorTextEditorOptions = blazorTextEditorOptions;
            _jsRuntime = jsRuntime;
            _textEditorService = textEditorService;
        }

        public void With(
            TextEditorViewModelKey textEditorViewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc)
        {
            _dispatcher.Dispatch(
                new TextEditorViewModelsCollection.SetViewModelWithAction(
                    textEditorViewModelKey,
                    withFunc));
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public async Task SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels)
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.setScrollPosition",
                bodyElementId,
                gutterElementId,
                scrollLeftInPixels,
                scrollTopInPixels);
        }

        public async Task SetGutterScrollTopAsync(
            string gutterElementId,
            double scrollTopInPixels)
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.setGutterScrollTop",
                gutterElementId,
                scrollTopInPixels);
        }

        public void Register(
            TextEditorViewModelKey textEditorViewModelKey,
            TextEditorModelKey textEditorModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorViewModelsCollection.RegisterAction(
                    textEditorViewModelKey,
                    textEditorModelKey,
                    _textEditorService));
        }

        public async Task MutateScrollVerticalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.mutateScrollVerticalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
        }

        public async Task MutateScrollHorizontalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.mutateScrollHorizontalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
        }

        public TextEditorModel? FindBackingModelOrDefault(
            TextEditorViewModelKey textEditorViewModelKey)
        {
            var textEditorViewModelsCollection = _textEditorService.ViewModelsCollectionWrap
                .Value;

            var viewModel = textEditorViewModelsCollection.ViewModelsList
                .FirstOrDefault(x =>
                    x.ViewModelKey == textEditorViewModelKey);

            if (viewModel is null)
                return null;

            return _textEditorService.Model.FindOrDefault(viewModel.ModelKey);
        }

        public string? GetAllText(
            TextEditorViewModelKey textEditorViewModelKey)
        {
            var textEditorModel = FindBackingModelOrDefault(textEditorViewModelKey);

            return textEditorModel is null
                ? null
                : _textEditorService.Model.GetAllText(textEditorModel.ModelKey);
        }

        public async Task FocusPrimaryCursorAsync(string primaryCursorContentId)
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.focusHtmlElementById",
                primaryCursorContentId);
        }

        public TextEditorViewModel? FindOrDefault(
            TextEditorViewModelKey textEditorViewModelKey)
        {
            return _textEditorService.ViewModelsCollectionWrap.Value.ViewModelsList
                .FirstOrDefault(x =>
                    x.ViewModelKey == textEditorViewModelKey);
        }

        public async Task<ElementMeasurementsInPixels> MeasureElementInPixelsAsync(
            string elementId)
        {
            return await _jsRuntime.InvokeAsync<ElementMeasurementsInPixels>(
                "blazorTextEditor.getElementMeasurementsInPixelsById",
                elementId);
        }

        public async Task<CharacterWidthAndRowHeight> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters)
        {
            return await _jsRuntime.InvokeAsync<CharacterWidthAndRowHeight>(
                    "blazorTextEditor.measureCharacterWidthAndRowHeight",
                    measureCharacterWidthAndRowHeightElementId,
                    countOfTestCharacters);
        }

        public void Dispose(TextEditorViewModelKey textEditorViewModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorViewModelsCollection.DisposeAction(
                    textEditorViewModelKey));
        }
    }
}
