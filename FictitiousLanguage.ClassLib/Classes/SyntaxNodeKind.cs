namespace FictitiousLanguage.ClassLib.Classes;

public enum SyntaxNodeKind
{
    TriviaSyntaxNode,
    StringExpressionSyntaxNode,
    AdditionOperatorSyntaxNode,
    SubtractionOperatorSyntaxNode,
    MultiplicationOperatorSyntaxNode,
    DivisionOperatorSyntaxNode,
    LiteralNumericExpressionSyntaxNode,
    ThreePartNumericExpressionSyntaxNode,
    ParenthesizedNumericExpressionSyntaxNode,
    VariableBoundNumericExpressionSyntaxNode,
    AssignmentOperatorSyntaxNode,
    StatementSyntaxNode,
    KeywordModifierSyntaxNode,
    VarKeywordModifierSyntaxNode,
    CompilationUnitSyntaxNode
}