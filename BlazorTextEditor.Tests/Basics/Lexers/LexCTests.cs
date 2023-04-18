using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.C.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.Tests.Basics.Lexers;

public class LexCTests
{
    [Fact]
    public async Task SHOULD_LEX_COMMENTS_SINGLE_LINE()
    {
        // TODO: What I say in the below comment about these local strings being interned I have doubts of.
        // this doesn't hold true here because I'm invoking a method on it?
        // Interning I'm thinking is done at compile time with string literals.
        // Any string made at runtime will that also get the same functionality?
        // I am using .ReplaceLineEndings("\n") to standardize the line endings of my verbatim string
        // across various operating systems. But doing so means I'm making a string at runtime.
        // I could use a non-verbatim string and manually insert '\n' characters.
        // But would this be annoying to read?
        //
        // Strings will be interned by the runtime so that these local
        // declarations of the same string point to the same immutable string in memory.
        //
        // Keeping the strings local to the method makes it far less likely to clobber
        // someone else's unit test.
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");
        
        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(40, 68, (byte)GenericDecorationKind.CommentSingleLine),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.CommentSingleLine)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task SHOULD_LEX_COMMENTS_MULTI_LINE()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");
        
        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(98, 127, (byte)GenericDecorationKind.CommentMultiLine),
            new(131, 163, (byte)GenericDecorationKind.CommentMultiLine),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.CommentMultiLine)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task SHOULD_LEX_KEYWORDS()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(70, 73, (byte)GenericDecorationKind.Keyword),
            new(84, 87, (byte)GenericDecorationKind.Keyword),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task SHOULD_LEX_STRING_LITERALS()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(173, 177, (byte)GenericDecorationKind.StringLiteral),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.StringLiteral)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task SHOULD_LEX_FUNCTIONS()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(74, 78, (byte)GenericDecorationKind.Function),
            new(166, 172, (byte)GenericDecorationKind.Function),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.Function)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task SHOULD_LEX_PREPROCESSOR_DIRECTIVES()
    {
        string testDataHelloWorld = @"#include <stdlib.h>
#include <stdio.h>

// C:\Users\hunte\Repos\Aaa\

int main()
{
	int x = 42;

	/*
		A Multi-Line Comment
	*/
	
	/* Another Multi-Line Comment */

	printf(""%d"", x);
}".ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new TextEditorTextSpan[]
        {
            new(0, 8, (byte)GenericDecorationKind.PreprocessorDirective),
            new(20, 28, (byte)GenericDecorationKind.PreprocessorDirective),
        };
        
        var cLexer = new TextEditorCLexer();

        var textEditorTextSpans = 
            await cLexer.Lex(testDataHelloWorld);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)GenericDecorationKind.PreprocessorDirective)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
}