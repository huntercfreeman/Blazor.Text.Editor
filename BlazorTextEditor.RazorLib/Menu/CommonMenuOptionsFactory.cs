using BlazorTextEditor.RazorLib.Clipboard;

namespace BlazorTextEditor.RazorLib.Menu;

public class CommonMenuOptionsFactory : ICommonMenuOptionsFactory
{
    private readonly IClipboardProvider _clipboardProvider;

    public CommonMenuOptionsFactory(
        IClipboardProvider clipboardProvider)
    {
        _clipboardProvider = clipboardProvider;
    }
    
    public MenuOptionRecord CopySelection()
    {
        throw new NotImplementedException();
    }
    
    public MenuOptionRecord CutSelection()
    {
        throw new NotImplementedException();
    }
    
    public MenuOptionRecord PasteClipboard()
    {
        throw new NotImplementedException();
    }
}