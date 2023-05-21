using System.Collections.Immutable;
using System.Text;
using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.RazorLib.Analysis;

/// <summary>
///     The marker for an out of bounds read is
///     <see cref="ParserFacts.END_OF_FILE" />.
///     <br /><br />
///     Provides common API that can be used when implementing an <see cref="ITextEditorLexer" />
///     for the <see cref="TextEditorModel" />.
///     <br /><br />
///     Additionally one can write a parser that takes in a string in order to handle
///     contextual lexing. The <see cref="ITextEditorLexer" /> can then traverse the parsed result
///     which might be that of a tree data structure.
///     <br /><br />
///     I am making up the word "contextual lexing" as I am not sure the actual terminology used.
///     I am still trying to learn all the details but I mean to say, in C# var is a
///     contextual keyword. You cannot go word by word using a Lexer and determine what
///     the word 'var' entails. You instead must have a 'sentence level' understanding
///     to determine under that context whether 'var' is a keyword or if it is being used
///     as something else (perhaps a variable name?).
/// </summary>
public class StringWalker
{
    /// <summary>
    ///     A private reference to the <see cref="string" /> that was provided
    ///     to the <see cref="StringWalker" />'s constructor.
    /// </summary>
    private readonly string _content;

    /// <param name="content">
    ///     The string that one, in a sense, wishes to step character by character through.
    /// </param>
    public StringWalker(string content)
    {
        _content = content;
    }

    /// <summary>
    ///     The character index within the <see cref="_content" /> provided
    ///     to the <see cref="StringWalker" />'s constructor.
    /// </summary>
    public int PositionIndex { get; private set; }

    /// <summary>
    ///     Returns <see cref="PeekCharacter" /> invoked with the value of zero
    /// </summary>
    public char CurrentCharacter => PeekCharacter(0);
    /// <summary>
    ///     Returns <see cref="PeekCharacter" /> invoked with the value of one
    /// </summary>
    public char NextCharacter => PeekCharacter(1);

    public string Content => _content;
    
    /// <summary>
    ///     Starting with <see cref="PeekCharacter" /> evaluated at 0
    ///     return that and the rest of the <see cref="_content" />
    ///     <br /><br />
    ///     <see cref="RemainingText" /> => _content.Substring(PositionIndex);
    /// </summary>
    public string RemainingText => _content.Substring(PositionIndex);
    
    /// <summary>
    /// Returns if the current character is the end of file character
    /// </summary>
    public bool IsEof => CurrentCharacter == ParserFacts.END_OF_FILE;

    /// <summary>
    ///     If <see cref="PositionIndex" /> is within bounds of the <see cref="_content" />.
    ///     <br /><br />
    ///     Then the character within the string <see cref="_content" /> at index
    ///     of <see cref="PositionIndex" /> is returned and <see cref="PositionIndex" /> is incremented
    ///     by one.
    ///     <br /><br />
    ///     Otherwise, <see cref="ParserFacts.END_OF_FILE" /> is returned and
    ///     the value of <see cref="PositionIndex" /> is unchanged.
    /// </summary>
    public char ReadCharacter()
    {
        if (PositionIndex >= _content.Length)
            return ParserFacts.END_OF_FILE;

        return _content[PositionIndex++];
    }

    /// <summary>
    ///     If (<see cref="PositionIndex" /> + <see cref="offset" />)
    ///     is within bounds of the <see cref="_content" />.
    ///     <br /><br />
    ///     Then the character within the string <see cref="_content" /> at index
    ///     of (<see cref="PositionIndex" /> + <see cref="offset" />) is returned and
    ///     <see cref="PositionIndex" /> is unchanged.
    ///     <br /><br />
    ///     Otherwise, <see cref="ParserFacts.END_OF_FILE" /> is returned and
    ///     the value of <see cref="PositionIndex" /> is unchanged.
    /// </summary>
    /// <param name="offset">Must be > -1</param>
    public char PeekCharacter(int offset)
    {
        if (offset <= -1)
            throw new ApplicationException($"{nameof(offset)} must be > -1");

        if (PositionIndex + offset >= _content.Length)
            return ParserFacts.END_OF_FILE;

        return _content[PositionIndex + offset];
    }

    /// <summary>
    ///     If <see cref="PositionIndex" /> being decremented by 1 would result
    ///     in <see cref="PositionIndex" /> being less than 0.
    ///     <br /><br />
    ///     Then <see cref="ParserFacts.END_OF_FILE" /> will be returned
    ///     and <see cref="PositionIndex" /> will be left unchanged.
    ///     <br /><br />
    ///     Otherwise, <see cref="PositionIndex" /> will be decremented by one
    ///     and the character within the string <see cref="_content" /> at index
    ///     of <see cref="PositionIndex" /> is returned.
    /// </summary>
    /// <returns>
    ///     The character one would get
    ///     if one invoked <see cref="BacktrackCharacter" /> and then immediately
    ///     afterwards invoked <see cref="PeekCharacter" /> with a value of 0 passed in.
    /// </returns>
    public char BacktrackCharacter()
    {
        if (PositionIndex == 0)
            return ParserFacts.END_OF_FILE;

        PositionIndex--;

        return PeekCharacter(0);
    }

    /// <summary>
    ///     Iterates a counter from 0 until the counter is equal to <see cref="length" />.
    ///     <br /><br />
    ///     Each iteration <see cref="ReadCharacter" /> will be invoked.
    ///     <br /><br />
    ///     If an iteration's invocation of <see cref="ReadCharacter" /> returned
    ///     <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit
    ///     and return regardless of whether it finished iterating to <see cref="length" />
    ///     or not.
    /// </summary>
    /// <returns>
    ///     The cumulative string that was built from invoking <see cref="ReadCharacter" />
    ///     <see cref="length" /> times.
    /// </returns>
    public string ReadRange(int length)
    {
        var consumeBuilder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var currentCharacter = ReadCharacter();

            consumeBuilder.Append(currentCharacter);

            if (currentCharacter == ParserFacts.END_OF_FILE)
                break;
        }

