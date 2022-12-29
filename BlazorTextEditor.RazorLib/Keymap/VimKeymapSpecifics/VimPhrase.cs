using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public class VimPhrase
{
    private readonly List<VimGrammarToken> _pendingPhrase = new()
    {
        new VimGrammarToken(VimGrammarKind.Start, string.Empty)
    };

    public bool TryLexPhrase(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        bool phraseIsSyntacticallyComplete;
        
        switch (_pendingPhrase.Last().VimGrammarKind)
        {
            case VimGrammarKind.Start:
            {
                phraseIsSyntacticallyComplete = HandleLexStart(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Command:
            {
                phraseIsSyntacticallyComplete = HandleLexCommand(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Expansion:
            {
                phraseIsSyntacticallyComplete = HandleLexExpansion(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Motion:
            {
                phraseIsSyntacticallyComplete = HandleLexMotion(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Repeat:
            {
                phraseIsSyntacticallyComplete = HandleLexRepeat(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            default:
            {
                throw new ApplicationException(
                    $"The {nameof(VimGrammarKind)}:" +
                    $" {_pendingPhrase.Last().VimGrammarKind} was not recognized.");
            }
        }

        if (phraseIsSyntacticallyComplete)
        {
            return TryParsePhrase(
                _pendingPhrase,
                keyboardEventArgs,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }

    private bool HandleLexRepeat(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    private bool HandleLexMotion(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    private bool HandleLexExpansion(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    private bool HandleLexCommand(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    private bool HandleLexStart(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// It is expected that one will only invoke <see cref="TryParsePhrase"/> when
    /// the Lexed phrase is syntactically complete. This method will then
    /// semantically interpret the phrase.
    /// </summary>
    /// <returns>
    /// Returns true if a phrase was successfully parsed into a <see cref="TextEditorCommand"/>
    /// <br/><br/>
    /// Returns false if a phrase not able to be parsed.
    /// </returns>
    public bool TryParsePhrase(
        List<VimGrammarToken> phrase,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        
    }
}