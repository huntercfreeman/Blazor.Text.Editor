using FictitiousLanguage.ClassLib;
using FictitiousLanguage.ClassLib.Classes;

namespace FictitiousLanguage.Tests;

public partial class LexerTests
{
    [Fact]
    public void TestWhitespaceNumericAndString()
    {
        var input = "123     \"alphabet\"";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralStringToken, tokens[2].Kind);
    }
    
    [Fact]
    public void TestPlus()
    {
        var input = "3 + 6";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
    }
    
    [Fact]
    public void TestMinus()
    {
        var input = "8 - 5";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.MinusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
    }
    
    [Fact]
    public void TestStar()
    {
        var input = "7 * 4";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.StarToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
    }
    
    [Fact]
    public void TestSlash()
    {
        var input = "21 / 7";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.SlashToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
    }

    /// <summary>
    /// Example: (1)
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionLiteral()
    {
        var x = 1;

        var input = $"({x})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[2].Kind);
    }
    
    /// <summary>
    /// Example: (1 + 2)
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionInnerThreePart()
    {
        var x = 1;
        var y = 2;

        var input = $"({x} + {y})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[6].Kind);
    }
    
    /// <summary>
    /// Example: (1 + 2) + 3
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionInnerThreePartEndingAdd()
    {
        var x = 1;
        var y = 2;
        var z = 3;

        var input = $"({x} + {y}) + {z}";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[6].Kind);        
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[10].Kind);
    }
    
    /// <summary>
    /// Example: 1 + (2 + 3)
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionInnerThreePartBeginningAdd()
    {
        var x = 1;
        var y = 2;
        var z = 3;

        var input = $"{x} + ({y} + {z})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[6].Kind);        
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[10].Kind);
    }
    
    /// <summary>
    /// Example: 1 + (2) + 3
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionBeginningAddParenthesizedLiteralEndingAdd()
    {
        var x = 1;
        var y = 2;
        var z = 3;

        var input = $"{x} + ({y}) + {z}";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[6].Kind);        
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[10].Kind);
    }
    
    /// <summary>
    /// Example: (2)(3)
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionParenthesizedLiteralImplicitlyMultipliedByParenthesizedLiteral()
    {
        var x = 2;
        var y = 3;

        var input = $"({x})({y})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[5].Kind);
    }
    
    /// <summary>
    /// Example: (2)3
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionParenthesizedLiteralImplicitlyMultipliedByLiteral()
    {
        var x = 2;
        var y = 3;

        var input = $"({x}){y}";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[3].Kind);
    }
    
    /// <summary>
    /// Example: 2(3)
    /// </summary>
    [Fact]
    public void TestParenthesizedExpressionLiteralImplicitlyMultipliedByParenthesizedLiteral()
    {
        var x = 2;
        var y = 3;

        var input = $"{x}({y})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[3].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionControl()
    {
        var input = $"{X} + {Y} / {Z}";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.SlashToken, tokens[6].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[8].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionNecessary()
    {
        var input = $"({X} + {Y}) / {Z}";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[6].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.SlashToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[10].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionRedundant()
    {
        var input = $"{X} + ({Y} / {Z})";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.OpenParenthesisToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[6].Kind);
        Assert.Equal(SyntaxTokenKind.SlashToken, tokens[7].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.CloseParenthesisToken, tokens[10].Kind);
    }

    [Fact]
    public void TestVariableAssignmentExpression()
    {
        var input = $"x = 2";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.VariableIdentifierToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.EqualsToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[4].Kind);
    }
    
    [Fact]
    public void TestVariableAssignmentStatement()
    {
        var input = $"var x = 2;";

        var tokens = UnitTestApi.Lex(input);

        Assert.Equal(SyntaxTokenKind.KeywordToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.VariableIdentifierToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.EqualsToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[6].Kind);
        Assert.Equal(SyntaxTokenKind.StatementDelimiterToken, tokens[7].Kind);
    }
    
    [Fact]
    public void TestVariableAssignmentTwoStatements()
    {
        var input = $"var x = 2; x + 1;";

        var tokens = UnitTestApi.Lex(input);

        // Statement One
        Assert.Equal(SyntaxTokenKind.KeywordToken, tokens[0].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[1].Kind);
        Assert.Equal(SyntaxTokenKind.VariableIdentifierToken, tokens[2].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[3].Kind);
        Assert.Equal(SyntaxTokenKind.EqualsToken, tokens[4].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[5].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[6].Kind);
        Assert.Equal(SyntaxTokenKind.StatementDelimiterToken, tokens[7].Kind);

        // Statement Two
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[8].Kind);
        Assert.Equal(SyntaxTokenKind.VariableIdentifierToken, tokens[9].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[10].Kind);
        Assert.Equal(SyntaxTokenKind.PlusToken, tokens[11].Kind);
        Assert.Equal(SyntaxTokenKind.WhitespaceToken, tokens[12].Kind);
        Assert.Equal(SyntaxTokenKind.LiteralNumericToken, tokens[13].Kind);
        Assert.Equal(SyntaxTokenKind.StatementDelimiterToken, tokens[14].Kind);
    }
}