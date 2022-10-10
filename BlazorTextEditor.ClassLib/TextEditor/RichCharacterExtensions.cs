using BlazorTextEditor.ClassLib.Keyboard;

namespace BlazorTextEditor.ClassLib.TextEditor;

public static class RichCharacterExtensions
{
    public static CharacterKind GetCharacterKind(this RichCharacter richCharacter)
    {
        if (KeyboardKeyFacts.IsWhitespaceCharacter(richCharacter.Value))
            return CharacterKind.Whitespace;
        else if (KeyboardKeyFacts.IsPunctuationCharacter(richCharacter.Value))
            return CharacterKind.Punctuation;
        else
            return CharacterKind.LetterOrDigit;
    }
}