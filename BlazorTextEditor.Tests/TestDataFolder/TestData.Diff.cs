using System.Collections.Immutable;

namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Diff
    {
        public static class Simple
        {
            public static class NoLineEndings
            {
                public const string SAMPLE_000 = "abcdefk";
                public const string SAMPLE_010 = "bhdefck";
            }

            public static class WithLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\n";
                public const string SAMPLE_010 = "bhdefck\n";
            }

            public static class WithCarriageReturnEnding
            {
                public const string SAMPLE_000 = "abcdefk\r";
                public const string SAMPLE_010 = "bhdefck\r";
            }

            public static class WithCarriageReturnLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\r\n";
                public const string SAMPLE_010 = "bhdefck\r\n";
            }

            public static IEnumerable<string> GetAllInputSimple => new[]
            {
                NoLineEndings.SAMPLE_000,
                NoLineEndings.SAMPLE_010,
                WithLinefeedEnding.SAMPLE_000,
                WithLinefeedEnding.SAMPLE_010,
                WithCarriageReturnEnding.SAMPLE_000,
                WithCarriageReturnEnding.SAMPLE_010,
                WithCarriageReturnLinefeedEnding.SAMPLE_000,
                WithCarriageReturnLinefeedEnding.SAMPLE_010,
            }.ToImmutableArray();
        }
        
        public static class MultiLine
        {
            public static class WithLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\nzabc";
                public const string SAMPLE_010 = "bhdefck\nzabc";
            }

            public static class WithCarriageReturnEnding
            {
                public const string SAMPLE_000 = "abcdefk\rzabc";
                public const string SAMPLE_010 = "bhdefck\rzabc";
            }

            public static class WithCarriageReturnLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\r\nzabc";
                public const string SAMPLE_010 = "bhdefck\r\nzabc";
            }

            public static IEnumerable<string> GetAllInputMultiLine => new[]
            {
                WithLinefeedEnding.SAMPLE_000,
                WithLinefeedEnding.SAMPLE_010,
                WithCarriageReturnEnding.SAMPLE_000,
                WithCarriageReturnEnding.SAMPLE_010,
                WithCarriageReturnLinefeedEnding.SAMPLE_000,
                WithCarriageReturnLinefeedEnding.SAMPLE_010,
            }.ToImmutableArray();
        }
    }
}