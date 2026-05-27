using ArcadeMaker.Core.Resources.Serializeables;

namespace ArcadeMaker.IDE.Items;

public class GameRoom : GameItem, IContainsScript
{
    /* do not change property name!!! */
    public static Bitmap icon { get; } = Properties.Resources.map;

    public readonly int index;
    private static int count = 0;

    public RoomSize size = new RoomSize(640, 480);
    public int speed = 30;
    public bool persistent = false;
    public List<RoomObject> objects = [];

    public bool viewsEnabled = false;
    public static int minNumOfViews { get; } = 5;
    public List<RoomView> views = [];

    public string caption = "";
    public RoomBackground[] backgrounds = new RoomBackground[8];
    public bool drawBackColor = true;
    public Color backColor = Color.Silver;

    private const string defaultScript = "namespace Game\n{\n\tpublic partial class RoomName\n\t{\n\t\tprotected override void Create()\n\t\t{\n\t\t\t\n\t\t}\n\t}\n}";

    public string Script
    {
        get
        {
            if (field == null)
                field = defaultScript.Replace("RoomName", name);
            return field;
        }
        set;
    } = null;

    public new RoomEditor editor
    {
        get
        {
            if (editorClosed)
            {
                base.editor = new RoomEditor(this);
            }
            return base.Editor as RoomEditor;
        }

        set => base.editor = value;
    }

    public GameRoom(string name, int index = -1) : base(name)
    {
        this.index = count++;
        if (index >= 0)
            this.index = index;
        getEditor += (s, e) =>
        {
            var activateGet = editor;
        };
        if (Environment.project != null)
            caption = Environment.project.name;
        else
            caption = name;
        editor = new RoomEditor(this);

        if (views.Count == 0)
        {
            for (var v = 0; v < minNumOfViews; v++)
            {
                views.Add(new RoomView());
            }
        }

        for (var i = 0; i < backgrounds.Length; i++)
            backgrounds[i] = new RoomBackground();
    }
}
