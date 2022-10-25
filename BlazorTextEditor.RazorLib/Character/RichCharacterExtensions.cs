using BlazorTextEditor.RazorLib.Keyboard;

namespace BlazorTextEditor.RazorLib.MoveThese;

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