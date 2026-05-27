using Exp;
using System;
using System.Windows.Forms;

namespace ArcadeMaker.Engines.MonoGame.WindowsDX;

public partial class ExceptionViewerDialog : Form
{
    public ExceptionViewerDialog(Exception ex)
    {
        InitializeComponent();

        textBox.Text = $"Engine bug:\n\n{ex}";
    }

    public ExceptionViewerDialog(RuntimeException ex)
    {
        InitializeComponent();

        textBox.Text = $"Exception thrown from {ex.source}({ex.line}, {ex.col}):\n\n{ex.Message}";
    }

    private void abortBtn_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Abort;
        Close();
    }

    private void ignoreBtn_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Ignore;
        Close();
    }
}