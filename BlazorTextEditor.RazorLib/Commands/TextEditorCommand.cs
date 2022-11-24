using BlazorTextEditor.RazorLib.Editing;

namespace BlazorTextEditor.RazorLib.Commands;

public class TextEditorCommand
{
    public TextEditorCommand(
        Func<ITextEditorCommandParameter, Task> doAsyncFunc,
        bool shouldScrollCursorIntoView,
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
        ShouldScrollCursorIntoView = shouldScrollCursorIntoView;
        DisplayName = displayName;
        InternalIdentifier = internalIdentifier;
        TextEditKind = textEditKind;
        OtherTextEditKindIdentifier = otherTextEditKindIdentifier;
    }

    public Func<ITextEditorCommandParameter, Task> DoAsyncFunc { get; }
    public bool ShouldScrollCursorIntoView { get; }
    public string DisplayName { get; }
    public string InternalIdentifier { get; }
    public TextEditKind TextEditKind { get; }
    public string? OtherTextEditKindIdentifier { get; }

    public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind)
    {
        throw new ApplicationException(
            $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
            $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
            $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");
    }
}