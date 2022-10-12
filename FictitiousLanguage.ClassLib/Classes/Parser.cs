using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes;

public sealed class Parser
{
    private readonly Dictionary<string, object> _variables = new();

    private ISyntaxNode? _nodeRoot;
    private Stack<ISyntaxNode> _nodeStack = new();
    private List<DiagnosticBlazorStudio> _diagnostics = new();
    private CompilationUnit _compilationUnit = new();

    public List<DiagnosticBlazorStudio> Diagnostics => _diagnostics;

    public CompilationUnit Parse(ImmutableArray<ISyntaxToken> tokens)
    {
        foreach (var token in tokens)
        {
            if (_diagnostics.Any())
                break;
            
            switch (token.Kind)
            {
                case SyntaxTokenKind.PlusToken:
                    HandlePlusToken(token);
                    break;
                case SyntaxTokenKind.MinusToken:
                    HandleMinusToken(token);
                    break;
                case SyntaxTokenKind.StarToken:
                    HandleStarToken(token);
                    break;
                case SyntaxTokenKind.SlashToken:
                    HandleSlashToken(token);
                    break;
                case SyntaxTokenKind.OpenParenthesisToken:
                    HandleOpenParenthesisToken(token);
                    break;
                case SyntaxTokenKind.CloseParenthesisToken:
                    HandleCloseParenthesisToken(token);
                    break;
                case SyntaxTokenKind.EqualsToken:
                    HandleEqualsToken(token);
                    break;
                case SyntaxTokenKind.VariableIdentifierToken:
                    HandleVariableIdentifierToken(token);
                    break;
                case SyntaxTokenKind.KeywordToken:
                    HandleKeywordToken(token);
                    break;
                case SyntaxTokenKind.StatementDelimiterToken:
                    HandleStatementDelimiterToken(token);
                    break;
                case SyntaxTokenKind.LiteralNumericToken:
                    HandleLiteralNumericToken(token);
                    break;
                case SyntaxTokenKind.LiteralStringToken:
                    break;
                case SyntaxTokenKind.WhitespaceToken:
                default:
                    break;
            }
        }

        // Is an expression
        if (_nodeRoot is not null)
            _compilationUnit.SyntaxNodes.Add(_nodeRoot);
        
        return _compilationUnit;
    }

    private void SetThreePartNumericExpression(NumericExpressionSyntaxNode previousNumericExpressionNode,
        OperatorSyntaxNode operatorNode)
    {
        var newThreePartNumericExpressionNode =
            new ThreePartNumericExpressionSyntaxNode(null,
                operatorNode,
                null);

        var handledPreviousThreePartNumericExpressionNode = false;

        if (previousNumericExpressionNode is ThreePartNumericExpressionSyntaxNode previousThreePartNumericExpressionNode)
        {
            if (previousThreePartNumericExpressionNode.RightExpressionNode is null)
            {
                throw new ApplicationException($"Cannot have two sequential operators.");
            }

            if (GetOperatorPrecedence(operatorNode)
                > GetOperatorPrecedence(previousThreePartNumericExpressionNode.OperatorSyntaxNode))
            {
                var temporaryNumericExpression = previousThreePartNumericExpressionNode.RightExpressionNode;

                newThreePartNumericExpressionNode.LeftExpressionNode = temporaryNumericExpression;

                previousThreePartNumericExpressionNode.RightExpressionNode = newThreePartNumericExpressionNode;

                handledPreviousThreePartNumericExpressionNode = true;
            }
        }

        ISyntaxNode? parentOfPreviousNode = null;

        if (!handledPreviousThreePartNumericExpressionNode)
        {
            newThreePartNumericExpressionNode.LeftExpressionNode = previousNumericExpressionNode;

            if (_nodeStack.TryPeek(out parentOfPreviousNode) &&
                parentOfPreviousNode is ParenthesizedNumericExpressionSyntaxNode parenthesizedNumericExpressionNode)
            {
                parenthesizedNumericExpressionNode.InnerNumericExpression = newThreePartNumericExpressionNode;
            }
            else
            {
                _nodeRoot = newThreePartNumericExpressionNode;
            }
        }

        _nodeStack.Push(newThreePartNumericExpressionNode);
        _nodeRoot ??= newThreePartNumericExpressionNode;
    }

