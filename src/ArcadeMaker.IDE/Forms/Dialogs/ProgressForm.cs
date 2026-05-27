namespace ArcadeMaker.IDE;

public partial class ProgressForm : Form
{
    public ProgressForm() => InitializeComponent();

    private void ProgressForm_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, Size.Width, Size.Height);

    private void ProgressForm_Load(object sender, EventArgs e) => Environment.ProgressUpdated += (s, progress) =>
                                                                       {
                                                                           Action upd = new Action(() =>
                                                                           {
                                                                               progressBar.Value = progress;
                                                                               if (progress == 100)
                                                                                   Close();
                                                                           });

                                                                           if (InvokeRequired)
                                                                           {
                                                                               Invoke((MethodInvoker)(() => upd()));
                                                                           }
                                                                           else
                                                                           {
                                                                               upd();
                                                                           }
                                                                       };

    public new void Close()
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)(() => base.Close()));
        }
        else
        {
            base.Close();
        }
    }
}