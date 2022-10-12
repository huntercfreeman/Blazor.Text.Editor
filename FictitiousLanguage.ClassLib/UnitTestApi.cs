using FictitiousLanguage.ClassLib.Classes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;
using System.Collections.Immutable;

namespace FictitiousLanguage.ClassLib;

public static class UnitTestApi
{
    public static ImmutableArray<ISyntaxToken> Lex(string input)
    {
        Lexer lexer = new();

        return lexer.Lex(input);
    }

    public static ISyntaxNode Parse(ImmutableArray<ISyntaxToken> tokens)
    {
        Parser parser = new();

        return parser.Parse(tokens);
    }
        
    public static EvaluatorResult Evaluate(ISyntaxNode rootSyntaxNode)
    {
        Evaluator evaluator = new();
            
        return evaluator.Evaluate(rootSyntaxNode);
    }
}