    private void HandlePlusToken(ISyntaxToken token)
    {
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            SetThreePartNumericExpression(previousNumericExpressionNode, new AdditionOperatorSyntaxNode(token));
        }
        else
        {
            throw new ApplicationException($"{nameof(AdditionOperatorSyntaxNode)} requires a " +
                                           $"previous {nameof(NumericExpressionSyntaxNode)}");
        }
    }

    private void HandleMinusToken(ISyntaxToken token)
    {
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            SetThreePartNumericExpression(previousNumericExpressionNode, new SubtractionOperatorSyntaxNode(token));
        }
        else
        {
            throw new ApplicationException($"{nameof(SubtractionOperatorSyntaxNode)} requires a " +
                                           $"previous {nameof(NumericExpressionSyntaxNode)}");
        }
    }

    private void HandleStarToken(ISyntaxToken token)
    {
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            SetThreePartNumericExpression(previousNumericExpressionNode, new MultiplicationOperatorSyntaxNode(token));
        }
        else
        {
            throw new ApplicationException($"{nameof(MultiplicationOperatorSyntaxNode)} requires a " +
                                           $"previous {nameof(NumericExpressionSyntaxNode)}");
        }
    }

    private void HandleSlashToken(ISyntaxToken token)
    {
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            SetThreePartNumericExpression(previousNumericExpressionNode, new DivisionOperatorSyntaxNode(token));
        }
        else
        {
            throw new ApplicationException($"{nameof(DivisionOperatorSyntaxNode)} requires a " +
                                           $"previous {nameof(NumericExpressionSyntaxNode)}");
        }
    }

    private void HandleEqualsToken(ISyntaxToken token)
    {
        // x = 2
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is VariableBoundNumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            SetThreePartNumericExpression(previousNumericExpressionNode, new AssignmentOperatorSyntaxNode(token));
        }
        else
        {
            throw new ApplicationException($"{nameof(AssignmentOperatorSyntaxNode)} requires a " +
                                           $"previous {nameof(VariableBoundNumericExpressionSyntaxNode)}");
        }
    }

    private void HandleOpenParenthesisToken(ISyntaxToken token)
    {
        var parenthesizedExpressionNode = new ParenthesizedNumericExpressionSyntaxNode(token);

        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            // TODO: Convert to switch using a enum of SyntaxNodeKind
            if (previousNode is ThreePartNumericExpressionSyntaxNode threePartNumericExpressionNode)
            {
                threePartNumericExpressionNode.RightExpressionNode = parenthesizedExpressionNode;

                _nodeStack.Push(threePartNumericExpressionNode);
                _nodeStack.Push(parenthesizedExpressionNode);
            }
            else if (previousNode is ParenthesizedNumericExpressionSyntaxNode || previousNode is LiteralNumericExpressionSyntaxNode)
            {
                SetThreePartNumericExpression(previousNumericExpressionNode,
                    new MultiplicationOperatorSyntaxNode(new StarSyntaxToken(new TextSpan(token.TextSpan.StartingIndex,
                        token.TextSpan.EndingIndex, token.TextSpan.Text), token.Value)));

                _nodeStack.TryPop(out var newNumericExpressionNode);

                var newThreePartNumericExpressionNode = (ThreePartNumericExpressionSyntaxNode)newNumericExpressionNode;

                newThreePartNumericExpressionNode.RightExpressionNode = parenthesizedExpressionNode;

                _nodeStack.Push(newThreePartNumericExpressionNode);
                _nodeStack.Push(parenthesizedExpressionNode);
            }
            else
            {
                throw new ApplicationException(
                    $"{nameof(previousNode.Kind)} was not expected before a {nameof(OpenParenthesisSyntaxToken)}.");
            }
        }
        else
        {
            _nodeStack.Push(parenthesizedExpressionNode);
            _nodeRoot = parenthesizedExpressionNode;
        }
    }

    private void HandleCloseParenthesisToken(ISyntaxToken token)
    {
        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            if (_nodeStack.TryPop(out var parentOfPreviousNode) &&
                parentOfPreviousNode is ParenthesizedNumericExpressionSyntaxNode parenthesizedExpressionNode)
            {
                parenthesizedExpressionNode.CloseParenthesis = (CloseParenthesisSyntaxToken)token;

                var peekedWasSuccessful = _nodeStack.TryPeek(out var peekedNode);

                if (!peekedWasSuccessful || peekedNode is not ThreePartNumericExpressionSyntaxNode)
                {
                    _nodeStack.Push(parenthesizedExpressionNode);
                }
            }
            else
            {
                _diagnostics.Add(new DiagnosticBlazorStudio(
                    $"{nameof(ParenthesizedNumericExpressionSyntaxNode.InnerNumericExpression)} " +
                    $"of {nameof(ParenthesizedNumericExpressionSyntaxNode)} was null",
                    DiagnosticLevel.Error));
            }
        }
        else
        {
            throw new ApplicationException($"{nameof(CloseParenthesisSyntaxToken)} requires a " +
                                           $"matching {nameof(OpenParenthesisSyntaxToken)}");
        }
    }

    private void HandleVariableIdentifierToken(ISyntaxToken token)
    {
        var variableBoundNumericExpressionNode = new VariableBoundNumericExpressionSyntaxNode(token);

        var validVariableIdentifierToken = true;

        if (_nodeStack.TryPop(out var previousNode))
        {
            if (previousNode is VarKeywordModifierSyntaxNode varKeywordModifierSyntaxNode)
            {
                try
                {
                    _variables.Add(token.TextSpan.Text, null);
                    
                    varKeywordModifierSyntaxNode.InnerVariableBoundNumericExpressionSyntaxNode =
                        variableBoundNumericExpressionNode;

                    _compilationUnit.SyntaxNodes.Add(varKeywordModifierSyntaxNode);
                    _nodeRoot = null;
                    _nodeStack.Push(variableBoundNumericExpressionNode);
                    return;
                }
                catch (ArgumentException e)
                {
                    _diagnostics.Add(new DiagnosticBlazorStudio($"Variable: \"{token.TextSpan.Text}\", already exists.", 
                        DiagnosticLevel.Error));
                    return;
                }
            }
            else
            {
                if (!_variables.ContainsKey(token.TextSpan.Text))
                    validVariableIdentifierToken = false;
            }
        }
        else
        {
            if(!_variables.ContainsKey(token.TextSpan.Text))
                validVariableIdentifierToken = false;
        }

        if (!validVariableIdentifierToken)
        {
            _diagnostics.Add(new DiagnosticBlazorStudio($"Variable: \"{token.TextSpan.Text}\", has not been declared.",
                DiagnosticLevel.Error));

            return;
        }
        
        _nodeStack.Push(variableBoundNumericExpressionNode);
        _nodeRoot ??= variableBoundNumericExpressionNode;
    }
    
    private void HandleKeywordToken(ISyntaxToken token)
    {
        KeywordModifierSyntaxNode keywordModifierNode;
        
        switch (token.TextSpan.Text)
        {
            case SyntaxTokenFacts.Keywords.VARIABLE_DECLARATION_KEYWORD:
                keywordModifierNode = new VarKeywordModifierSyntaxNode(token);
                break;
            default:
                keywordModifierNode = new VarKeywordModifierSyntaxNode(token);
                break;
        }

        _nodeStack.Push(keywordModifierNode);
        _nodeRoot ??= keywordModifierNode;
    }
    
    private void HandleStatementDelimiterToken(ISyntaxToken token)
    {
        if (_nodeRoot is not null)
        {
            _compilationUnit.SyntaxNodes.Add(_nodeRoot);
            
            _nodeRoot = null;
        }

        _nodeStack.Clear();
    }

    private void HandleLiteralNumericToken(ISyntaxToken token)
    {
        var literalNumericExpressionNode = new LiteralNumericExpressionSyntaxNode(token);

        if (_nodeStack.TryPop(out var previousNode) &&
            previousNode is NumericExpressionSyntaxNode previousNumericExpressionNode)
        {
            switch (previousNumericExpressionNode.Kind)
            {
                case SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode:
                    var threePartNumericExpressionNode =
                        (ThreePartNumericExpressionSyntaxNode)previousNumericExpressionNode;

                    threePartNumericExpressionNode.RightExpressionNode = literalNumericExpressionNode;

                    _nodeStack.Push(previousNumericExpressionNode);
                    break;
                case SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode:
                    var parenthesizedExpressionNode =
                        (ParenthesizedNumericExpressionSyntaxNode)previousNumericExpressionNode;

                    if (parenthesizedExpressionNode.InnerNumericExpression is null)
                    {
                        parenthesizedExpressionNode.InnerNumericExpression = literalNumericExpressionNode;

                        _nodeStack.Push(previousNumericExpressionNode);
                        _nodeStack.Push(literalNumericExpressionNode);
                    }
                    else
                    {
                        SetThreePartNumericExpression(parenthesizedExpressionNode,
                            new MultiplicationOperatorSyntaxNode(new StarSyntaxToken(new TextSpan(token.TextSpan.StartingIndex,
                                token.TextSpan.EndingIndex, token.TextSpan.Text), token.Value)));

                        _nodeStack.TryPop(out var newNumericExpressionNode);

                        var newThreePartNumericExpressionNode = (ThreePartNumericExpressionSyntaxNode)newNumericExpressionNode;

                        newThreePartNumericExpressionNode.RightExpressionNode = literalNumericExpressionNode;

                        _nodeStack.Push(newThreePartNumericExpressionNode);
                    }

                    break;
            }
        }
        else
        {
            _nodeStack.Push(literalNumericExpressionNode);
            _nodeRoot = literalNumericExpressionNode;
        }
    }

    private int GetOperatorPrecedence(OperatorSyntaxNode operatorSyntaxNode)
    {
        switch (operatorSyntaxNode.Kind)
        {
            case SyntaxNodeKind.MultiplicationOperatorSyntaxNode:
            case SyntaxNodeKind.DivisionOperatorSyntaxNode:
                return 1;
            case SyntaxNodeKind.AdditionOperatorSyntaxNode:
            case SyntaxNodeKind.SubtractionOperatorSyntaxNode:
            default:
                return 0;
        }
    }

    private int GetExpressionPrecedence(NumericExpressionSyntaxNode numericExpressionSyntaxNode)
    {
        switch (numericExpressionSyntaxNode.Kind)
        {
            case SyntaxNodeKind.LiteralNumericExpressionSyntaxNode:
                return 2;
            case SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode:
                return 1;
            case SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode:
            default:
                return 0;
        }
    }

    private string ReportInvalidNumericExpression(string actualKind)
    {
        return $"{actualKind} was not of type " +
               $"{nameof(NumericExpressionSyntaxNode)}";
    }
}