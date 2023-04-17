using BlazorCommon.RazorLib.Keyboard;

namespace BlazorTextEditor.RazorLib.Character;

public static class CharacterKindHelper
{
    public static CharacterKind CharToCharacterKind(char value)
    {
        if (KeyboardKeyFacts.IsWhitespaceCharacter(value))
            return CharacterKind.Whitespace;
        if (KeyboardKeyFacts.IsPunctuationCharacter(value))
            return CharacterKind.Punctuation;
        return CharacterKind.LetterOrDigit;
    }
}