using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE;

public partial class RoomInstanceCreationCodeEditor : Form
{
    private RoomObject obj = null;

    public RoomInstanceCreationCodeEditor(RoomObject obj)
    {
        InitializeComponent();
        this.obj = obj;
        codeBox.Text = obj.Script;
        Text = "Edit creation code for instance of " + obj.obj.name;
    }

    private void resetCodeBtn_Click(object sender, EventArgs e)
    {
        if (codeBox.Text != obj.defaultCreationCode)
        {
            if (MessageBox.Show("Are you sure you want to reset creation code for this instance?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                codeBox.Text = obj.defaultCreationCode;
        }
    }

    private bool closedWithOkBtn = false;

    private void okBtn_Click(object sender, EventArgs e)
    {
        SaveChanges();
        closedWithOkBtn = true;
        Close();
    }

    private void RoomInstanceCreationCodeEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!closedWithOkBtn && codeBox.Text != obj.defaultCreationCode)
        {
            DialogResult result = MessageBox.Show("Do you want to save changes?", "Close", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (result == DialogResult.Yes)
            {
                SaveChanges();
            }
        }
    }

    private void SaveChanges()
    {
        obj.Script = codeBox.Text;
    }
}