using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Diff;

public partial class TextEditorDiffsCollection
{
    public record RegisterAction(
        TextEditorDiffKey DiffKey,
        TextEditorViewModelKey BeforeViewModelKey,
        TextEditorViewModelKey AfterViewModelKey);
    
    public record DisposeAction(
        TextEditorDiffKey DiffKey);
}