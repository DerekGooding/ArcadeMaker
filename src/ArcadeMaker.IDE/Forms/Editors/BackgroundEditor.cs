using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE;

public partial class BackgroundEditor : Form
{
    private GameBackground background;

    public BackgroundEditor(GameBackground background)
    {
        InitializeComponent();
        this.background = background;
        nameBox.Text = background.name;
        background.NameChanged += (s, e) =>
        {
            if (!renaming)
                nameBox.Text = e.newName;
        };
    }

    private void BackgroundEditor_Load(object sender, EventArgs e) => imageBox.Image = background.image;

    private bool renaming = false;

    private void nameBox_TextChanged(object sender, EventArgs e)
    {
        renaming = true;
        try
        {
            background.name = nameBox.Text;
            if (nameBox.BackColor == Color.Red)
                nameBox.BackColor = Color.White;
        }
        catch
        {
            nameBox.BackColor = Color.Red;
        }
        renaming = false;
    }

    private void okBtn_Click(object sender, EventArgs e) => Close();

    private void importBtn_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            Bitmap image = (Bitmap)Global.ImageFromFile(openFileDialog.FileName);
            background.image = image;
            imageBox.Image = image;
        }
    }

    private void editBtn_Click(object sender, EventArgs e)
    {
        SpriteDesigner designer = new SpriteDesigner(background.image);
        designer.ShowDialog();
    }
}