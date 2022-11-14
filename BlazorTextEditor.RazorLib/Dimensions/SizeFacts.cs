namespace BlazorTextEditor.RazorLib.Dimensions;

public static class SizeFacts
{
    public static class bte
    {
        public static class Header
        {
            public static readonly DimensionUnit Height = new()
            {
                Value = 3,
                DimensionUnitKind = DimensionUnitKind.RootCharacterHeight
            };
        }
    }
}