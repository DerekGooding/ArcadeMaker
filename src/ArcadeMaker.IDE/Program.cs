namespace ArcadeMaker.IDE;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string projectPath = null;
        if (args != null && args.Length > 0 && args[0] != null)
            projectPath = args[0];
        object[] structs = null;
        if (projectPath != null)
        {
            Environment.project = GameProject.Open(projectPath, out structs);
            Global.PushRecentProject(projectPath);
        }
        if (Environment.project == null)
            Environment.project = new GameProject("New Project" /*+ DateTime.Today.ToString("dd-MM-yy")*/);
        Application.Run(new Form1(structs));
    }
}