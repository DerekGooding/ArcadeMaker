global using ObjectEvent = ArcadeMaker.Core.Resources.Serializeables.ObjectEvent;

namespace ArcadeMaker.IDE
{
    public partial class SelectEventDialog : Form
    {
        public SelectEventDialog()
        {
            InitializeComponent();
        }

        private void SelectEventDialog_Load(object sender, EventArgs e)
        {
        }

        public event EventHandler<ObjectEvent> EventSelected;

        private void keyDownBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.KeyDown);
        }

        private void SelectEvent(ObjectEvent ev)
        {
            var handler = EventSelected;
            handler?.Invoke(this, ev);
            Close();
        }

        private void keyUpBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.KeyUp);
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.Create);
        }

        private void stepBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.Step);
        }

        private void mouseDownBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.MouseDown);
        }

        private void mousePressBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.MousePress);
        }

        private void mouseUpBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.MouseUp);
        }

        private void drawBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.Draw);
        }

        private void keyPressBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.KeyPress);
        }

        //private void mouseMoveBtn_Click(object sender, EventArgs e)
        //{
        //    SelectEvent(ObjectEvent.MouseMove);
        //}

        private void mouseWheelBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.MouseWheel);
        }

        private void alarmBtn_Click(object sender, EventArgs e)
        {
            SelectEvent(ObjectEvent.Alarm);
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mouseMenuBtn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) // else the menu will automatically open
                mouseMenuBtn.ContextMenuStrip.Show(mouseMenuBtn, Point.Empty);
        }
    }
}