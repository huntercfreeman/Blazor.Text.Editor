using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public class CssSyntaxTree
{
    public static List<TextEditorTextSpan> ParseText(string content)
    {
        // Pseudo code. Indentation is to imply
        // the line is dependent on the previous line being true.
        //
        // Comments
        // Tag Selectors
        //
        // States:
        // Inside css block => true when: '{'
        //     Outside css block '}'
        //
        // *NOTE* If 'Inside css block' is a boolean
        // how would one handle nesting in css compilers
        //
        // *NOTE*
        // -Do not make any assumptions from whitespace
        //     -One might indent a selector that is accessing a child of an accessor written above it
        //     -One might put the css block on one line ".myClass { height: 500px; }"
        // -Are semicolons a requirement in css?

        return new List<TextEditorTextSpan>();
    }
}