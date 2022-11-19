using System.Text;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxTree
{
    /// <summary>
    /// First attempt at lexing JavaScript here will return
    /// List&lt;string&gt; as I want to start by parsing the Keywords
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static List<string> ParseText(string content)
    {
        // A single while loop will go character by character
        // until the end of the file for this method.
        var stringWalker = new StringWalker(content);

        // The wordBuilder is appended to everytime a
        // character is consumed.
        var wordBuilder = new StringBuilder();
        
        // When a keyword does not '.StartsWith()'
        // the 'wordBuilder.ToString()' then remove it
        // from the possibleKeywordsState List
        //
        // If one finds whitespace then check if any of the remaining
        // possibleKeywordsState entries are an exact match to the
        // 'wordBuilder.ToString()'. If there is a keyword
        // which is an exact match, then set that span of text
        // to have the keyword decoration byte.
        //
        // After finding a keyword, reset 'possibleKeywordsState' list
        // back to JavaScriptKeywords.All.ToList(); As well,
        // clear 'wordBuilder'
        var possibleKeywordsState = JavaScriptKeywords.All.ToList();
        
        stringWalker.WhileNotEndOfFile(() =>
        {
            if (stringWalker.CheckForSubstringRange(JavaScriptKeywords.All))
            {
                
            }
        });
    }
}