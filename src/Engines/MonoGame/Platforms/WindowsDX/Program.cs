using ArcadeMaker.Engines.MonoGame.Core;
using ArcadeMaker.Engines.MonoGame.WindowsDX;
using System.Windows.Forms;

namespace ArcadeMaker.Engines.MonoGame.Platforms.WindowsDX;

public static class Program
{
    /// <summary>
    /// The main entry point for the application on Windows.
    /// Configures the application for high DPI awareness.
    /// It also creates an instance of your game and calls it's Run() method
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            throw new System.Exception("No project file was attached.");
        }

        // configure the application to be DPI-aware for better display scaling.
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // create an instance of the game and start the game loop.
        using var game = new ArcadeMakerMonoGame(args[0]);

        Program.game = game;
        game.OnExpRuntimeError += (s, e) => { if (!exceptionViewerIsDisplayed) ShowExceptionViewer(new ExceptionViewerDialog(e)); };
        game.OnCsError += (s, e) => ShowExceptionViewer(new ExceptionViewerDialog(e));

        game.Run();
    }

    private static ArcadeMakerMonoGame game;
    private static bool exceptionViewerIsDisplayed;

    private static void ShowExceptionViewer(ExceptionViewerDialog dialog)
    {
        exceptionViewerIsDisplayed = true;
        var result = dialog.ShowDialog();
        if (result == DialogResult.Abort)
            game.Exit();
        exceptionViewerIsDisplayed = false;
    }
}