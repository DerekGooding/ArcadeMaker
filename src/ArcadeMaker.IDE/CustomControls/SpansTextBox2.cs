using System.ComponentModel;
using System.Diagnostics;

namespace ArcadeMaker.IDE;

public partial class SpansTextBox2 : UserControl
{
    private readonly List<ScriptBoxSpan> OldSpans = [];
    public readonly List<ScriptBoxSpan> Spans = [];
    public readonly List<char> CharAlerts = [];

    public float LineSpacing => Font.Size * Font.FontFamily.GetLineSpacing(FontStyle.Regular) / Font.FontFamily.GetEmHeight(FontStyle.Regular) + 2;

    public string Text
    {
        get
        {
            var text = "";
            for (var span = 0; span < Spans.Count; span++)
                text += Spans[span].text;
            return text;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Loading
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                Invalidate();
            }
        }
    } = false;

    public string TextRange(int endIndex) => TextRange(0, endIndex);

    public string TextRange(int startIndex, int endIndex)
    {
        var text = "";
        for (int span = 0, totalCharIndex = 0; span < Spans.Count; span++)
        {
            for (var c = 0; c < Spans[span].text.Length; c++, totalCharIndex++)
            {
                if (startIndex <= totalCharIndex)
                {
                    text += Spans[span].text[c];
                    if (totalCharIndex == endIndex)
                        return text;
                }
            }
        }
        return text;
    }

    private string GetTextStats(out int lineCount)
    {
        var text = "";
        lineCount = 0;
        foreach (var span in Spans)
        {
            foreach (var c in span.text)
            {
                if (c == '\n')
                    lineCount++;
                text += c;
            }
        }
        return text;
    }

    public int Length
    {
        get
        {
            var length = 0;
            foreach (var span in Spans)
                length += span.text.Length;
            return length;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Font Font
    { get; set { field = value; knownCharWidthes_chars.Clear(); knownCharWidthes_widthes.Clear(); Invalidate(); } } = new Font("Consolas", 9.5F);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float ScrollX
    { get; set { field = value; Invalidate(); } }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float ScrollY
    { get; set { field = value; Invalidate(); } }

    public event EventHandler<int> SelectionStartChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectionStart
    { get; set { field = value; SelectionStartChanged?.Invoke(this, value); Invalidate(); } } = 0;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectionLength
    { get; set { field = value; Invalidate(); } } = 0;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color SelectionColor
    { get; set { field = value; Invalidate(); } } = Color.White;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color SelectionBackColor
    { get; set { field = value; Invalidate(); } } = Color.Violet;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Brush CurrentLineHighlightBrush
    { get; set { field = value; Invalidate(); } } = new Pen(Color.FromArgb(60, 60, 60)).Brush;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color LineNumbersColor
    { get; set { field = value; Invalidate(); } } = Color.IndianRed;

    public new event EventHandler<SpansTextBox2TextChangedEventArgs> TextChanged;

    public event EventHandler<SpansTextBox2CharAlertEventArgs> CharAlert;

    private System.Windows.Forms.Timer CaretTimer = new() { Interval = 700 };

    public static readonly string TabSpace = "    ";

    private readonly ToolStripMenuItem copyMenuBtn = new("Copy");
    private readonly ToolStripMenuItem cutMenuBtn = new("Cut");
    private readonly ToolStripMenuItem pasteMenuBtn = new("Paste");

    public SpansTextBox2()
    {
        InitializeComponent();
        completionBox.SelectedValueChanged += (s, e) => ShowCompletionItemInfo();
        textStartLocX += vScrollBar.Width;
        drawTextFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
        CaretTimer.Tick += CaretTimer_Tick;

        completionBox.VisibleChanged += (s, e) =>
        {
            if (!completionBox.Visible)
            {
                toolTipText = null;
                completionToolTip.Hide(this);
            }
        };

        ContextMenuStrip = new();
        copyMenuBtn.Click += (s, e) => Copy();
        cutMenuBtn.Click += (s, e) => Cut();
        pasteMenuBtn.Click += (s, e) => Paste();
        copyMenuBtn.ShortcutKeys = Keys.Control | Keys.C;
        cutMenuBtn.ShortcutKeys = Keys.Control | Keys.X;
        pasteMenuBtn.ShortcutKeys = Keys.Control | Keys.V;
        ContextMenuStrip.Items.AddRange([copyMenuBtn, cutMenuBtn, pasteMenuBtn]);
        ContextMenuStrip.Opened += ContextMenu_Popup;
    }

    private void ContextMenu_Popup(object sender, EventArgs e)
    {
        bool canCopy = SelectionLength > 0, canPaste = Clipboard.ContainsText();
        copyMenuBtn.Enabled = canCopy;
        cutMenuBtn.Enabled = canCopy;
        pasteMenuBtn.Enabled = canPaste;
    }

    private bool skipNextCaretTimerTick = false;

    private void CaretTimer_Tick(object sender, EventArgs e)
    {
        if (skipNextCaretTimerTick)
        {
            skipNextCaretTimerTick = false;
            return;
        }

        invalidatedByTimer = true;

        drawCaret = !drawCaret;
        Invalidate();

        invalidatedByTimer = false;
    }

    private StringFormat drawTextFormat = StringFormat.GenericTypographic;
    private const float textMinStartLocX = 15F, textMinStartLocY = 10F;
    private float textStartLocX = textMinStartLocX, textStartLocY = textMinStartLocY;
    private bool drawCaret = false;

    private new float HScroll(SizeF textSize) => hScrollBar.Maximum > 0 ? Global.MapGPT(hScrollBar.Value, hScrollBar.Minimum, hScrollBar.Maximum, 0, (int)textSize.Width) : 0;

    private new float VScroll(SizeF textSize) => vScrollBar.Maximum > 0 ? Global.MapGPT(vScrollBar.Value, vScrollBar.Minimum, vScrollBar.Maximum, 0, (int)textSize.Height) : 0;

    private new float VScroll(int lineCount = -1)
    {
        if (lineCount < 0)
            lineCount = Text.CountOf('\n') + 1;
        return vScrollBar.Maximum > 0 ? Global.MapGPT(vScrollBar.Value, vScrollBar.Minimum, vScrollBar.Maximum, 0, (int)(lineCount * LineSpacing + textStartLocY)) : 0;
    }

    private SizeF? lastTextSize = null;
    private bool invalidatedByTimer = false;

    /// <summary>
    /// The number of lines in the last drawn text (excluding first line)
    /// </summary>
    private int displayTextLineCount = -1;

    private string displayedText = null;
    private float[] displayCharWidthes = null;

    private List<char> knownCharWidthes_chars = [];
    private List<float> knownCharWidthes_widthes = [];

    private void SpanTextBox2_Paint(object sender, PaintEventArgs e)
    {
        var Text = GetTextStats(out var lineCount);
        SizeF textSize;
        if (lastTextSize == null || !lastTextSize.HasValue || displayedText != Text)
        {
            textSize = MeasureString(Text, e.Graphics);
            lastTextSize = textSize;
            displayedText = Text;
        }
        else
            textSize = lastTextSize.Value;
        PointF? caretLocation = null;
        var canScrollH = hScrollBar.Maximum > 0;
        var canScrollV = vScrollBar.Maximum > 0;
        var Length = this.Length;
        float hscroll = 0, vscroll = 0;
        displayTextLineCount = lineCount;
        if (canScrollH || canScrollV)
        {
            hscroll = HScroll(textSize);
            vscroll = VScroll(lineCount);
            e.Graphics.TranslateTransform(-hscroll, -vscroll);
        }
        using (Pen pen = new Pen(LineNumbersColor))
        {
            float x = textStartLocX, y = textStartLocY;

            // number lines (calculation part)
            var lineNumberWidth = MeasureString((lineCount + 1).ToString(), e.Graphics).Width;
            textStartLocX = vScrollBar.Width + textMinStartLocX + lineNumberWidth;

            var totalCharIndex = 0;
            char? previousChar = null;

            var Spans = this.Spans;

            // if text is empty, simulate single line text to draw caret & line highlight
            var foundText = false;
            foreach (var span in Spans)
            {
                if (span.text.Length > 0)
                {
                    foundText = true;
                    break;
                }
            }
            if (!foundText)
            {
                Spans.Clear();
                Spans =
                [
                    new ScriptBoxSpan { text = "\n" }
                ];
                Length = 1; // we're talking about the local parameter 'Length', not the property << int Length { get; } >>
            }

            displayCharWidthes = new float[Text.Length];
            bool outsideView_up = y + LineSpacing < vscroll, outsideView_down = y + LineSpacing > vscroll + DisplayRectangle.Height;

            for (var spanIndex = 0; spanIndex < Spans.Count; spanIndex++)
            {
                var span = Spans[spanIndex];
                for (var charIndex = 0; charIndex < span.text.Length; charIndex++, totalCharIndex++)
                {
                    var c = span.text[charIndex];

                    outsideView_up = false;
                    outsideView_down = false;
                    var outsideView = false;

                    void CalculateOutsideView()
                    {
                        outsideView_up = y + LineSpacing < vscroll;
                        outsideView_down = y + LineSpacing > vscroll + DisplayRectangle.Height;
                        outsideView = outsideView_up || outsideView_down;
                    }

                    CalculateOutsideView();

                    var highlightLineCheck = totalCharIndex == 0;
                    var currentCharIsNewLine = false;
                    var drawCaretAfterLineHighlight = false;
                    if (c == '\n')
                    {
                        x = textStartLocX;

                        // if previous char was endline too, then we need to draw here the caret
                        var lastCharInText = Length - 1 == totalCharIndex;
                        if (!outsideView && drawCaret)
                        {
                            if (totalCharIndex == SelectionStart && previousChar == '\n')
                            {
                                pen.Color = Color.Black;
                                e.Graphics.DrawLine(pen, x, y + 2, x, y + LineSpacing + 2);
                            }
                            else if ((lastCharInText && totalCharIndex == SelectionStart - 1) || (SelectionStart == totalCharIndex && SelectionStart == 0))
                            {
                                // if this is the last char in the Text, draw caret at next line
                                drawCaretAfterLineHighlight = true;
                            }
                        }

                        y += LineSpacing;
                        CalculateOutsideView();
                        highlightLineCheck = true;
                        currentCharIsNewLine = true;
                    }
                    if (!outsideView && Focused && highlightLineCheck)
                    {
                        // check if caret is in this line
                        if (totalCharIndex < SelectionStart || (totalCharIndex == SelectionStart && SelectionStart == 0))
                        {
                            var caretAtCurrentLine = true;
                            for (int sc = charIndex + 1, lc = totalCharIndex + 1, lspan = spanIndex; lc < SelectionStart; lc++, sc++)
                            {
                                while (sc >= Spans[lspan].text.Length)
                                {
                                    sc = 0;
                                    if (++lspan >= Spans.Count)
                                    {
                                        goto HighlightLine;
                                    }
                                }
                                if (Spans[lspan].text[sc] == '\n')
                                {
                                    caretAtCurrentLine = false;
                                    break;
                                }
                            }

                            // highlight current line
                        HighlightLine:
                            if (caretAtCurrentLine)
                            {
                                var highlightY = (SelectionStart == 0 ? textStartLocY : y) + 2;
                                e.Graphics.FillRectangle(CurrentLineHighlightBrush, textStartLocX, highlightY, textSize.Width + DisplayRectangle.Width, LineSpacing);
                            }
                        }
                    }

                    // [AFTER LINE HIGHLIGHT] if there were 2 endl in a row, draw selection (if selected)
                    if (!outsideView && SelectionLength > 0 && c == '\n' && (previousChar == '\n' || totalCharIndex == Length - 1 || totalCharIndex == 0) && SelectionStart <= totalCharIndex && SelectionLength + SelectionStart >= totalCharIndex)
                    {
                        pen.Color = SelectionBackColor;
                        float drawAtY = y - LineSpacing + 2, drawHeight = LineSpacing;
                        if (totalCharIndex == Length - 1)
                        {
                            if (previousChar != '\n')
                                drawAtY += LineSpacing;
                            else
                                drawHeight *= 2;
                        }
                        e.Graphics.FillRectangle(pen.Brush, x, drawAtY, 4, drawHeight);
                    }

                    if (drawCaretAfterLineHighlight)
                    {
                        pen.Color = Color.Black;
                        var caretY = y + 2;
                        if (totalCharIndex == 0)
                            caretY -= LineSpacing;
                        caretLocation = new PointF(x, caretY);
                        e.Graphics.DrawLine(pen, caretLocation.Value.X, caretLocation.Value.Y, caretLocation.Value.X, caretLocation.Value.Y + LineSpacing);
                    }
                    if (!currentCharIsNewLine)
                    {
                        if (!outsideView)
                        {
                            float charWidth;
                            var knownCharsWidthes_index = knownCharWidthes_chars.IndexOf(c);
                            if (knownCharsWidthes_index < 0)
                            {
                                var charSize = MeasureString(c, e.Graphics);
                                charWidth = charSize.Width;
                                knownCharWidthes_chars.Add(c);
                                knownCharWidthes_widthes.Add(charWidth);
                            }
                            else
                            {
                                charWidth = knownCharWidthes_widthes[knownCharsWidthes_index];
                            }
                            displayCharWidthes[totalCharIndex] = charWidth;

                            // draw selection
                            if (totalCharIndex >= SelectionStart && totalCharIndex < SelectionStart + SelectionLength)
                            {
                                pen.Color = SelectionBackColor;
                                e.Graphics.FillRectangle(pen.Brush, x, y + 2, charWidth, LineSpacing);
                            }
                            pen.Color = span.color;
                            if (Loading)
                                pen.Color = Color.FromArgb(pen.Color.A / 2, pen.Color.R, pen.Color.G, pen.Color.B);

                            if (span.isLink)
                                e.Graphics.DrawLine(pen, x, y + LineSpacing, x + charWidth, y + LineSpacing);
                            e.Graphics.DrawString(c.ToString(), Font, pen.Brush, x, y, drawTextFormat);

                            var caretInFirstChar = SelectionStart == 0;

                            if (!caretInFirstChar)
                                x += charWidth;

                            // draw caret
                            pen.Color = Color.Black;
                            float caretX = x, caretY = y + 2;
                            var drawCaretNow = false;
                            if (totalCharIndex == SelectionStart - (caretInFirstChar ? 0 : 1))
                            {
                                drawCaretNow = true;
                            }
                            else if (previousChar == '\n' && totalCharIndex == SelectionStart)
                            {
                                caretX -= charWidth;
                                drawCaretNow = true;
                            }
                            else if (totalCharIndex == Length - 1 && SelectionStart == totalCharIndex + 1)
                            {
                                caretX = textStartLocX;
                                caretY += LineSpacing;
                                drawCaretNow = true;
                            }
                            caretLocation = new PointF(caretX, caretY);
                            if (drawCaret && drawCaretNow)
                            {
                                e.Graphics.DrawLine(pen, caretX, caretY, caretX, caretY + LineSpacing);
                            }

                            if (caretInFirstChar)
                                x += charWidth;
                        }
                    }
                    previousChar = c;
                }
            }

            // set scroll bars
            int vert = (int)(y + LineSpacing + textStartLocY), hor = (int)(textSize.Width + textStartLocX);
            if (!invalidatedByTimer)
            {
                if (vert > DisplayRectangle.Height)
                {
                    vScrollBar.Height = DisplayRectangle.Height;
                    var oldMaximum = vScrollBar.Maximum;
                    if (oldMaximum != vert - DisplayRectangle.Height)
                    {
                        vScrollBar.Maximum = vert - DisplayRectangle.Height;
                    }
                    vScrollBar.Visible = true;
                }
                else
                {
                    vScrollBar.Visible = false;
                    vScrollBar.Maximum = 0;
                }
                if (hor > DisplayRectangle.Width)
                {
                    hScrollBar.Width = DisplayRectangle.Width - hScrollBar.Location.X;
                    var oldMaximum = hScrollBar.Maximum;
                    if (oldMaximum != hor - DisplayRectangle.Width)
                    {
                        hScrollBar.Maximum = hor - DisplayRectangle.Width;
                    }
                    hScrollBar.Visible = true;
                }
                else
                {
                    hScrollBar.Visible = false;
                    hScrollBar.Maximum = 0;
                }

                // move scrolling to fit caret
                if (false && invalidatedByKeyPress && caretLocation.HasValue && caretLocation.Value != null)
                {
                    invalidatedByKeyPress = false;
                    var scrolled = false;
                    var v_fromHigh = (int)(lineCount * LineSpacing + textStartLocY);
                    if (caretLocation.Value.Y < vscroll)
                    {
                        //vScrollBar.Value = Global.MapGPT((int)caretLocation.Value.Y, 0, v_fromHigh, vScrollBar.Minimum, vScrollBar.Maximum);
                        scrolled = true;
                    }
                    else if (caretLocation.Value.Y > vscroll + DisplayRectangle.Height /*- (hScrollBar.Visible ? hScrollBar.Height : 0)*/)
                    {
                        //vScrollBar.Value = Global.MapGPT((int)caretLocation.Value.Y - DisplayRectangle.Height, 0, v_fromHigh, vScrollBar.Minimum, vScrollBar.Maximum);
                        scrolled = true;
                    }
                    if (caretLocation.Value.X < hscroll)
                    {
                        //hScrollBar.Value = Global.MapGPT((int)hscroll, 0, hor + (int)textStartLocX + DisplayRectangle.Width, hScrollBar.Minimum, hScrollBar.Maximum);
                        scrolled = true;
                    }
                    else if (caretLocation.Value.X > hscroll + DisplayRectangle.Width)
                    {
                        //hScrollBar.Value = Global.MapGPT((int)hscroll + DisplayRectangle.Width, 0, hor + (int)textStartLocX + DisplayRectangle.Width, hScrollBar.Minimum, hScrollBar.Maximum);
                        scrolled = true;
                    }

                    if (scrolled)
                    {
                        e.Graphics.Clear(BackColor);
                        SpanTextBox2_Paint(sender, e);
                        return;
                    }
                }
            }

            // number lines (drawing part)
            pen.Color = LineNumbersColor;
            var lineNumbersX = (vScrollBar.Visible ? vScrollBar.Width : 0) + hscroll;
            e.Graphics.FillRectangle(Brushes.DarkSlateGray, lineNumbersX, vscroll/*0*/, textStartLocX - (vScrollBar.Visible ? vScrollBar.Width : 0), DisplayRectangle.Height/*vert + DisplayRectangle.Height * 2*/);
            for (var line = (int)(vscroll / LineSpacing) + 1; line * LineSpacing <= vscroll + DisplayRectangle.Height && line <= lineCount + 1; line++)
            {
                e.Graphics.DrawString(line.ToString(), Font, pen.Brush, lineNumbersX + 1, (line - 1) * LineSpacing + textStartLocY + 2);
            }

            // loading message
            if (Loading)
            {
                using (Pen lpen = new Pen(Color.FromArgb(200, Color.Black)))
                {
                    e.Graphics.ResetTransform();
                    const string text = "Loading...";
                    textSize = MeasureString(text, e.Graphics);
                    float rwidth = textSize.Width + 50, rheight = textSize.Height + 50;
                    e.Graphics.FillRectangle(pen.Brush, Width / 2 - rwidth / 2, Height / 2 - rheight / 2, rwidth, rheight);
                    /* using (*/
                    var textBrush = Brushes.White;
                    e.Graphics.DrawString(text, Font, textBrush, Width / 2 - textSize.Width / 2, Height / 2 - textSize.Height / 2);
                }
            }
        }
    }

    private int mouseDownCharInd = 0;

    private async void SpanTextBox2_MouseDown(object sender, MouseEventArgs e)
    {
        if (completionBox.Visible)
            HideSuggestions();

        if (e.Button != MouseButtons.Left)
            return;

        SelectionStart = GetCharIndexByPosition(e.Location);
        SelectionLength = 0;

        mouseDownCharInd = SelectionStart;

        // if clicked on a link, open it
        if (ModifierKeys == Keys.Control)
        {
            await Task.Run(() =>
            {
                var spanIndex = 0;
                var @break = false;
                for (var totalCharIndex = 0; spanIndex < Spans.Count; spanIndex++)
                {
                    for (var charIndex = 0; charIndex < Spans[spanIndex].text.Length; charIndex++, totalCharIndex++)
                    {
                        if (totalCharIndex == SelectionStart)
                        {
                            @break = true;
                            break;
                        }
                    }
                    if (@break) break;
                }

                if (Spans[spanIndex].isLink)
                {
                    try
                    {
                        Process.Start(Spans[spanIndex].link);
                    }
                    catch (System.Exception ex)
                    {
                        var err = "Could not open link in browser.";
#if DEBUG
                        err += "\n\n[Debug Mode]\n" + ex;
#endif
                        MessageBox.Show(err, "Error");
                    }
                }
            });
        }
    }

    public int GetCharIndexByPosition(PointF position, bool ignoreScrolling = false)
    {
        if (position.Y < textStartLocY)
            return 0;

        var Text = displayedText ?? this.Text;

        SizeF textSize;
        if (lastTextSize.HasValue && lastTextSize.Value != null)
            textSize = lastTextSize.Value;
        else
            textSize = MeasureString(Text);
        if (!ignoreScrolling)
        {
            position.X += (int)HScroll(textSize);
            position.Y += (int)VScroll(displayTextLineCount);
        }
        float x = textStartLocX, y = textStartLocY;

        for (var index = 0; index < Text.Length; index++)
        {
            var endl = Text[index] == '\n';
            var charWidth = displayCharWidthes == null ? MeasureString(Text[index]).Width : displayCharWidthes[index];

            if (!endl && position.Y >= y && position.Y <= y + LineSpacing)
            {
                if (position.X <= x + charWidth || (index == Text.Length - 1 || (index < Text.Length - 1 && Text[index + 1] == '\n')))
                {
                    if (position.X >= x + charWidth / 2)
                    {
                        return index + 1;
                    }
                    else if (position.X >= x)
                    {
                        return index;
                    }
                }
            }

            if (endl)
            {
                x = textStartLocX;
                if (index > 0) // else - after empty line check
                    y += LineSpacing;

                // check empty line
                if ((index == 0 || (index < Text.Length - 1 && Text[index + 1] == '\n')) && position.Y >= y && position.Y <= y + LineSpacing + (index == 0 ? LineSpacing : 0))
                {
                    if (index > 0)
                        return index + 1;
                    else
                    {
                        // the second empty line would not have a check, because the first one is checking for itself,
                        // not for the next, so we need to sepcificly check it
                        if (position.Y > y + LineSpacing)
                        {
                            if (Text[index + 1] == '\n')
                                return index + 1;
                        }
                        else
                            return index;
                    }
                }

                if (index == 0)
                    y += LineSpacing;
            }
            else
                x += charWidth;
        }

        return Text.Length;
    }

    public PointF GetPositionOfCharIndex(int charIndex, bool ignoreScrolling = false)
    {
        var Text = displayedText ?? this.Text;
        float x = 0, y = 0;
        SizeF textSize = new SizeF(0, 0);
        var lineCount = 1;
        for (var c = 0; c < charIndex; c++)
        {
            if (c < Text.Length) // prevent async work bugs
            {
                if (Text[c] == '\n')
                {
                    if (x > textSize.Width)
                        textSize.Width = x;

                    x = 0;
                    y += LineSpacing;

                    textSize.Height = y;
                    lineCount++;
                }
                else
                {
                    x += displayCharWidthes == null ? MeasureString(Text[c]).Width : displayCharWidthes[c];
                }
            }
        }

        if (ignoreScrolling)
        {
            x -= HScroll(lastTextSize.Value);
            y -= VScroll();
        }

        return new PointF(x + textStartLocX, y + textStartLocY);
    }

    private const char KeyBackspace = (char)8;
    private const char KeyEnter = (char)13;
    private const char KeyEscape = (char)27;
    private bool interceptEnterKeyPress = false;

    private void SpanTextBox2_KeyPress(object sender, KeyPressEventArgs e)
    {
        invalidatedByKeyPress = true;

        if (e.KeyChar == KeyBackspace)
        {
            if (SelectionLength == 0)
                RemoveChar();
            else
                InsertText();
        }
        else if (e.KeyChar == KeyEnter)
        {
            if (!interceptEnterKeyPress)
            {
                //// insert new line, with correct tabbing space
                var Text = this.Text;

                // get depth
                var caretLoc = SelectionStart;
                var depth = 0;
                for (var i = 0; i < caretLoc; i++)
                {
                    var c = Text[i];
                    if (c == '{')
                        depth++;
                    else if (c == '}')
                        depth--;
                }

                // insert <depth> tabs
                var tabs = "";
                for (var t = 0; t < depth; t++)
                    tabs += TabSpace;

                InsertText("\n" + tabs);
            }
            else
            {
                interceptEnterKeyPress = false;
            }
        }
        else if (e.KeyChar == '\t')
        {
            var selectionStart = SelectionStart;
            var text = Text;

            // check if selection start is the first character (excluding spaces) in the line
            var firstCharInLine = true;
            for (var i = selectionStart - 1; i >= 0 && text[i] != '\n'; i--)
            {
                if (text[i] != ' ')
                {
                    firstCharInLine = false;
                    break;
                }
            }

            // if it is, insert TAB at each line in the selected area
            if (firstCharInLine && SelectionLength > 0)
            {
                var newLine = true;
                var tabsCount = 0;
                for (var i = selectionStart; i <= selectionStart + SelectionLength && i < text.Length; i++)
                {
                    if (text[i] == '\n')
                        newLine = true;
                    else if (newLine && text[i] != ' ')
                    {
                        newLine = false;
                        SelectionStart = i + tabsCount * TabSpace.Length;
                        InsertText(TabSpace, removeSelected: false, setSpans: false, invalidate: false);
                        tabsCount++;
                    }
                }
                SelectionStart = selectionStart;
                SelectionLength += tabsCount * TabSpace.Length;
                SetSpans();
                TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(TextChangedKind.Replace, false));
                Invalidate();
            }
            else
                InsertText(TabSpace);
        }
        else if (e.KeyChar != KeyEscape)
            InsertText(e.KeyChar.ToString());
    }

    private int shiftKeyPressCharIndex = 0;
    private bool invalidatedByKeyPress = false;

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Control)
        {
            SetLinkCursorAsync();
        }
        else if (keyData == (Keys.Control | Keys.C))
        {
            Copy(); return true;
        }
        else if (keyData == (Keys.Control | Keys.X))
        {
            Cut(); return true;
        }
        else if (keyData == (Keys.Control | Keys.V))
        {
            Paste(); return true;
        }
        else if (keyData == (Keys.Control | Keys.A))
        {
            SelectionStart = 0;
            SelectionLength = Text.Length;

            return true;
        }
        else if (keyData == Keys.Right)
        {
            // caret right
            if (SelectionStart < Text.Length)
                SelectionStart++;
            return true;
        }
        else if (keyData == Keys.Left)
        {
            // caret left
            if (SelectionStart > 0)
                SelectionStart--;
            return true;
        }
        else if (keyData == Keys.Up)
        {
            if (!completionBox.Visible)
            {
                var loc = GetPositionOfCharIndex(SelectionStart);
                loc.Y -= (LineSpacing - 1);
                SelectionStart = GetCharIndexByPosition(loc, true);
            }
            return true;
        }
        else if (keyData == Keys.Down)
        {
            if (!completionBox.Visible)
            {
                var loc = GetPositionOfCharIndex(SelectionStart);
                loc.Y += LineSpacing + 1;
                SelectionStart = GetCharIndexByPosition(loc, true);
            }
            return true;
        }
        else if (keyData == (Keys.Shift | Keys.Right))
        {
            if (SelectionLength == 0)
                shiftKeyPressCharIndex = SelectionStart;

            // select right
            if (SelectionStart >= shiftKeyPressCharIndex)
            {
                SelectionLength++;
            }
            else
            {
                SelectionStart++;
                SelectionLength--;
            }
            return true;
        }
        else if (keyData == (Keys.Shift | Keys.Left))
        {
            if (SelectionLength == 0)
                shiftKeyPressCharIndex = SelectionStart;

            // select left
            if (SelectionStart + SelectionLength > shiftKeyPressCharIndex)
            {
                SelectionLength--;
            }
            else
            {
                SelectionStart--;
                SelectionLength++;
            }
            return true;
        }
        else if (keyData == (Keys.Shift | Keys.Up))
        {
            if (SelectionLength == 0)
                shiftKeyPressCharIndex = SelectionStart;

            // select up
            mouseDownCharInd = SelectionStart + SelectionLength;

            var selectionStartAndEndAreInSameLine = true;
            var text = Text;
            for (var i = SelectionStart; i < SelectionStart + SelectionLength; i++)
            {
                if (text[i] == '\n')
                {
                    selectionStartAndEndAreInSameLine = false;
                    break;
                }
            }

            // implement this operation by simulating selection with mouse move
            var loc = GetPositionOfCharIndex(SelectionStart + (selectionStartAndEndAreInSameLine ? SelectionLength : 0));
            SpansTextBox2_MouseMove(this, new MouseEventArgs(MouseButtons.Left, 1, (int)loc.X, (int)(loc.Y - (LineSpacing - 1)), 0));

            return true;
        }
        else if (keyData == (Keys.Shift | Keys.Down))
        {
            if (SelectionLength == 0)
                shiftKeyPressCharIndex = SelectionStart;

            // select down
            mouseDownCharInd = SelectionStart + SelectionLength;

            var selectionStartAndEndAreInSameLine = true;
            var text = Text;
            for (var i = SelectionStart; i < SelectionStart + SelectionLength; i++)
            {
                if (text[i] == '\n')
                {
                    selectionStartAndEndAreInSameLine = false;
                    break;
                }
            }

            // implement this operation by simulating selection with mouse move
            var loc = GetPositionOfCharIndex(SelectionStart + (selectionStartAndEndAreInSameLine ? SelectionLength : 0));
            SpansTextBox2_MouseMove(this, new MouseEventArgs(MouseButtons.Left, 1, (int)loc.X, (int)(loc.Y + (LineSpacing + 1)), 0));

            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    public void Copy()
    {
        var text = Text.Substring(SelectionStart, SelectionLength);
        Clipboard.SetText(text);
    }

    public void Paste()
    {
        if (Clipboard.ContainsText())
            InsertText(Clipboard.GetText());
    }

    public void Cut()
    {
        Copy();
        InsertText(); // remove selected text
    }

    private void MakeCharAlert(string text)
    {
        if (CharAlerts.Count > 0 && CharAlert != null && SelectionStart > 0)
        {
            if (text == null)
                text = Text;
            if (SelectionStart <= text.Length && CharAlerts.Contains(text[SelectionStart - 1]))
            {
                // get span before
                int spanIndex, totalCharIndex;
                ScriptBoxSpan span = null;
                for (spanIndex = 0, totalCharIndex = 0; spanIndex < Spans.Count; spanIndex++)
                {
                    for (var charIndex = 0; charIndex < Spans[spanIndex].text.Length; charIndex++, totalCharIndex++)
                    {
                        if (totalCharIndex == SelectionStart - 2)
                        {
                            span = Spans[spanIndex];
                            goto BreakPoint;
                        }
                    }
                }

            BreakPoint:
                CharAlert?.Invoke(this, new SpansTextBox2CharAlertEventArgs(text[SelectionStart - 1], SelectionStart - 1, span));
            }
        }
    }

    private void InsertText(string text = "", bool removeSelected = true, bool setSpans = true, bool invalidate = true)
    {
        if (SelectionStart == Length)
        {
            string Text = null;
            if (Spans.Any())
            {
                Spans.Last().text += text;
                if (setSpans)
                    SetSpans();
            }
            else
            {
                // text is empty
                if (setSpans)
                    Spans.AddRange(GetScriptBoxSpans(text));
                else
                    Spans.Add(new ScriptBoxSpan { text = text });
                Text = text;
            }

            SelectionStart += text.Length;
            if (invalidate)
            {
                TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(TextChangedKind.Insert, false));
                MakeCharAlert(Text);
                Invalidate();
            }
            return;
        }
        var spanIndex = 0;
        var totalCharIndex = 0;
        foreach (var span in Spans)
        {
            for (var c = 0; c < span.text.Length; c++, totalCharIndex++)
            {
                if (totalCharIndex == SelectionStart)
                {
                    // first, remove selected text
                    var removedSelection = SelectionLength > 0;
                    if (removeSelected)
                    {
                        for (int removeSpanCharInd = c, removeSpanInd = spanIndex, removeInd = 0; removeInd < SelectionLength; removeInd++)
                        {
                            Spans[removeSpanInd].text = Spans[removeSpanInd].text.Remove(removeSpanCharInd, 1);
                            if (removeSpanCharInd >= Spans[removeSpanInd].text.Length)
                            {
                                if (Spans[removeSpanInd].text.Length == 0 && Spans[removeSpanInd] != span)
                                {
                                    Spans.RemoveAt(removeSpanInd);
                                }
                                else
                                {
                                    removeSpanInd++;
                                }
                                removeSpanCharInd = 0;
                            }
                        }

                        SelectionLength = 0;
                    }

                    // now we can add the new char
                    span.text = span.text.Insert(c, text);

                    SelectionStart += text.Length;

                    string Text = null;
                    if (setSpans)
                        Text = SetSpans();

                    if (invalidate)
                    {
                        TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(removedSelection ? TextChangedKind.Replace : TextChangedKind.Insert, false));
                        MakeCharAlert(Text);
                        Invalidate();
                    }
                    return;
                }
            }
            spanIndex++;
        }
    }

    private void RemoveChar()
    {
        if (SelectionStart == 0)
            return;
        if (SelectionStart == Length && Spans.Any())
        {
            while (Spans.Last().text.Length == 0)
            {
                Spans.RemoveAt(Spans.Count - 1);
                if (!Spans.Any())
                    return;
            }

            var span = Spans.Last();
            span.text = span.text.Remove(Spans.Last().text.Length - 1);

            while (span.text.Length == 0)
            {
                Spans.Remove(span);
                if (Spans.Any())
                    span = Spans.Last();
                else
                    break;
            }

            SelectionStart--;
            TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(TextChangedKind.Remove, false));
            MakeCharAlert(null);
            Invalidate();
            return;
        }

        var spanIndex = 0;
        var totalCharIndex = 0;
        foreach (var span in Spans)
        {
            for (var c = 0; c < span.text.Length; c++, totalCharIndex++)
            {
                if (totalCharIndex == SelectionStart)
                {
                    var removed = false;
                    if (c > 0)
                    {
                        span.text = span.text.Remove(c - 1, 1);
                        removed = true;
                    }
                    else if (spanIndex > 0)
                    {
                        var rspan = Spans[--spanIndex];
                        while (rspan.text.Length == 0)
                        {
                            Spans.Remove(rspan);
                            if (spanIndex >= 0)
                                rspan = Spans[spanIndex];
                            else
                            {
                                rspan = null;
                                break;
                            }
                        }
                        if (rspan != null)
                        {
                            rspan.text = rspan.text.Remove(rspan.text.Length - 1);
                        }
                        removed = true;
                    }

                    string Text = null;
                    if (removed)
                    {
                        SelectionStart--;
                        Text = SetSpans();
                    }

                    TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(TextChangedKind.Remove, false));
                    MakeCharAlert(Text);
                    Invalidate();
                    return;
                }
            }
            spanIndex++;
        }
    }

    private string SetSpans() =>
        //string text = "";
        //while (Spans.Any())
        //{
        //    var span = Spans.First();
        //    text += span.text;
        //    span.Dispose();
        //    Spans.RemoveAt(0);
        //}
        //Spans.AddRange(GetScriptBoxSpans(text));

        //return text;

        SetSpansLocateChanges(out var a, out var b);

    private string SetSpansLocateChanges(out int lastSpanEqualBefore, out int firstSpanEqualAfter)
    {
        var Spans = OldSpans;

        lastSpanEqualBefore = -1;
        firstSpanEqualAfter = -1;

        var Text = this.Text;
        var newSpans = GetScriptBoxSpans(Text);

        // location algorithm: iterates from start to end & from end to start, and stops a direction when meeting non-equality:  >>>they  will make  cookies<<<
        //                                                                                                                       >>>they >have made< cookies<<<
        for (var i = 0; i < newSpans.Length && i < Spans.Count && (lastSpanEqualBefore < 0 || firstSpanEqualAfter < 0); i++)
        {
            if (lastSpanEqualBefore == -1)
            {
                if (!newSpans[i].Equals(Spans[i]))
                    lastSpanEqualBefore = i;
                else
                {
                    //newSpans[i].Dispose();
                    newSpans[i] = Spans[i];
                }
            }
            if (firstSpanEqualAfter == -1)
            {
                int i_new = newSpans.Length - i - 1, i_old = Spans.Count - i - 1;
                if (!newSpans[i_new].Equals(Spans[i_old]))
                    firstSpanEqualAfter = i;
                else
                {
                    //newSpans[i_new].Dispose();
                    newSpans[i_new] = Spans[i_old];
                }
            }
        }

        this.Spans.Clear();
        this.Spans.AddRange(newSpans);
        Spans.Clear();
        Spans.AddRange(newSpans);
        return Text;
    }

    public ScriptBoxSpan[] GetScriptBoxSpans(string text) => Global.GetScriptBoxSpans(text, replaceTabSpaceWith: TabSpace);

    private void SpansTextBox2_Load(object sender, EventArgs e)
    {
        BackColor = Color.FromArgb(255, 42, 42, 42);
        CaretTimer.Start();
        CaretTimer.Enabled = Focused;
        var form = FindForm();
        if (form != null)
        {
            form.FormClosed += (s, ea) =>
            {
                while (Spans.Any())
                {
                    var span = Spans[0];
                    span.Dispose();
                    Spans.RemoveAt(0);
                }
                try
                {
                    CaretTimer?.Dispose();
                    toolTipFont?.Dispose();
                    toolTipTitleFont?.Dispose();
                }
                catch { }
                if (!IsDisposed)
                    Dispose();
            };
        }
    }

    private void SpansTextBox2_MouseWheel(object sender, MouseEventArgs e)
    {
        // set scroll jump for 3 lines per wheel rotation
        var scrollJump = (int)(LineSpacing * 3);

        // convert to scroll bar scale
        var textHeight = (int)((displayedText ?? Text).CountOf('\n') * LineSpacing);
        scrollJump = Global.MapGPT(scrollJump, 0, textHeight, vScrollBar.Minimum, vScrollBar.Maximum);

        if (e.Delta > 0)
        {
            // wheel rotated up
            if (vScrollBar.Minimum <= vScrollBar.Value - scrollJump)
                vScrollBar.Value -= scrollJump;
            else
                vScrollBar.Value = vScrollBar.Minimum;

            Invalidate();
        }
        else if (e.Delta < 0)
        {
            // wheel rotated down
            if (vScrollBar.Maximum >= vScrollBar.Value + scrollJump)
                vScrollBar.Value += scrollJump;
            else
                vScrollBar.Value = vScrollBar.Maximum;

            Invalidate();
        }
    }

    private Point lastMouseHoverLoc = Point.Empty;

    private void SpansTextBox2_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var charIndex = GetCharIndexByPosition(e.Location);
            if (charIndex != SelectionStart)
            {
                SelectionLength = Math.Abs(mouseDownCharInd - charIndex);
                if (charIndex < mouseDownCharInd)
                    SelectionStart = charIndex;
            }
        }
        else if (ModifierKeys == Keys.Control)
        {
            SetLinkCursorAsync(e.Location);
        }
        else if (toolTip.Active)
        {
            if (ToolTipShowLoc.X != -1 && ToolTipShowLoc.Y != -1 && e.Location.DistanceTo(ToolTipShowLoc) > LineSpacing)
            {
                toolTip.Hide(this);
                ToolTipShowLoc = new Point(-1, -1);
            }
        }

        if (lastMouseHoverLoc.DistanceTo(e.Location) > LineSpacing)
        {
            ResetMouseEventArgs(); // this way mouse hover would fire more than 1 times
            lastMouseHoverLoc = e.Location;
        }
    }

    private async void SetLinkCursorAsync(Point? mouseLoc = null)
    {
        if (mouseLoc == null || !mouseLoc.HasValue)
            mouseLoc = Cursor.Position;
        await Task.Run(() =>
        {
            var charIndex = GetCharIndexByPosition(mouseLoc.Value);
            for (int span = 0, totalCharIndex = 0; span < Spans.Count; span++)
            {
                for (var i = 0; i < Spans[span].text.Length; i++, totalCharIndex++)
                {
                    if (totalCharIndex == charIndex)
                    {
                        if (Spans[span].isLink)
                        {
                            if (InvokeRequired)
                            {
                                Invoke(new MethodInvoker(() => Cursor = Cursors.Hand));
                            }
                            else
                            {
                                Cursor = Cursors.Hand;
                            }
                        }
                        else if (Cursor == Cursors.Hand)
                        {
                            if (InvokeRequired)
                            {
                                Invoke(new MethodInvoker(() => Cursor = Cursors.IBeam));
                            }
                            else
                            {
                                Cursor = Cursors.IBeam;
                            }
                        }
                        return;
                    }
                }
            }
        });
    }

    private void SpansTextBox2_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // select the span the user clicked on

            // first, find the char index of the click
            var charIndex = GetCharIndexByPosition(e.Location);

            // find the span that this char belongs to
            int spanIndex = 0, spanStartCharIndex = 0;
            for (int i = 0, spanCharIndex = 0; i < charIndex; i++)
            {
                if (spanCharIndex++ >= Spans[spanIndex].text.Length)
                {
                    spanIndex++;
                    spanCharIndex = 0;
                    spanStartCharIndex = i;
                    i--;
                }
            }

            // select the span
            SelectionStart = spanStartCharIndex;
            SelectionLength = Spans[spanIndex].text.Length;
        }
    }

    private void SpansTextBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (completionBox.Visible && e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || (completionBox.SelectedItem != null && e.KeyCode == Keys.Enter) || e.KeyCode == Keys.Escape)
        {
            e.IsInputKey = false;
            completionBox_PreviewKeyDown(sender, e);
        }
        if (e.KeyData == Keys.Tab)
            e.IsInputKey = true;
    }

    private void vScrollBar_Scroll(object sender, ScrollEventArgs e) => Invalidate();

    private void hScrollBar_Scroll(object sender, ScrollEventArgs e) => Invalidate();

    private void SpansTextBox2_GotFocus(object sender, EventArgs e)
    {
        CaretTimer.Enabled = true;
        drawCaret = true;
        Invalidate();
        skipNextCaretTimerTick = true;
    }

    private void completionBox_MouseClick(object sender, MouseEventArgs e) => SelectSuggestion();

    private void SpansTextBox2_LostFocus(object sender, EventArgs e)
    {
        CaretTimer.Enabled = false;
        drawCaret = false;
        Invalidate();
    }

    private void completionBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && completionBox.SelectedItem != null && completionBox.Visible)
        {
            interceptEnterKeyPress = true;
            SelectSuggestion();
        }
        else if (e.KeyCode == Keys.Down)
        {
            if (completionBox.SelectedIndex < completionBox.Items.Count - 1)
                completionBox.SelectedIndex++;
        }
        else if (e.KeyCode == Keys.Up)
        {
            if (completionBox.SelectedIndex > 0)
                completionBox.SelectedIndex--;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            completionBox.Items.Clear();
            completionBox.Visible = false;
        }
        else
        {
            e.IsInputKey = false;
            Focus();
        }
    }

    private int suggestionSpanStart, suggestionSpanEnd;

    public void ShowSuggestions(SpansTextBox2Suggestion[] suggestions, int start, int end)
    {
        completionBox.Items.Clear();

        SpansTextBox2Suggestion matchSug = null;
        if (end > start)
        {
            var span = TextRange(start, end - 1);
            Array.Sort(suggestions, new SuggestionsSorter(span));
            matchSug = suggestions.Find(s => s.DisplayText.Equals(span, StringComparison.CurrentCultureIgnoreCase));
        }
        completionBox.Items.AddRange(suggestions);

        completionBox.Visible = suggestions.Length >= 1;

        var caretPos = GetPositionOfCharIndex(SelectionStart, ignoreScrolling: true);
        Point loc = new Point((int)caretPos.X, (int)(caretPos.Y + LineSpacing));
        if (loc.X + completionBox.Size.Width >= DisplayRectangle.Width)
            loc.X -= loc.X + completionBox.Size.Width - DisplayRectangle.Width;
        if (loc.Y + completionBox.Size.Height >= DisplayRectangle.Height)
            loc.Y -= (completionBox.Size.Height - DisplayRectangle.Height) + (int)LineSpacing;
        completionBox.Location = loc;

        if (matchSug == null)
            completionBox.SelectedIndex = 0;
        else
            completionBox.SelectedItem = matchSug;

        suggestionSpanStart = start;
        suggestionSpanEnd = end;
    }

    private void SelectSuggestion()
    {
        if (completionBox.SelectedItem is SpansTextBox2Suggestion sug && completionBox.Visible)
        {
            // find current span
            //bool inserted = false;
            //for (int spanIndex = 0, totalCharIndex = 0; !inserted && spanIndex < Spans.Count; spanIndex++)
            //{
            //    for (int charIndex = 0; !inserted && charIndex < Spans[spanIndex].text.Length; charIndex++, totalCharIndex++)
            //    {
            //        if (totalCharIndex == SelectionStart - 1)
            //        {
            //            string spanLastText = Spans[spanIndex].text;
            //            Spans[spanIndex].text = sug.Text;
            //            SelectionStart = totalCharIndex + sug.Text.Length - spanLastText.Length + 1;
            //            inserted = true;
            //        }
            //    }
            //}

            //if (!inserted)
            //{
            //    Spans.Add(new ScriptBoxSpan { text = sug.Text });
            //    SelectionStart += sug.Text.Length;
            //}

            var oldText = Text;
            Spans.Clear();
            var newText = oldText.Insert(suggestionSpanEnd, sug.Text).Remove(suggestionSpanStart, suggestionSpanEnd - suggestionSpanStart);
            Spans.Add(new ScriptBoxSpan { text = newText });
            SelectionStart += newText.Length - oldText.Length;

            SetSpans();
            TextChanged?.Invoke(this, new SpansTextBox2TextChangedEventArgs(suggestionSpanEnd > suggestionSpanStart ? TextChangedKind.Replace : TextChangedKind.Insert, true));

            //Invalidate(); [needed?]
            completionBox.Visible = false;
            Focus();
        }
    }

    public void HideSuggestions()
    {
        completionBox.Items.Clear();
        completionBox.Visible = false;
    }

    private Point ToolTipShowLoc { get; set; } = new Point(-1, -1);
    private string toolTipText = null;

    public void ShowHint(string text, Point position, string title = null)
    {
        toolTip.ToolTipTitle = title;
        toolTipText = text;
        toolTip.Show(text, this, position);
        ToolTipShowLoc = position;
    }

    public void ShowHint(string text, string title = null) => ShowHint(text, PointToClient(Cursor.Position).WithY(+(int)LineSpacing, relative: true), title);

    public bool HintIsShown => ToolTipShowLoc.X != -1 && ToolTipShowLoc.Y != -1;

    public void ShowParamDesc(string title, string summary, Point loc)
    {
        paramToolTip.ToolTipTitle = title;
        toolTipText = summary;
        paramToolTip.Show(summary, this, loc);
    }

    private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
    {
        using (Pen pen = new Pen(toolTip.BackColor))
        {
            e.Graphics.FillRectangle(pen.Brush, e.Bounds);
            pen.Color = toolTip.ForeColor;
            e.Graphics.DrawRectangle(pen, e.Bounds);
            pen.Color = Color.Black;

            Rectangle textLayout = new Rectangle(5, 5, DisplayRectangle.Width - (int)maxToolTipPadding.Width, DisplayRectangle.Height - (int)maxToolTipPadding.Height);
            e.Graphics.DrawString(toolTip.ToolTipTitle, toolTipTitleFont, pen.Brush, textLayout);
            textLayout.Y = 15 + (int)toolTipTitleSize.Height;
            e.Graphics.DrawString(e.ToolTipText, toolTipFont, pen.Brush, textLayout);
        }
    }

    private readonly Font toolTipFont = new Font(FontFamily.GenericMonospace, 8);
    private Font toolTipTitleFont => Font;
    private SizeF toolTipTitleSize = SizeF.Empty;
    private SizeF toolTipTextSize = SizeF.Empty;

    /// <summary>The maximum possible size of the tool tip is <c>DisplayRectangle.Size</c> minus this size</summary>
    private SizeF maxToolTipPadding = new Size(100, 100);

    private void toolTip_Popup(object sender, PopupEventArgs e)
    {
        using (var graphics = CreateGraphics())
        {
            if (string.IsNullOrWhiteSpace(toolTip.ToolTipTitle))
                toolTipTitleSize = new SizeF(0, 0);
            else
                toolTipTitleSize = graphics.MeasureString(toolTip.ToolTipTitle, toolTipFont, DisplayRectangle.Size.Width - (int)maxToolTipPadding.Width);
            toolTipTextSize = graphics.MeasureString(toolTipText, toolTipTitleFont, DisplayRectangle.Size - maxToolTipPadding);
            e.ToolTipSize = new Size((int)Math.Max(toolTipTitleSize.Width, toolTipTextSize.Width) + 30, (int)(toolTipTitleSize.Height + toolTipTextSize.Height + 30));
        }
    }

    public void HideParamDesc()
    {
        if (InvokeRequired)
            Invoke(new MethodInvoker(() => paramToolTip.Hide(this)));
        else
            paramToolTip.Hide(this);
    }

    public event EventHandler<CompletionItemInfoEventArgs> OnCompletionItemShowInfo;

    private void ShowCompletionItemInfo()
    {
        toolTipText = null;
        completionToolTip.Hide(this);

        var ea = new CompletionItemInfoEventArgs(null, completionBox.SelectedItem as SpansTextBox2Suggestion);
        OnCompletionItemShowInfo?.Invoke(this, ea);
        if (ea.Text != null)
        {
            toolTipText = ea.Text;
            completionToolTip.Show(ea.Text, this, completionBox.Location.WithX(completionBox.Location.X + completionBox.Size.Width));
        }
    }

    public int GetSpanIndexByCharIndex(int index)
    {
        for (int spanIndex = 0, totalCharIndex = 0; spanIndex < Spans.Count; spanIndex++)
        {
            for (var charIndex = 0; charIndex < Spans[spanIndex].text.Length; charIndex++, totalCharIndex++)
            {
                if (totalCharIndex == index)
                    return spanIndex;
            }
        }
        return -1;
    }

    public ScriptBoxSpan GetSpanByCharIndex(int index)
    {
        var spanIndex = GetSpanIndexByCharIndex(index);
        if (spanIndex >= 0)
            return Spans[spanIndex];
        return null;
    }

    private readonly List<KnownCharSize> knownCharSizes = [];

    private class KnownCharSize(char c, SizeF size)
    {
        public char Char { get; } = c;
        public SizeF Size { get; } = size;
    }

    private SizeF GetKnownCharSize(char c)
    {
        var known = knownCharSizes.Find(kc => kc.Char == c);
        return known != null ? known.Size : new SizeF(-1, -1);
    }

    private bool CharSizeIsKnown(char c) => GetKnownCharSize(c).Width >= 0;

    private SizeF MeasureString(string str, Graphics g = null)
    {
        ArgumentNullException.ThrowIfNull(str);

        SizeF size;
        var dispose = g == null;
        if (dispose)
            g = CreateGraphics();
        try
        {
            size = g.MeasureString(str, Font, int.MaxValue, drawTextFormat);
        }
        finally
        {
            if (dispose)
                g.Dispose();
        }
        return size;
        /*
        SizeF size = str.Length > 0 ? new SizeF(0, LineSpacing) : SizeF.Empty;

        bool dispose = g == null;
        try
        {
            foreach (char c in str)
            {
                if (c == '\n')
                {
                    size.Height += LineSpacing;
                    continue;
                }
                else if (!CharSizeIsKnown(c))
                {
                    if (g == null)
                        g = CreateGraphics();
                    knownCharSizes.Add(new KnownCharSize(c, g.MeasureString(c.ToString(), Font, int.MaxValue, drawTextFormat)));
                }
                size.Width += GetKnownCharSize(c).Width;
            }
        }
        finally
        {
            if (dispose)
                g?.Dispose();
        }

        return size;
        */
    }

    private SizeF MeasureString(char str, Graphics g = null) => MeasureString(str.ToString(), g);
}

