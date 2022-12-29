using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public class VimPhrase
{
    private readonly List<VimGrammarToken> _pendingPhrase = new()
    {
        new VimGrammarToken(VimGrammarKind.Start, string.Empty)
    };

    /// <summary>
    /// TODO: Having this method is asking for trouble as one can just circumvent the method by invoking _pendingPhrase.Clear() without adding in an initial VimGrammarKind.Start. This should be changed. The idea for this method is that one must always start the pending phrase with VimGrammarKind.Start yet as of this moment you need special knowledge to know to call this method so it is awkward.
    /// </summary>
    private void ResetPendingPhrase()
    {
        _pendingPhrase.Clear();
        _pendingPhrase.Add(
            new VimGrammarToken(VimGrammarKind.Start, string.Empty));
    }

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
                phraseIsSyntacticallyComplete = ContinuePhraseFromStart(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Command:
            {
                phraseIsSyntacticallyComplete = ContinuePhraseFromCommand(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Expansion:
            {
                phraseIsSyntacticallyComplete = ContinuePhraseFromExpansion(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Motion:
            {
                phraseIsSyntacticallyComplete = ContinuePhraseFromMotion(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Repeat:
            {
                phraseIsSyntacticallyComplete = ContinuePhraseFromRepeat(
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

    private bool ContinuePhraseFromStart(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimCommandFacts.TryConstructCommandToken( // Example: "d..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimMotionFacts.TryConstructMotionToken( // Example: "w" => move cursor forward until reaching the next word.
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimRepeatFacts.TryConstructRepeatToken( // Example: "3..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
            return false;

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Command:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return false;
            }
            case VimGrammarKind.Expansion:
            {
                // VimGrammarKind.Expansion is
                // invalid here so ignore and keep VimGrammarKind.Start
                return false;
            }
            case VimGrammarKind.Motion:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return true;
            }
            case VimGrammarKind.Repeat:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return false;
            }
        }

        return false;
    }
    
    private bool ContinuePhraseFromCommand(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimCommandFacts.TryConstructCommandToken( // Example: "dd" => delete line
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimMotionFacts.TryConstructMotionToken( // Example: "dw" => delete word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimRepeatFacts.TryConstructRepeatToken( // Example: "d3..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
            return false;

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Command:
            {
                if (_pendingPhrase.Last().TextValue == vimGrammarToken.TextValue)
                {
                    _pendingPhrase.Add(vimGrammarToken);
                    return true;
                }

                // The command was overriden so restart phrase
                ResetPendingPhrase();
                
                return ContinuePhraseFromStart(
                    keyboardEventArgs,
                    hasTextSelection);
            }
            case VimGrammarKind.Expansion:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return false;
            }
            case VimGrammarKind.Motion:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return true;
            }
            case VimGrammarKind.Repeat:
            {
                _pendingPhrase.Add(vimGrammarToken);
                return false;
            }
        }

        return false;
    }
    
    private bool ContinuePhraseFromExpansion(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }

    private bool ContinuePhraseFromMotion(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        throw new NotImplementedException();
    }
    
    private bool ContinuePhraseFromRepeat(
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