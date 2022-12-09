namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
	public static class Razor
	{
		public const string EXAMPLE_TEXT = @"<!-- TEST: Razor comment -->
@* A comment using razor syntax *@

<!-- TEST: Recognize razor keyword -->
@page ""/testRazorPage""

<!-- TEST: Cover the use of ${reserved words} as variable names -->
@{
	<!-- TEST: Arbitrary placement of codeblocks -->
	<!-- TEST: Cover the use of {Razor Keywords} as variable names -->
	{
		// TEST: Razor keyword as a local variable name.
		var page = 2;
		
		// TEST: Razor keyword as a substring within a local variable name.
		var pageIsSubstring = 2;
	}
	
	<!-- TEST: Cover the use of {C# Razor keywords} as variable names -->
	{
		// TEST: C# Razor keyword as a local variable name.
		var for = 2;
		
		// TEST: C# Razor keyword as a substring within a local variable name.
		var forIsSubstring = 2;
	}
}

<!-- TEST: Cover text output without rendering an HTML element -->
<div>
	<!-- TEST: Use a string literal to have text output without rendering an HTML element -->
	<div>
		@{
			// TEST: Single-line text output without rendering an HTML element syntax
			@: Single-line text output without rendering an HTML element syntax
		
			<div>
				This div serves to separate the single-line and 
				multi-line text output without rendering an HTML element syntaxtes
			</div>
		
			// TEST: Multi-line text output without rendering an HTML element syntax
			<text>
				Multi-line text output without rendering an HTML element syntax
			</text>
		}
	</div>
	<!-- TEST: Use an inline expression to have text output without rendering an HTML element -->
	<div>
		@{
			// TEST: Implicit inline expression used with the single-line text output without rendering an HTML element syntax
			@: @page @(page)
			// TEST: Explicit inline expression used with the single-line text output without rendering an HTML element syntax
			@: @for @(for)
		
			<div>
				This div serves to separate the single-line and 
				multi-line text output without rendering an HTML element syntaxtes
			</div>
		
			// TEST: Implicit inline expression used with the multi-line text output without rendering an HTML element syntax
			<text>
				@page&nbsp;
				@(page)
			</text>
			<text>
				@for&nbsp;
				@(for)
			</text>
		}
	</div>
</div>

<!-- TEST: Cover razor inline expression syntax -->
<div>
	@{
		// TEST: Two implicit inline expressions separated by an HTML entity
		@: @page&nbsp;@page
		
		// TEST: Two explicit inline expressions separated by an HTML entity
		@: @(page)&nbsp;@(page)
	}
</div>

<!-- TEST: Cover @code section -->
@code 
{
	// TEST: Property attribute decoration single
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
	
	// TEST: Property attribute decoration many
	[Parameter, EditorRequired]
    public Func<VirtualizationRequest, VirtualizationResult<T>?> EntriesProviderFunc { get; set; } = null!;

	// TEST: Constructor
	public FetchData()
	{
		LocalFunctionTest();
	
		// TEST: Local function
		void LocalFunctionTest()
		{
			Console.WriteLine(""This is a test local function"");
		}
	}

	// TEST: Cover fields
	private int _fieldWithValueTypeTest;
	private List<double> _fieldWithReferenceTypeTest;
	private string _fieldWithStringTypeTest;

	// TEST: Cover fields which are property encapsulated
	private int _fieldPropertyEncapsulatedWithValueTypeTest;
	private List<double> _fieldPropertyEncapsulatedWithReferenceTypeTest;
	private string _fieldPropertyEncapsulatedWithStringTypeTest;
	private string _fieldPropertyEncapsulatedBothExpressionsWithStringTypeTest;
	
	// TEST: Cover auto properties
	public int PropertyWithValueTypeTest { get; private set; }
	public List<double> PropertyWithReferenceTypeTest { get; private set; }
	public string PropertyWithStringTypeTest { get; private set; }
	
	// TEST: Region where the identifier contains a space character
	#region Region With Spaces
	
	public int RegionWithSpacesPropertyOne { get; set; }
	public int RegionWithSpacesPropertyTwo { get; set; }
	public int RegionWithSpacesPropertyThree { get; set; }
	
    #endregion
    
	// TEST: Region where the identifier does NOT contain a space character
    #region RegionNoSpaces
	public int RegionNoSpacesPropertyOne { get; set; }
	public int RegionNoSpacesPropertyTwo { get; set; }
	public int RegionNoSpacesPropertyThree { get; set; }
    #endregion
	
	// TEST: Cover properties with a backing field
	public int PropertyBackingFieldWithValueTypeTest 
	{ 
		get => _fieldPropertyEncapsulatedWithValueTypeTest;
		private set
		{
			_fieldPropertyEncapsulatedWithValueTypeTest = value;
			MethodTest();
		}
	}
	public List<double> PropertyBackingFieldWithReferenceTypeTest
	{ 
		get
		{
			MethodTest();
			return _fieldPropertyEncapsulatedWithReferenceTypeTest;
		}
		private set
		{
			MethodTest();
			_fieldPropertyEncapsulatedWithReferenceTypeTest = value;
		}
	}
	public string PropertyBackingFieldWithStringTypeTest
	{ 
		get
		{
			MethodTest();
			return _fieldPropertyEncapsulatedWithStringTypeTest;
		}
		private set => _fieldPropertyEncapsulatedWithStringTypeTest = value;
	}
	public string PropertyBackingFieldBothExpressionsWithStringTypeTest
	{ 
		get => _fieldPropertyEncapsulatedBothExpressionsWithStringTypeTest;
		private set => _fieldPropertyEncapsulatedBothExpressionsWithStringTypeTest = value;
	}
	
	// TEST: Cover expression bound properties
	public string PropertyExpressionBound => ""This is an expression bound property."";
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var boundaryIds = new List<object>();

            if (UseHorizontalVirtualization)
            {
                boundaryIds.AddRange(new[]
                {
                    LeftVirtualizationBoundaryDisplayId,
                    RightVirtualizationBoundaryDisplayId,
                });
            }

            if (UseVerticalVirtualization)
            {
                boundaryIds.AddRange(new[]
                {
                    TopVirtualizationBoundaryDisplayId,
                    BottomVirtualizationBoundaryDisplayId,
                });
            }

            await JsRuntime.InvokeVoidAsync(
                ""blazorTextEditor.initializeVirtualizationIntersectionObserver"",
                _intersectionObserverMapKey.ToString(),
                DotNetObjectReference.Create(this),
                _scrollableParentFinder,
                boundaryIds);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

	// TEST: Method attribute decoration
    [JSInvokable]
    public Task OnScrollEventAsync()
    {
        return Task.CompletedTask;
    }
	
	// TEST: Method
	private void MethodTest()
	{
		Console.WriteLine(""This is a test method"");
	}
}";
	}
}