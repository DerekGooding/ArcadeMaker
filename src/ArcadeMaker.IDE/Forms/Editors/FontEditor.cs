using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE
{
    public partial class FontEditor : Form
    {
        private GameFont font = null;

        public FontEditor(GameFont font)
        {
            this.font = font;

            InitializeComponent();

            font.NameChanged += (s, e) =>
            {
                skipSetFontProperties = true;

                nameBox.Text = e.newName;

                skipSetFontProperties = false;
            };

            foreach (FontFamily family in FontFamily.Families)
            {
                familiesBox.Items.Add(family.Name);
            }
        }

        private bool skipSetFontProperties = false;

        private void FontEditor_Load(object sender, EventArgs e)
        {
            skipSetFontProperties = true;

            nameBox.Text = font.name;
            familiesBox.SelectedIndex = FontFamily.Families.IndexOf(FontFamily.Families.Find(f => f.Name == font.family));
            sizeBox.Value = (decimal)font.size;
            boldBox.Checked = font.bold;
            italicBox.Checked = font.italic;

            skipSetFontProperties = false;

            UpdatePreview();
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            if (skipSetFontProperties)
                return;

            if (nameBox.Text.IsPossibleName(font.name))
            {
                font.name = nameBox.Text;
                nameBox.BackColor = Color.White;
            }
            else
            {
                nameBox.BackColor = Color.Red;
            }
        }

        private void familiesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (skipSetFontProperties)
                return;

            font.family = familiesBox.SelectedItem.ToString();
            UpdatePreview();
        }

        private void sizeBox_ValueChanged(object sender, EventArgs e)
        {
            if (skipSetFontProperties)
                return;

            font.size = (int)sizeBox.Value;
            UpdatePreview();
        }

        private void boldBox_CheckedChanged(object sender, EventArgs e)
        {
            if (skipSetFontProperties)
                return;

            font.bold = boldBox.Checked;
            UpdatePreview();
        }

        private void italicBox_CheckedChanged(object sender, EventArgs e)
        {
            if (skipSetFontProperties)
                return;

            font.italic = italicBox.Checked;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            previewLbl.Font = new Font(font.family, font.size, font.bold && font.italic ? FontStyle.Bold | FontStyle.Italic : font.bold ? FontStyle.Bold : font.italic ? FontStyle.Italic : FontStyle.Regular);
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}