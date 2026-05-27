using System.Drawing;

namespace Exp.Spans;

public class TextSpan(int location) : IDisposable
{
    public string text
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                if (!disposed)
                    TextChanged?.Invoke(this, value);
            }
        }
    } = "";

    public int location = location;
    public ScriptDocument Doc { get; set; }
    public Color color = Color.Black;
    public Color backColor = Color.Transparent;
    public SpanType type = SpanType.Normal;
    public bool insideFormattedString = false;
    public bool isKeyword = false;
    public string link = null;

    public bool isLink => link != null;

    //public SyntaxKind SyntaxKind { get; set; } = SyntaxKind.None;

    public event EventHandler<string> TextChanged;

    private bool disposed = false;

    public void Dispose()
    {
        text = "";
        color = Color.Empty;
        backColor = Color.Empty;
        disposed = true;
    }

    public TextSpan Duplicate() => new TextSpan(location) { text = text, color = color, backColor = backColor, type = type, insideFormattedString = insideFormattedString };

    public override string ToString() => text;

    public override bool Equals(object obj)
    {
        if (obj is TextSpan other)
        {
            return text == other.text && type == other.type && link == other.link;
        }
        return false;
    }
}
