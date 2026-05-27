namespace ArcadeMaker.IDE;

public partial class SpansTextBox2SearchForm : Form
{
    public SpansTextBox2 scriptBox = null;

    public SpansTextBox2SearchForm(SpansTextBox2 scriptBox)
    {
        InitializeComponent();
        this.scriptBox = scriptBox ?? throw new ArgumentNullException("scriptBox");

        Form owner = scriptBox.FindForm();
        if (owner != null)
            Owner = owner;
    }

    private int FindNextIndex(bool notFoundMsg = true)
    {
        if (!string.IsNullOrWhiteSpace(searchBox.Text))
        {
            int startIndex = scriptBox.SelectionStart;
            if (scriptBox.SelectionLength > 0 && scriptBox.Text.Length > startIndex)
                startIndex++;

            StringComparison comparison = matchCaseBox.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            int index = scriptBox.Text.IndexOf(searchBox.Text, startIndex, comparison);
            if (index < 0)
            {
                // if not found
                if (scriptBox.SelectionLength == 0 && scriptBox.SelectionStart > 0)
                {
                    scriptBox.SelectionStart = 0;
                    scriptBox.SelectionLength = 0;
                    return FindNextIndex(notFoundMsg);
                }
                else
                {
                    if (notFoundMsg)
                        MessageBox.Show("The given text was not found.");
                    return -1;
                }
            }
            else
            {
                return index;
            }
        }
        else
        {
            MessageBox.Show("Enter a valid text to search.");
            return -1;
        }
    }

    private bool ReplaceNext(bool notFoundMsg = true)
    {
        int startIndex = FindNextIndex(notFoundMsg);
        if (startIndex > 0)
        {
            string newText = scriptBox.Text.Remove(startIndex, searchBox.Text.Length);
            newText = newText.Insert(startIndex, replaceBox.Text);
            scriptBox.Spans.Clear();
            scriptBox.Spans.AddRange(scriptBox.GetScriptBoxSpans(newText));
            scriptBox.SelectionStart = startIndex;
            scriptBox.SelectionLength = replaceBox.Text.Length;
            return FindNextIndex(notFoundMsg) > startIndex - (searchBox.Text.Length - replaceBox.Text.Length);
        }
        return false;
    }

    private void findNextBtn_Click(object sender, EventArgs e)
    {
        int startIndex = FindNextIndex();
        if (startIndex > 0)
        {
            scriptBox.SelectionStart = startIndex;
            scriptBox.SelectionLength = searchBox.Text.Length;
        }
    }

    private void replaceNextBtn_Click(object sender, EventArgs e)
    {
        ReplaceNext();
    }

    private void replaceAllBtn_Click(object sender, EventArgs e)
    {
        bool next;
        do
        {
            next = ReplaceNext(false);
        }
        while (next);
    }

    private void SpansTextBox2SearchForm_Load(object sender, EventArgs e)
    {
    }
}