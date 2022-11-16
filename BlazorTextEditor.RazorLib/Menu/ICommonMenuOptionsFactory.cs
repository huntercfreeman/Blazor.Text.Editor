namespace BlazorTextEditor.RazorLib.Menu;

public interface ICommonMenuOptionsFactory
{
    public MenuOptionRecord CopySelection();
    public MenuOptionRecord CutSelection();
    public MenuOptionRecord PasteClipboard();
}