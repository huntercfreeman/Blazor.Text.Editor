using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public class VimSentence
{
    private readonly List<VimGrammarToken> _pendingSentence = new()
    {
        new VimGrammarToken(VimGrammarKind.Start, string.Empty)
    };

    public ImmutableArray<VimGrammarToken> PendingSentence => _pendingSentence
        .ToImmutableArray();

    /// <summary>
    /// TODO: Having this method is asking for trouble as one can just circumvent the method by invoking _pendingSentence.Clear() without adding in an initial VimGrammarKind.Start. This should be changed. The idea for this method is that one must always start the pending phrase with VimGrammarKind.Start yet as of this moment you need special knowledge to know to call this method so it is awkward.
    /// </summary>
    private void ResetPendingSentence()
    {
        _pendingSentence.Clear();
        _pendingSentence.Add(
            new VimGrammarToken(VimGrammarKind.Start, string.Empty));
    }

    public bool TryLex(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;
        
        switch (_pendingSentence.Last().VimGrammarKind)
        {
            case VimGrammarKind.Start:
            {
                sentenceIsSyntacticallyComplete = ContinueSentenceFromStart(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Verb:
            {
                sentenceIsSyntacticallyComplete = ContinueSentenceFromVerb(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Modifier:
            {
                sentenceIsSyntacticallyComplete = ContinueSentenceFromModifier(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.TextObject:
            {
                sentenceIsSyntacticallyComplete = ContinueSentenceFromTextObject(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            case VimGrammarKind.Repeat:
            {
                sentenceIsSyntacticallyComplete = ContinueSentenceFromRepeat(
                    keyboardEventArgs,
                    hasTextSelection);
                
                break;
            }
            default:
            {
                throw new ApplicationException(
                    $"The {nameof(VimGrammarKind)}:" +
                    $" {_pendingSentence.Last().VimGrammarKind} was not recognized.");
            }
        }

        if (sentenceIsSyntacticallyComplete)
        {
            return TryParseSentence(
                _pendingSentence,
                keyboardEventArgs,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        return true;
    }

    private bool ContinueSentenceFromStart(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimVerbFacts.TryConstructVerbToken( // Example: "d..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimTextObjectFacts.TryConstructTextObjectToken( // Example: "w" => move cursor forward until reaching the next word.
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimRepeatFacts.TryConstructRepeatToken( // Example: "3..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            ResetPendingSentence();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
            case VimGrammarKind.Modifier:
            {
                // VimGrammarKind.Modifier is
                // invalid here so ignore and keep VimGrammarKind.Start
                return false;
            }
            case VimGrammarKind.TextObject:
            {
                _pendingSentence.Add(vimGrammarToken);
                return true;
            }
            case VimGrammarKind.Repeat:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
        }

        return false;
    }
    
    private bool ContinueSentenceFromVerb(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimVerbFacts.TryConstructVerbToken( // Example: "dd" => delete line
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimTextObjectFacts.TryConstructTextObjectToken( // Example: "dw" => delete word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimRepeatFacts.TryConstructRepeatToken( // Example: "d3..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            ResetPendingSentence();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
            {
                if (_pendingSentence.Last().TextValue == vimGrammarToken.TextValue)
                {
                    _pendingSentence.Add(vimGrammarToken);
                    return true;
                }

                // The verb was overriden so restart sentence
                ResetPendingSentence();
                
                return ContinueSentenceFromStart(
                    keyboardEventArgs,
                    hasTextSelection);
            }
            case VimGrammarKind.Modifier:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
            case VimGrammarKind.TextObject:
            {
                _pendingSentence.Add(vimGrammarToken);
                return true;
            }
            case VimGrammarKind.Repeat:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
        }

        return false;
    }
    
    private bool ContinueSentenceFromModifier(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimTextObjectFacts.TryConstructTextObjectToken( // Example: "diw" => delete inner word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            ResetPendingSentence();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.TextObject:
            {
                _pendingSentence.Add(vimGrammarToken);
                return true;
            }
        }

        return false;
    }

    private bool ContinueSentenceFromTextObject(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        // This state should not occur as a TextObject always ends a sentence if it is there.
        ResetPendingSentence();
        return false;
    }
    
    private bool ContinueSentenceFromRepeat(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = VimVerbFacts.TryConstructVerbToken( // Example: "3dd" => 3 times do delete line
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimTextObjectFacts.TryConstructTextObjectToken( // Example: "3w" => 3 times do move cursor to the start of next word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            ResetPendingSentence();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
            case VimGrammarKind.TextObject:
            {
                _pendingSentence.Add(vimGrammarToken);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// It is expected that one will only invoke <see cref="TryParseSentence"/> when
    /// the Lexed sentence is syntactically complete. This method will then
    /// semantically interpret the sentence.
    /// </summary>
    /// <returns>
    /// Returns true if a sentence was successfully parsed into a <see cref="TextEditorCommand"/>
    /// <br/><br/>
    /// Returns false if a sentence not able to be parsed.
    /// </returns>
    public bool TryParseSentence(
        List<VimGrammarToken> sentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        
        ResetPendingSentence();
        return true;
    }
}