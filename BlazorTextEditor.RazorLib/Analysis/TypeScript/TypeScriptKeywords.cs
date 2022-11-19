using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript;

/// <summary>
/// Found the list of keywords for TypeScript used in Monaco:
/// https://github.com/microsoft/monaco-editor/blob/main/src/basic-languages/typescript/typescript.ts
/// <br/><br/>
/// Monaco is making their list of keywords by looking at the TypeScript source code
/// see the:
/// https://github.com/microsoft/TypeScript/blob/master/src/compiler/scanner.ts
/// </summary>
public static class TypeScriptKeywords
{
    public const string AbstractKeyword = "abstract";
    public const string AnyKeyword = "any";
    public const string AsKeyword = "as";
    public const string AssertsKeyword = "asserts";
    public const string BigintKeyword = "bigint";
    public const string BooleanKeyword = "boolean";
    public const string BreakKeyword = "break";
    public const string CaseKeyword = "case";
    public const string CatchKeyword = "catch";
    public const string ClassKeyword = "class";
    public const string ContinueKeyword = "continue";
    public const string ConstKeyword = "const";
    public const string ConstructorKeyword = "constructor";
    public const string DebuggerKeyword = "debugger";
    public const string DeclareKeyword = "declare";
    public const string DefaultKeyword = "default";
    public const string DeleteKeyword = "delete";
    public const string DoKeyword = "do";
    public const string ElseKeyword = "else";
    public const string EnumKeyword = "enum";
    public const string ExportKeyword = "export";
    public const string ExtendsKeyword = "extends";
    public const string FalseKeyword = "false";
    public const string FinallyKeyword = "finally";
    public const string ForKeyword = "for";
    public const string FromKeyword = "from";
    public const string FunctionKeyword = "function";
    public const string GetKeyword = "get";
    public const string IfKeyword = "if";
    public const string ImplementsKeyword = "implements";
    public const string ImportKeyword = "import";
    public const string InKeyword = "in";
    public const string InferKeyword = "infer";
    public const string InstanceofKeyword = "instanceof";
    public const string InterfaceKeyword = "interface";
    public const string IsKeyword = "is";
    public const string KeyofKeyword = "keyof";
    public const string LetKeyword = "let";
    public const string ModuleKeyword = "module";
    public const string NamespaceKeyword = "namespace";
    public const string NeverKeyword = "never";
    public const string NewKeyword = "new";
    public const string NullKeyword = "null";
    public const string NumberKeyword = "number";
    public const string ObjectKeyword = "object";
    public const string OutKeyword = "out";
    public const string PackageKeyword = "package";
    public const string PrivateKeyword = "private";
    public const string ProtectedKeyword = "protected";
    public const string PublicKeyword = "public";
    public const string OverrideKeyword = "override";
    public const string ReadonlyKeyword = "readonly";
    public const string RequireKeyword = "require";
    public const string GlobalKeyword = "global";
    public const string ReturnKeyword = "return";
    public const string SetKeyword = "set";
    public const string StaticKeyword = "static";
    public const string StringKeyword = "string";
    public const string SuperKeyword = "super";
    public const string SwitchKeyword = "switch";
    public const string SymbolKeyword = "symbol";
    public const string ThisKeyword = "this";
    public const string ThrowKeyword = "throw";
    public const string TrueKeyword = "true";
    public const string TryKeyword = "try";
    public const string TypeKeyword = "type";
    public const string TypeofKeyword = "typeof";
    public const string UndefinedKeyword = "undefined";
    public const string UniqueKeyword = "unique";
    public const string UnknownKeyword = "unknown";
    public const string VarKeyword = "var";
    public const string VoidKeyword = "void";
    public const string WhileKeyword = "while";
    public const string WithKeyword = "with";
    public const string YieldKeyword = "yield";
    public const string AsyncKeyword = "async";
    public const string AwaitKeyword = "await";
    public const string OfKeyword = "of";
    
    public static readonly ImmutableArray<string> All = new[]
    {
        AbstractKeyword,
        AnyKeyword,
        AsKeyword,
        AssertsKeyword,
        BigintKeyword,
        BooleanKeyword,
        BreakKeyword,
        CaseKeyword,
        CatchKeyword,
        ClassKeyword,
        ContinueKeyword,
        ConstKeyword,
        ConstructorKeyword,
        DebuggerKeyword,
        DeclareKeyword,
        DefaultKeyword,
        DeleteKeyword,
        DoKeyword,
        ElseKeyword,
        EnumKeyword,
        ExportKeyword,
        ExtendsKeyword,
        FalseKeyword,
        FinallyKeyword,
        ForKeyword,
        FromKeyword,
        FunctionKeyword,
        GetKeyword,
        IfKeyword,
        ImplementsKeyword,
        ImportKeyword,
        InKeyword,
        InferKeyword,
        InstanceofKeyword,
        InterfaceKeyword,
        IsKeyword,
        KeyofKeyword,
        LetKeyword,
        ModuleKeyword,
        NamespaceKeyword,
        NeverKeyword,
        NewKeyword,
        NullKeyword,
        NumberKeyword,
        ObjectKeyword,
        OutKeyword,
        PackageKeyword,
        PrivateKeyword,
        ProtectedKeyword,
        PublicKeyword,
        OverrideKeyword,
        ReadonlyKeyword,
        RequireKeyword,
        GlobalKeyword,
        ReturnKeyword,
        SetKeyword,
        StaticKeyword,
        StringKeyword,
        SuperKeyword,
        SwitchKeyword,
        SymbolKeyword,
        ThisKeyword,
        ThrowKeyword,
        TrueKeyword,
        TryKeyword,
        TypeKeyword,
        TypeofKeyword,
        UndefinedKeyword,
        UniqueKeyword,
        UnknownKeyword,
        VarKeyword,
        VoidKeyword,
        WhileKeyword,
        WithKeyword,
        YieldKeyword,
        AsyncKeyword,
        AwaitKeyword,
        OfKeyword,
    }.ToImmutableArray();
}