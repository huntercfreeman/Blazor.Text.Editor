using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens.Keywords;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes;

public sealed class Lexer
{
    private List<ISyntaxToken> _tokens = new();
    private int _startPosition = 0;
    private int _currentPosition = 0;
    private string _sourceText;
    private SyntaxToken _currentToken;
    private List<DiagnosticBlazorStudio> _diagnostics = new();

    public List<DiagnosticBlazorStudio> Diagnostics => _diagnostics;

    public ImmutableArray<ISyntaxToken> Lex(string input)
    {
        _sourceText = input;

        while (_currentPosition < input.Length)
        {
            var currentChar = PeekChar(0);

            switch (char.ToLower(currentChar))
            {
                // String Beginning
                case '"':
                    HandleLiteralString();
                    break;
                // Letters
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    HandleVariableIdentifier();
                    break;
                // Numbers
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    HandleLiteralNumeric();
                    break;
                case ';':
                    HandleStatementDelimiter();
                    break;
                case '+':
                    HandlePlus();
                    break;
                case '-':
                    HandleMinus();
                    break;
                case '*':
                    HandleStar();
                    break;
                case '/':
                    HandleSlash();
                    break;
                case '=':
                    HandleEquals();
                    break;
                case '(':
                    HandleOpenParenthesis();
                    break;
                case ')':
                    HandleCloseParenthesis();
                    break;
                default:
                    HandleWhitespace();
                    break;
            }
        }

        return _tokens.ToImmutableArray();
    }

    private void HandleVariableIdentifier()
    {
        _startPosition = _currentPosition;

        while (_currentPosition < _sourceText.Length)
        {
            var currentChar = PeekChar(0);

            if (char.IsLetter(currentChar))
                _ = ConsumeChar();
            else
                break;
        }
        
        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        if (SyntaxTokenFacts.KeywordsList.Contains(substring))
        {
            HandleKeyword(substring);
            return;
        }

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new VariableIdentifierSyntaxToken(textSpan, substring);

        _tokens.Add(_currentToken);
    }
    
    private void HandleKeyword(string keywordString)
    {
        var textSpan = new TextSpan(_startPosition, _currentPosition, keywordString);

        _currentToken = new KeywordSyntaxToken(textSpan, keywordString);

        _tokens.Add(_currentToken);
    }
    
    private void HandleLiteralString()
    {
        _startPosition = ++_currentPosition;

        while (_currentPosition < _sourceText.Length && PeekChar(0) != '"')
        {
            var currentChar = PeekChar(0);

            if (char.IsLetter(currentChar) || char.IsDigit(currentChar))
                _ = ConsumeChar();
            else
                break;
        }

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new LiteralStringSyntaxToken(textSpan, substring);

        _tokens.Add(_currentToken);
    }

    private void HandleLiteralNumeric()
    {
        _startPosition = _currentPosition;

        while (_currentPosition < _sourceText.Length)
        {
            var currentChar = PeekChar(0);

            if (char.IsDigit(currentChar))
                _ = ConsumeChar();
            else
                break;
        }

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        var value = int.Parse(substring);

        _currentToken = new LiteralNumericSyntaxToken(textSpan, value);

        _tokens.Add(_currentToken);
    }
    
    private void HandleStatementDelimiter()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new StatementDelimiterSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }

    private void HandlePlus()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new PlusSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }
    
    private void HandleMinus()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new MinusSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }
    
    private void HandleStar()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new StarSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }
    
    private void HandleSlash()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new SlashSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }

    private void HandleEquals()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new EqualsSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }

    private void HandleOpenParenthesis()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new OpenParenthesisSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }
    
    private void HandleCloseParenthesis()
    {
        _startPosition = _currentPosition;

        var currentChar = ConsumeChar();

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new CloseParenthesisSyntaxToken(textSpan, currentChar);

        _tokens.Add(_currentToken);
    }

    private void HandleWhitespace()
    {
        _startPosition = _currentPosition;

        while (_currentPosition < _sourceText.Length)
        {
            var currentChar = PeekChar(0);

            if (char.IsWhiteSpace(currentChar))
                _ = ConsumeChar();
            else
                break;
        }

        if (_startPosition == _currentPosition)
        {
            // Bad Character
            _currentPosition++;
            return;
        }

        var substring = _sourceText.Substring(_startPosition,
            _currentPosition - _startPosition);

        var textSpan = new TextSpan(_startPosition, _currentPosition, substring);

        _currentToken = new WhitespaceSyntaxToken(textSpan, substring);

        _tokens.Add(_currentToken);
    }

    /// <summary>
    /// From current index position add the offset and 
    /// return the character at that index.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    private char PeekChar(int offset)
    {
        var peekIndex = _currentPosition + offset;

        if (peekIndex >= _sourceText.Length)
            return '\0';

        return _sourceText[peekIndex];
    }

    /// <summary>
    /// Returns the current character and increments
    /// the index position
    /// </summary>
    /// <returns></returns>
    private char ConsumeChar()
    {
        if (_currentPosition >= _sourceText.Length)
            return '\0';

        return _sourceText[_currentPosition++];
    }
}