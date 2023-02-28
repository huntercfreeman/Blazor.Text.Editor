using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Store.ViewModel;

public partial class TextEditorViewModelsCollection
{
    public record RegisterAction(
        TextEditorViewModelKey TextEditorViewModelKey, 
        TextEditorModelKey TextEditorModelKey,
        ITextEditorService TextEditorService);

    public record DisposeAction(
        TextEditorViewModelKey TextEditorViewModelKey);
    
    public record SetViewModelWithAction(
        TextEditorViewModelKey TextEditorViewModelKey, 
        Func<TextEditorViewModel, TextEditorViewModel> WithFunc);
}