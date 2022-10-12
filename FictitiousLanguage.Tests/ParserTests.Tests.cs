using FictitiousLanguage.ClassLib;
using FictitiousLanguage.ClassLib.Classes;

namespace FictitiousLanguage.Tests;

public partial class ParserTests
{
    [Fact]
    public void TestAddition()
    {
        var input = "3 + 6";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestSubtraction()
    {
        var input = "8 - 5";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.SubtractionOperatorSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestMultiplication()
    {
        var input = "8 * 5";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.MultiplicationOperatorSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestDivision()
    {
        var input = "21 / 7";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.DivisionOperatorSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestPrecedenceMultiplicationAndAddition()
    {
        var input = "3 + 2 * 4";
        
        /*
         * With precedence tree evaluates to 11:
         * 
         *              +
         *             / \
         *           3    *
         *              /   \
         *             2     4
         */
         
         /*
         * Without precedence tree evaluates to 20:
         * 
         *              *
         *             / \
         *           +    4
         *         /   \
         *       3      2
         */

        var tokens = UnitTestApi.Lex(input);
        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
            rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.MultiplicationOperatorSyntaxNode,
            rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[2].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
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
        
        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.MultiplicationOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.MultiplicationOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
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

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.MultiplicationOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionControl()
    {
        var input = $"{X} + {Y} / {Z}";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.DivisionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionNecessary()
    {
        var input = $"({X} + {Y}) / {Z}";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.DivisionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestParenthesizedExpressionRedundant()
    {
        var input = $"{X} + ({Y} / {Z})";

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        var rootSyntaxNode = compilationUnit.GetChildSyntaxNodes[0];

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.DivisionOperatorSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode, 
	        rootSyntaxNode.GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestVariableAssignmentExpression()
    {
        /*
         *
         *     x
         *     |
         *     2
         *
         */
        
        var input = $"var x = 2"; // 2

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        Assert.Equal(SyntaxNodeKind.VarKeywordModifierSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].Kind);
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AssignmentOperatorSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestVariableParenthesizedAssignmentExpressionWithAdd()
    {
        var input = $"var x; 4 + (x = 2);"; // 6

        var tokens = UnitTestApi.Lex(input);

        var compilationUnit = UnitTestApi.Parse(tokens);

        Assert.Equal(SyntaxNodeKind.VarKeywordModifierSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.AdditionOperatorSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[1].Kind);

        Assert.Equal(SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.AssignmentOperatorSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].GetChildSyntaxNodes[0].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestTwoPartVariableAssignmentStatement()
    {
        var input = $"var x; x = 2;";

        var tokens = UnitTestApi.Lex(input);
        var compilationUnit = UnitTestApi.Parse(tokens);

        Assert.Equal(SyntaxNodeKind.CompilationUnitSyntaxNode,
            compilationUnit.Kind);
        
        Assert.Equal(SyntaxNodeKind.VarKeywordModifierSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[0].Kind);
        Assert.Equal(SyntaxNodeKind.AssignmentOperatorSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[1].Kind);
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestOnePartVariableAssignmentStatement()
    {
        var input = $"var x = 2;";

        var tokens = UnitTestApi.Lex(input);
        var compilationUnit = UnitTestApi.Parse(tokens);

        Assert.Equal(SyntaxNodeKind.CompilationUnitSyntaxNode,
            compilationUnit.Kind);
        
        Assert.Equal(SyntaxNodeKind.VarKeywordModifierSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].Kind);

        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[0].GetChildSyntaxNodes[0].Kind);
        
        Assert.Equal(SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].Kind);
        
        Assert.Equal(SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[0].Kind);
        Assert.Equal(SyntaxNodeKind.AssignmentOperatorSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[1].Kind);
        Assert.Equal(SyntaxNodeKind.LiteralNumericExpressionSyntaxNode,
            compilationUnit.GetChildSyntaxNodes[1].GetChildSyntaxNodes[2].Kind);
    }
    
    [Fact]
    public void TestVariableAssignmentTwoStatements()
    {
        var input = $"var x = 2; x + 1;";

        var tokens = UnitTestApi.Lex(input);

        throw new NotImplementedException();
    }
}