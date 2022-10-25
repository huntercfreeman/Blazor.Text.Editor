namespace BlazorTextEditor.RazorLib.TextEditor;

public class TextEditorKeymap : ITextEditorKeymap
{
    
}

public interface ITextEditorKeymap
{
    
}

public class TextEditorCommand
{
    public TextEditorCommand(
        Func<ITextEditorCommandParameter, Task> doAsyncFunc,
        string displayName,
        string internalIdentifier,
        TextEditKind textEditKind = TextEditKind.None,
        string? otherTextEditKindIdentifier = null)
    {
        DoAsyncFunc = doAsyncFunc;
        DisplayName = displayName;
        InternalIdentifier = internalIdentifier;
        TextEditKind = textEditKind;
        OtherTextEditKindIdentifier = otherTextEditKindIdentifier;

        if (TextEditKind == TextEditKind.Other &&
            OtherTextEditKindIdentifier is null)
        {
            throw new ApplicationException(
                $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
                $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
                $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");
        }
    }

    public Func<ITextEditorCommandParameter, Task> DoAsyncFunc { get; }
    public string DisplayName { get; }
    public string InternalIdentifier { get; }
    public TextEditKind TextEditKind { get; }
    public string? OtherTextEditKindIdentifier { get; }
}

public interface ITextEditorCommandParameter
{
    
}

public class TextEditorCommandParameter : ITextEditorCommandParameter
{
    
}
