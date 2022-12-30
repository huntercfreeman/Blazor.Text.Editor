using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public class VimSentence
{
    private readonly List<VimGrammarToken> _pendingSentence = new();

    public ImmutableArray<VimGrammarToken> PendingSentence => _pendingSentence
        .ToImmutableArray();

    /// <summary>
    /// TODO: Remove the argument "List&gt;(VimSentence, TextEditorCommand)&lt; textEditorCommandHistoryTuples". Am using it while developing to easily see what is going on.
    /// </summary>
    public bool TryLex(
        List<(ImmutableArray<VimGrammarToken>, TextEditorCommand)> textEditorCommandHistoryTuples,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;

        var mostRecentToken = _pendingSentence.LastOrDefault();

        if (mostRecentToken is null)
        {
            sentenceIsSyntacticallyComplete = ContinueSentenceFromStart(
                keyboardEventArgs,
                hasTextSelection);
        }
        else
        {
            switch (mostRecentToken.VimGrammarKind)
            {
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
        }

        if (sentenceIsSyntacticallyComplete)
        {
            var sentenceSnapshot = PendingSentence;
            _pendingSentence.Clear();
            
            return TryParseVimSentence(
                textEditorCommandHistoryTuples,
                sentenceSnapshot,
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
            _pendingSentence.Clear();
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
            _pendingSentence.Clear();
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
                _pendingSentence.Clear();
                
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
            _pendingSentence.Clear();
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
        _pendingSentence.Clear();
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
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            VimRepeatFacts.TryConstructRepeatToken( // Example: "27w" => 27 times do move cursor to the start of next word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            _pendingSentence.Clear();
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
            case VimGrammarKind.Repeat:
            {
                _pendingSentence.Add(vimGrammarToken);
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// It is expected that one will only invoke <see cref="TryParseVimSentence"/> when
    /// the Lexed sentence is syntactically complete. This method will then
    /// semantically interpret the sentence.
    /// <br/><br/>
    /// TODO: Remove the argument "List&gt;(VimSentence, TextEditorCommand)&lt; textEditorCommandHistoryTuples". Am using it while developing to easily see what is going on.
    /// </summary>
    /// <returns>
    /// Returns true if a sentence was successfully parsed into a <see cref="TextEditorCommand"/>
    /// <br/><br/>
    /// Returns false if a sentence not able to be parsed.
    /// </returns>
    public bool TryParseVimSentence(
        List<(ImmutableArray<VimGrammarToken>, TextEditorCommand)> textEditorCommandHistoryTuples,
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        var firstToken = sentenceSnapshot.FirstOrDefault();

        var success = false;

        if (firstToken is null)
        {
            textEditorCommand = TextEditorCommandFacts.DoNothingDiscard;
        }
        else
        {
            success = TryParseMoveNext(
                sentenceSnapshot,
                0,
                keyboardEventArgs,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommandHistoryTuples.Add((sentenceSnapshot, textEditorCommand));
        return success;
    }

    public static bool TryParseMoveNext(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand textEditorCommand)
    {
        var currentToken = sentenceSnapshot[indexInSentence];
        
        switch (currentToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
            {
                return VimVerbFacts.TryParseVimSentence(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            }
            case VimGrammarKind.Modifier:
            {
                return VimModifierFacts.TryParseVimSentence(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            }
            case VimGrammarKind.TextObject:
            {
                return VimTextObjectFacts.TryParseVimSentence(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            }
            case VimGrammarKind.Repeat:
            {
                return VimRepeatFacts.TryParseVimSentence(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            }
            default:
            {
                throw new ApplicationException(
                    $"The {nameof(VimGrammarKind)}:" +
                    $" {sentenceSnapshot.Last().VimGrammarKind} was not recognized.");
            }
        }
    }
}