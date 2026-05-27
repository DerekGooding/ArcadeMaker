using ArcadeMaker.IDE.Items;
using Timer = System.Windows.Forms.Timer;

namespace ArcadeMaker.IDE
{
    public partial class GameScriptEditor : Form
    {
        public GameScript script;

        public GameScriptEditor(GameScript script)
        {
            InitializeComponent();
            this.script = script;

            script.NameChanged += (s, e) =>
            {
                if (!renaming)
                    nameBox.Text = e.newName;
            };

            compileTimer.Tick += compileTimer_Tick;
        }

        private void GameScriptEditor_Load(object sender, EventArgs e)
        {
            nameBox.Text = script.name;
            scriptBox.Text = script.Script;
        }

        private bool renaming = false;

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            renaming = true;
            try
            {
                script.name = nameBox.Text;
                if (nameBox.BackColor == Color.Red)
                    nameBox.BackColor = Color.White;
            }
            catch
            {
                nameBox.BackColor = Color.Red;
            }
            renaming = false;
        }

        private void scriptBox_TextChanged(object sender, EventArgs e)
        {
            script.Script = scriptBox.Text;
            if (compileTimer.Enabled)
                compileTimer.Stop();
            compileTimer.Start();
        }

        private Timer compileTimer = new Timer { Interval = 5000 };

        private void compileTimer_Tick(object sender, EventArgs e)
        {
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GameScriptEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            compileTimer.Stop();
            compileTimer.Dispose();
        }
    }
}