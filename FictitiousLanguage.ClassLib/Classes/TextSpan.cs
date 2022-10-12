namespace FictitiousLanguage.ClassLib.Classes;

public class TextSpan
{
    public TextSpan(int startingIndex, int endingIndex, string text)
    {
        StartingIndex = startingIndex;
        EndingIndex = endingIndex;
        Text = text;
    }

    public int StartingIndex { get; set; }
    public int EndingIndex { get; set; }
    public string Text { get; set; }
}