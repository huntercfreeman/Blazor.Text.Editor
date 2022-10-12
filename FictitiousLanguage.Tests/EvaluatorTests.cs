using FictitiousLanguage.ClassLib;

namespace FictitiousLanguage.Tests;
public class EvaluatorTests
{
    [Fact]
    public void TestAddition()
    {
        var firstArgument = 3;
        var secondArgument = 6;

        var input = $"{firstArgument} + {secondArgument}";

        var tokens = UnitTestApi.Lex(input);

        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);

        Assert.Equal(firstArgument + secondArgument,
            evaluatorResult.ResultValue);
    }
    
    [Fact]
    public void TestSubtraction()
    {
        var firstArgument = 8;
        var secondArgument = 5;

        var input = $"{firstArgument} - {secondArgument}";

        var tokens = UnitTestApi.Lex(input);

        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);

        Assert.Equal(firstArgument - secondArgument,
            evaluatorResult.ResultValue);
    }
    
    [Fact]
    public void TestMultiplication()
    {
        var firstArgument = 7;
        var secondArgument = 4;

        var input = $"{firstArgument} * {secondArgument}";

        var tokens = UnitTestApi.Lex(input);

        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);

        Assert.Equal(firstArgument * secondArgument,
            evaluatorResult.ResultValue);
    }
    
    [Fact]
    public void TestDivision()
    {
        var firstArgument = 21;
        var secondArgument = 7;

        var input = $"{firstArgument} / {secondArgument}";

        var tokens = UnitTestApi.Lex(input);

        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);

        Assert.Equal(firstArgument / secondArgument,
            evaluatorResult.ResultValue);
    }
    
    [Fact]
    public void TestPrecedenceMultiplicationAndAddition()
    {
        var x = 3;
        var y = 2;
        var z = 4;
        
        var input = $"{x} + {y} * {z}";
        
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
        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);
        Assert.Equal(x + y * z, evaluatorResult.ResultValue);
    }
    
    [Fact]
    public void TestVariableAssignmentExpression()
    {
        var variableValue = 2;
        
        var input = $"var x = {variableValue}"; // 2

        var tokens = UnitTestApi.Lex(input);
        var rootSyntaxNode = UnitTestApi.Parse(tokens);

        var evaluatorResult = UnitTestApi.Evaluate(rootSyntaxNode);
        
        Assert.IsType<int>(evaluatorResult.ResultValue);
        Assert.Equal(variableValue, evaluatorResult.ResultValue);
    }
}