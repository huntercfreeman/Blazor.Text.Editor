namespace BlazorTextEditor.RazorLib.TextEditor;

public class TextEditorCommand
{
    public TextEditorCommand(
        Func<ITextEditorCommandParameter, Task> doAsyncFunc,
        string displayName,
        string internalIdentifier,
        TextEditKind textEditKind = TextEditKind.None,
        string? otherTextEditKindIdentifier = null)
    {
        if (textEditKind == TextEditKind.Other &&
            otherTextEditKindIdentifier is null)
        {
            ThrowOtherTextEditKindIdentifierWasExpectedException(
                textEditKind);
        }
        
        DoAsyncFunc = doAsyncFunc;
        DisplayName = displayName;
        InternalIdentifier = internalIdentifier;
        TextEditKind = textEditKind;
        OtherTextEditKindIdentifier = otherTextEditKindIdentifier;
    }
    
    public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind) => throw new ApplicationException(
        $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
        $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
        $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");

    public Func<ITextEditorCommandParameter, Task> DoAsyncFunc { get; }
    public string DisplayName { get; }
    public string InternalIdentifier { get; }
    public TextEditKind TextEditKind { get; }
    public string? OtherTextEditKindIdentifier { get; }
}