using FictitiousLanguage.ClassLib.Classes.SyntaxNodes;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

namespace FictitiousLanguage.ClassLib.Classes;

public sealed class Evaluator
{
    private readonly Dictionary<string, object> _variables = new();
    private List<DiagnosticBlazorStudio> _diagnostics = new();

    public List<DiagnosticBlazorStudio> Diagnostics => _diagnostics;

    public EvaluatorResult? Evaluate(ISyntaxNode rootSyntaxNode)
    {
        if (rootSyntaxNode is CompilationUnit)
        {
            EvaluatorResult? evaluatorResult = null;

            foreach (var statement in rootSyntaxNode.GetChildSyntaxNodes)
            {
                if (statement is NumericExpressionSyntaxNode numericExpressionSyntaxNode)
                {
                    evaluatorResult = HandleNumericExpressionSyntaxNode(
                        numericExpressionSyntaxNode,
                        null
                    );
                }
                else
                {
                    Console.WriteLine($"statement.Kind: {statement.Kind}");
                }
            }

            return evaluatorResult;
        }
        
        if (rootSyntaxNode is NumericExpressionSyntaxNode)
        {
            return HandleNumericExpressionSyntaxNode(
                (NumericExpressionSyntaxNode)rootSyntaxNode,
                null
            );
        }

        return null;
    }

    private EvaluatorResult HandleNumericExpressionSyntaxNode(NumericExpressionSyntaxNode syntaxNode,
        NumericExpressionSyntaxNode parentExpression)
    {
        if (parentExpression is not null)
        {
            if (parentExpression is not NumericExpressionSyntaxNode)
            {
                throw new ApplicationException($"{nameof(NumericExpressionSyntaxNode)} " +
                                               $"cannot have parent expression of type " +
                                               $"{parentExpression.Kind}");
            }
        }

        if (syntaxNode is LiteralNumericExpressionSyntaxNode literalNumericExpressionSyntaxNode)
        {
            return new EvaluatorResult
            {
                ResultType = typeof(int),
                ResultValue = literalNumericExpressionSyntaxNode.LiteralNumericSyntaxToken.Value
            };
        }

        if (syntaxNode is ParenthesizedNumericExpressionSyntaxNode parenthesizedNumericExpressionSyntaxNode)
        {
            if (parenthesizedNumericExpressionSyntaxNode.InnerNumericExpression is not null)
            {
                if (parenthesizedNumericExpressionSyntaxNode.CloseParenthesis is null)
                {
                    _diagnostics.Add(new DiagnosticBlazorStudio(
                        $"{nameof(ParenthesizedNumericExpressionSyntaxNode.CloseParenthesis)} of " +
                        $"{nameof(ParenthesizedNumericExpressionSyntaxNode)} was null",
                        DiagnosticLevel.Error));
                }

                return new EvaluatorResult
                {
                    ResultType = typeof(int),
                    ResultValue = HandleNumericExpressionSyntaxNode(parenthesizedNumericExpressionSyntaxNode.InnerNumericExpression,
                            parenthesizedNumericExpressionSyntaxNode)
                        .ResultValue
                };
            }

            _diagnostics.Add(new DiagnosticBlazorStudio(
                $"{nameof(ParenthesizedNumericExpressionSyntaxNode.InnerNumericExpression)} of " +
                $"{nameof(ParenthesizedNumericExpressionSyntaxNode)} was null",
                DiagnosticLevel.Error));

            return null;
        }

        if (syntaxNode is not ThreePartNumericExpressionSyntaxNode threePartNumericExpressionSyntaxNode)
        {
            // TODO: Handle root nodes of non ThreePartNumericExpressionSyntaxNode type
            _diagnostics.Add(new DiagnosticBlazorStudio(
                $"Root node of parsed result not handled yet",
                DiagnosticLevel.Error));

            return null;
        }

        if (threePartNumericExpressionSyntaxNode.OperatorSyntaxNode.Kind == SyntaxNodeKind.AssignmentOperatorSyntaxNode)
        {
            try
            {
                return HandleAssignment(threePartNumericExpressionSyntaxNode);
            }
            catch (ApplicationException e)
            {
                _diagnostics.Add(new DiagnosticBlazorStudio(
                    e.Message,
                    DiagnosticLevel.Error));

                return null;
            }
        }

        var leftExpressionResult = HandleNumericExpressionSyntaxNode(threePartNumericExpressionSyntaxNode.LeftExpressionNode,
            threePartNumericExpressionSyntaxNode);

        var rightExpressionResult = HandleNumericExpressionSyntaxNode(threePartNumericExpressionSyntaxNode.RightExpressionNode,
            threePartNumericExpressionSyntaxNode);

        if (leftExpressionResult is null)
        {
            _diagnostics.Add(new DiagnosticBlazorStudio(
                $"Left expression result is null",
                DiagnosticLevel.Error));

            return null;
        }
        
        if (rightExpressionResult is null)
        {
            _diagnostics.Add(new DiagnosticBlazorStudio(
                $"Right expression result is null",
                DiagnosticLevel.Error));

            return null;
        }
        
        var leftValue = (int)leftExpressionResult.ResultValue;
        var rightValue = (int)rightExpressionResult.ResultValue;

        return PerformNumericCalculation(leftValue, threePartNumericExpressionSyntaxNode.OperatorSyntaxNode, rightValue);
    }