public enum TextChangedKind
{
    Insert,
    Remove,
    Replace
}

public class SpansTextBox2TextChangedEventArgs(TextChangedKind kind, bool bySuggestion) : EventArgs
{
    public TextChangedKind Kind { get; set; } = kind;
    public bool BySuggestion { get; set; } = bySuggestion;
}

public class SpansTextBox2CharAlertEventArgs(char alert, int index, ScriptBoxSpan spanBefore = null) : EventArgs
{
    public char Alert { get; } = alert;
    public int Index { get; } = index;
    public ScriptBoxSpan SpanBefore { get; } = spanBefore;
}

public class SpansTextBox2Suggestion
{
    public string DisplayText { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }

    public SpansTextBox2Suggestion(string displayText, string text = null)
    {
        if (text == null)
            text = displayText;
        DisplayText = displayText;
        Text = text;
    }

    public override string ToString() => DisplayText;
}

public class SuggestionsSorter(string span) : IComparer<SpansTextBox2Suggestion>
{
    public string Span { get; set; } = span;

    public int Compare(SpansTextBox2Suggestion a, SpansTextBox2Suggestion b)
    {
        if (Span is "" or null)
            return 0;

        var ignoreCase = StringComparison.CurrentCultureIgnoreCase;
        int indexAtA = a.DisplayText.IndexOf(Span, ignoreCase), indexAtB = b.DisplayText.IndexOf(Span, ignoreCase);
        if (indexAtA == indexAtB)
            return 0;
        if (indexAtA > indexAtB)
            return -1;
        return 1;
    }
}

public class CompletionItemInfoEventArgs(string text, SpansTextBox2Suggestion item) : EventArgs
{
    public string Text { get; set; } = text;
    public SpansTextBox2Suggestion Item { get; set; } = item;
}