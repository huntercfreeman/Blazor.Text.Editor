using System.Collections.Immutable;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public class TextEditorViewModelsCollectionReducer
{
    [ReducerMethod]
    public static TextEditorViewModelsCollection ReduceRegisterTextEditorViewModelAction(
        TextEditorViewModelsCollection previousTextEditorViewModelsCollection,
        RegisterTextEditorViewModelAction registerTextEditorViewModelAction)
    {
        var textEditorViewModel = previousTextEditorViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == registerTextEditorViewModelAction.TextEditorViewModelKey);

        if (textEditorViewModel is not null)
            return previousTextEditorViewModelsCollection;

        var viewModel = new TextEditorViewModel(
            registerTextEditorViewModelAction.TextEditorViewModelKey,
            registerTextEditorViewModelAction.TextEditorKey,
            registerTextEditorViewModelAction.TextEditorService,
            new VirtualizationResult<List<RichCharacter>>(
                ImmutableArray<VirtualizationEntry<List<RichCharacter>>>.Empty,
                new VirtualizationBoundary(0, 0, 0, 0),
                new VirtualizationBoundary(0, 0, 0, 0),
                new VirtualizationBoundary(0, 0, 0, 0),
                new VirtualizationBoundary(0, 0, 0, 0),
                new ElementMeasurementsInPixels(0, 0, 0, 0, 0, 0, CancellationToken.None),
                new CharacterWidthAndRowHeight(0, 0)));

        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Add(viewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }

    [ReducerMethod]
    public static TextEditorViewModelsCollection ReduceSetViewVirtualizationResultAction(
        TextEditorViewModelsCollection previousTextEditorViewModelsCollection,
        SetViewVirtualizationResultAction setViewVirtualizationResultAction)
    {
        var textEditorViewModel = previousTextEditorViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == setViewVirtualizationResultAction.TextEditorViewModelKey);

        if (textEditorViewModel is null)
            return previousTextEditorViewModelsCollection;

        var nextViewModel = textEditorViewModel with
        {
            VirtualizationResult = setViewVirtualizationResultAction.VirtualizationResult,
            TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
        };

        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Replace(textEditorViewModel, nextViewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }
    
    [ReducerMethod]
    public static TextEditorViewModelsCollection ReduceSetViewModelShouldMeasureDimensionsAction(
        TextEditorViewModelsCollection previousTextEditorViewModelsCollection,
        SetViewModelShouldMeasureDimensionsAction setViewModelShouldMeasureDimensionsAction)
    {
        var textEditorViewModel = previousTextEditorViewModelsCollection.ViewModelsList.FirstOrDefault(x =>
            x.TextEditorViewModelKey == setViewModelShouldMeasureDimensionsAction.TextEditorViewModelKey);

        if (textEditorViewModel is null)
            return previousTextEditorViewModelsCollection;

        var nextViewModel = textEditorViewModel with
        {
            ShouldMeasureDimensions = setViewModelShouldMeasureDimensionsAction.ShouldMeasureDimensions,
            TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
        };

        var nextViewModelsList = previousTextEditorViewModelsCollection.ViewModelsList
            .Replace(textEditorViewModel, nextViewModel);

        return new TextEditorViewModelsCollection
        {
            ViewModelsList = nextViewModelsList
        };
    }
}