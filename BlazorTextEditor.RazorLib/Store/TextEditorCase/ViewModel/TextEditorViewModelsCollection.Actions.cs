using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;

public partial class TextEditorViewModelsCollection
{
    public record RegisterTextEditorViewModelAction(
        TextEditorViewModelKey TextEditorViewModelKey, 
        TextEditorModelKey TextEditorModelKey,
        ITextEditorService TextEditorService);

    public record SetViewModelWithAction(
        TextEditorViewModelKey TextEditorViewModelKey, 
        Func<TextEditorViewModel, TextEditorViewModel> WithFunc);
}