        return consumeBuilder.ToString();
    }

    /// <summary>
    ///     Iterates a counter from 0 until the counter is equal to <see cref="length" />.
    ///     <br /><br />
    ///     Each iteration <see cref="PeekCharacter" /> will be invoked using the
    ///     (<see cref="offset" /> + counter).
    ///     <br /><br />
    ///     If an iteration's invocation of <see cref="PeekCharacter" /> returned
    ///     <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit
    ///     and return regardless of whether it finished iterating to <see cref="length" />
    ///     or not.
    /// </summary>
    /// <returns>
    ///     The cumulative string that was built from invoking <see cref="PeekCharacter" />
    ///     <see cref="length" /> times.
    /// </returns>
    public string PeekRange(int offset, int length)
    {
        var peekBuilder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var currentCharacter = PeekCharacter(offset + i);

            peekBuilder.Append(currentCharacter);

            if (currentCharacter == ParserFacts.END_OF_FILE)
                break;
        }

        return peekBuilder.ToString();
    }

    /// <summary>
    ///     Iterates a counter from 0 until the counter is equal to <see cref="length" />.
    ///     <br /><br />
    ///     Each iteration <see cref="BacktrackCharacter" /> will be invoked using the.
    ///     <br /><br />
    ///     If an iteration's invocation of <see cref="BacktrackCharacter" /> returned
    ///     <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit
    ///     and return regardless of whether it finished iterating to <see cref="length" />
    ///     or not.
    /// </summary>
    /// <returns>
    ///     The cumulative string that was built from invoking <see cref="BacktrackCharacter" />
    ///     <see cref="length" /> times.
    /// </returns>
    public string BacktrackRange(int length)
    {
        var backtrackBuilder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            if (PositionIndex == 0)
            {
                backtrackBuilder.Append(ParserFacts.END_OF_FILE);
                return backtrackBuilder.ToString();
            }

            BacktrackCharacter();

            backtrackBuilder.Append(PeekCharacter(0));
        }

        return backtrackBuilder.ToString();
    }

    public string PeekNextWord()
    {
         var nextWordBuilder = new StringBuilder();
    
         var i = 0;
    
         char peekedChar;
    
         do
         {
             peekedChar = PeekCharacter(i++);
    
             if (WhitespaceFacts.ALL.Contains(peekedChar) ||
                 KeyboardKeyFacts.IsPunctuationCharacter(peekedChar))
             {
                 break;
             }
    
             nextWordBuilder.Append(peekedChar);
         } while (peekedChar != ParserFacts.END_OF_FILE);

         return nextWordBuilder.ToString();
    }
    
    /// <summary>
    ///     Form a substring of the <see cref="_content" /> that starts
    ///     inclusively at the index <see cref="PositionIndex" /> and has a maximum
    ///     length of <see cref="substring" />.Length.
    ///     <br /><br />
    ///     This method uses <see cref="PeekRange" /> internally and therefore
    ///     will return a string that ends with <see cref="ParserFacts.END_OF_FILE" />
    ///     if an index out of bounds read was performed on <see cref="_content" />
    /// </summary>
    public bool CheckForSubstring(string substring)
    {
        var peekedSubstring = PeekRange(
            0,
            substring.Length);

        return peekedSubstring == substring;
    }

    public bool CheckForSubstringRange(ImmutableArray<string> substrings, out string? matchedOn)
    {
        foreach (var substring in substrings)
        {
            if (CheckForSubstring(substring))
            {
                matchedOn = substring;
                return true;
            }
        }

        matchedOn = null;
        return false;
    }

    public void WhileNotEndOfFile(Func<bool> shouldBreakFunc)
    {
        while (CurrentCharacter != ParserFacts.END_OF_FILE)
        {
            if (shouldBreakFunc.Invoke())
                break;

            _ = ReadCharacter();
        }
    }
    
    /// <summary>
    /// <see cref="ConsumeWord"/> will return immediately upon encountering whitespace.
    /// </summary>
    public (TextEditorTextSpan textSpan, string value) ConsumeWord(
        ImmutableArray<char>? additionalCharactersToBreakOn = null)
    {
        additionalCharactersToBreakOn ??= ImmutableArray<char>.Empty;
        
        // The wordBuilder is appended to everytime a
        // character is consumed.
        var wordBuilder = new StringBuilder();

        // wordBuilderStartingIndexInclusive == -1 is to mean
        // that wordBuilder is empty. Once the first letter or digit
        // (non whitespace) is read, then the wordBuilderStartingIndexInclusive
        // will be set to a value other than -1.
        var wordBuilderStartingIndexInclusive = -1;
        
        WhileNotEndOfFile(() =>
        {
            if (WhitespaceFacts.ALL.Contains(CurrentCharacter) ||
                additionalCharactersToBreakOn.Value.Contains(CurrentCharacter))
            {
                return true;
            }
            
            if (wordBuilderStartingIndexInclusive == -1)
            {
                // This is the start of a word
                // as opposed to the continuation of a word

                wordBuilderStartingIndexInclusive = PositionIndex;
            }
            
            wordBuilder.Append(CurrentCharacter);

            return false;
        });

        return (new TextEditorTextSpan(
            wordBuilderStartingIndexInclusive,
            PositionIndex,
            0),
                wordBuilder.ToString());
    }
}