using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.Vim;

public class VimSentence
{
    private readonly List<VimGrammarToken> _pendingSentence = new();

    public ImmutableArray<VimGrammarToken> PendingSentence => _pendingSentence
        .ToImmutableArray();

    public bool TryLex(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;

        var mostRecentToken = _pendingSentence.LastOrDefault();

        if (mostRecentToken is null)
        {
            sentenceIsSyntacticallyComplete = ContinueFromStart(
                keyboardEventArgs,
                hasTextSelection);
        }
        else
        {
            switch (mostRecentToken.VimGrammarKind)
            {
                case VimGrammarKind.Verb:
                {
                    sentenceIsSyntacticallyComplete = ContinueFromVerb(
                        keyboardEventArgs,
                        hasTextSelection);
                
                    break;
                }
                case VimGrammarKind.Modifier:
                {
                    sentenceIsSyntacticallyComplete = ContinueFromModifier(
                        keyboardEventArgs,
                        hasTextSelection);
                
                    break;
                }
                case VimGrammarKind.TextObject:
                {
                    sentenceIsSyntacticallyComplete = ContinueFromTextObject(
                        keyboardEventArgs,
                        hasTextSelection);
                
                    break;
                }
                case VimGrammarKind.Repeat:
                {
                    sentenceIsSyntacticallyComplete = ContinueFromRepeat(
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
            
            return TryParseSentence(
                sentenceSnapshot,
                keyboardEventArgs,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }

    private bool ContinueFromStart(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex(
                // Example: "d..." is valid albeit incomplete
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex(
                // Example: "w" => move cursor forward until reaching the next word.
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex(
                // Example: "3..." is valid albeit incomplete
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
                if (keyboardEventArgs.CtrlKey)
                {
                    // This if case relates to 'Ctrl + e' which does not get
                    // double tapped instead it only takes one press of the keymap
                    
                    _pendingSentence.Clear();
                    _pendingSentence.Add(vimGrammarToken);
                    
                    return true;
                }
                
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
    
    private bool ContinueFromVerb(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex( // Example: "dd" => delete line
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex( // Example: "dw" => delete word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex( // Example: "d3..." is valid albeit incomplete
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
                if (_pendingSentence.Last().KeyboardEventArgs.Key == vimGrammarToken.KeyboardEventArgs.Key)
                {
                    _pendingSentence.Add(vimGrammarToken);
                    return true;
                }

                if (keyboardEventArgs.CtrlKey)
                {
                    // This if case relates to 'Ctrl + e' which does not get
                    // double tapped instead it only takes one press of the keymap
                    
                    _pendingSentence.Clear();
                    _pendingSentence.Add(vimGrammarToken);
                    
                    return true;
                }

                // The verb was overriden so restart sentence
                _pendingSentence.Clear();
                
                return ContinueFromStart(
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
    
    private bool ContinueFromModifier(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxTextObjectVim.TryLex( // Example: "diw" => delete inner word
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

    private bool ContinueFromTextObject(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        // This state should not occur as a TextObject always ends a sentence if it is there.
        _pendingSentence.Clear();
        return false;
    }
    
    private bool ContinueFromRepeat(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex( // Example: "3dd" => 3 times do delete line
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex( // Example: "3w" => 3 times do move cursor to the start of next word
                keyboardEventArgs, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex( // Example: "27w" => 27 times do move cursor to the start of next word
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
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        var firstToken = sentenceSnapshot.FirstOrDefault();

        if (firstToken is null)
        {
            textEditorCommand = null;
            return false;
        }
        
        return TryParseNextToken(
            sentenceSnapshot,
            0,
            keyboardEventArgs,
            hasTextSelection,
            out textEditorCommand);
    }

    public static bool TryParseNextToken(
        ImmutableArray<VimGrammarToken> sentenceSnapshot,
        int indexInSentence,
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        if (indexInSentence >= sentenceSnapshot.Length)
        {
            textEditorCommand = null;
            return false;
        }

        var currentToken = sentenceSnapshot[indexInSentence];
        
        switch (currentToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
                return SyntaxVerbVim.TryParse(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            case VimGrammarKind.Modifier:
                return SyntaxModifierVim.TryParse(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            case VimGrammarKind.TextObject:
                return SyntaxTextObjectVim.TryParse(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            case VimGrammarKind.Repeat:
                return SyntaxRepeatVim.TryParse(
                    sentenceSnapshot,
                    indexInSentence,
                    keyboardEventArgs,
                    hasTextSelection,
                    out textEditorCommand);
            default:
                throw new ApplicationException(
                    $"The {nameof(VimGrammarKind)}:" +
                    $" {sentenceSnapshot.Last().VimGrammarKind} was not recognized.");
        }
    }
}