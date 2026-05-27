using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE;

public static class Environment
{
    public static GameProject project = null;

    private static string Basecode = null;
    private static bool readBasecodeAgain = false;

    public static string[] AssembliesLocations
    {
        get
        {
            if (field == null)
                LoadAssembliesLocations();
            return field;
        }

        private set;
    } = null;

    private static void LoadAssembliesLocations()
    {
        List<string> assemblies =
        [
            // we only want the assembly, the class does not matter
            System.Reflection.Assembly.GetAssembly(typeof(System.Linq.Expressions.Expression)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.Random)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.IDisposable)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.IO.Stream)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.Windows.Forms.Form)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.Drawing.Bitmap)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.Linq.Enumerable)).Location,
            System.Reflection.Assembly.GetAssembly(typeof(System.ComponentModel.AddingNewEventArgs)).Location,
            //engineDllLocation,
            //System.Reflection.Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location
        ];

        // load NAudio NuGet package assemblies
        //Type[] NAudio_assemblies_types = new Type[] {
        //    typeof(NAudio.Wave.AudioFileReader),
        //    typeof(NAudio.Wave.WaveStream),
        //    typeof(NAudio.Wave.WaveOut),
        //    typeof(NAudio.Wave.MediaFoundationReader),
        //    typeof(NAudio.Wave.WaveFormatConversionStream)
        //};
        //foreach (Type type in NAudio_assemblies_types)
        //{
        //    string location = System.Reflection.Assembly.GetAssembly(type).Location;
        //    if (!assemblies.Contains(location))
        //        assemblies.Add(location);
        //}

        AssembliesLocations = assemblies.ToArray();
    }

    private static string[] GetNAudioAssembliesLocations()
    {
        List<string> assemblies = [];
        return assemblies.ToArray();
    }

    public static event EventHandler<int> ProgressUpdated;

    public static int Progress
    {
        get;
        private set
        {
            field = value;
            if (ProgressUpdated != null)
                ProgressUpdated(null, value);
        }
    } = 0;

    private const string EngineDllResName = "engine.dll";
    internal static bool isGameRunning = false;

    public static void GenerateExe(string savePath = null, bool run = false, bool console = true)
    {
        var rooms = project.items.OfType<GameRoom>();
        if (!rooms.Any())
        {
            MessageBox.Show("Game must have at least 1 room.");
            return;
        }

        var debugPath = AppDomain.CurrentDomain.BaseDirectory + "\\DEBUG";
        const string debugPname = "debugbuild";
        project.Save(debugPath, successMsg: false, fileName: debugPname);
        Progress = 50;
        isGameRunning = true;

        Engines.MonoGame.Platforms.WindowsDX.Program.Main([debugPath + $@"\{debugPname}\{debugPname}.gsp"]);

        Progress = 100;
        isGameRunning = false;
    }
}