    private EvaluatorResult PerformNumericCalculation(int leftValue,
        OperatorSyntaxNode operatorSyntaxNode,
        int rightValue)
    {
        Type resultType;
        object resultValue;

        switch (operatorSyntaxNode.Kind)
        {
            case SyntaxNodeKind.AdditionOperatorSyntaxNode:
                resultType = typeof(int);
                resultValue = leftValue + rightValue;
                break;
            case SyntaxNodeKind.SubtractionOperatorSyntaxNode:
                resultType = typeof(int);
                resultValue = leftValue - rightValue;
                break;
            case SyntaxNodeKind.MultiplicationOperatorSyntaxNode:
                resultType = typeof(int);
                resultValue = leftValue * rightValue;
                break;
            case SyntaxNodeKind.DivisionOperatorSyntaxNode:
                resultType = typeof(int);
                resultValue = leftValue / rightValue;
                break;
            default:
                resultType = null;
                resultValue = null;
                break;
        }

        return new EvaluatorResult
        {
            ResultType = resultType,
            ResultValue = resultValue
        };
    }

    private EvaluatorResult HandleAssignment(ThreePartNumericExpressionSyntaxNode threePartNumericExpressionSyntaxNode)
    {
        if (threePartNumericExpressionSyntaxNode.LeftExpressionNode is not 
            VariableBoundNumericExpressionSyntaxNode variableBoundNumericExpressionSyntaxNode)
        {
            throw new ApplicationException(
                $"Left expression node must be of type {nameof(VariableBoundNumericExpressionSyntaxNode)}");
        }
        
        if (threePartNumericExpressionSyntaxNode.OperatorSyntaxNode is not 
            AssignmentOperatorSyntaxNode assignmentOperatorSyntaxNode)
        {
            throw new ApplicationException(
                $"Operator syntaxNode node must be of type {nameof(assignmentOperatorSyntaxNode)}");
        }
        
        if (threePartNumericExpressionSyntaxNode.RightExpressionNode is not 
            NumericExpressionSyntaxNode numericExpressionSyntaxNode)
        {
            throw new ApplicationException(
                $"Right expression node must be of type {nameof(NumericExpressionSyntaxNode)}");
        }

        var variableName = variableBoundNumericExpressionSyntaxNode.VariableIdentifierSyntaxToken.TextSpan.Text;

        variableBoundNumericExpressionSyntaxNode.InnerNumericExpression = numericExpressionSyntaxNode;
        
        _variables[variableName] = HandleNumericExpressionSyntaxNode(numericExpressionSyntaxNode,
                threePartNumericExpressionSyntaxNode)
            .ResultValue;

        return new EvaluatorResult
        {
            ResultType = typeof(int),
            ResultValue = (int)_variables[variableName]
        };
    }
}