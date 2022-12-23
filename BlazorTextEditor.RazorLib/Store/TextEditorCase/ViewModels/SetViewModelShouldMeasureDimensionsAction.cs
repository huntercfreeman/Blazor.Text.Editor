namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public record SetViewModelShouldMeasureDimensionsAction(
    TextEditorViewModelKey TextEditorViewModelKey, 
    bool ShouldMeasureDimensions);