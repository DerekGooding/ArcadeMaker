using ArcadeMaker.Core.Resources.Serializeables;
using ArcadeMaker.IDE.Editors.Object.ObjectProperties;

namespace ArcadeMaker.IDE.Items;

public class GameObject : GameItem //, IContainsScript
{
    public static System.Drawing.Bitmap icon { get; } = Properties.Resources.object32;

    public List<IDEObjectProperty> ExtraProperties { get; } = [];
    public List<EventScripts> EventScripts { get; } = [];

    public bool CompiledSyntaxTree { get; set; } = false;

    public bool compiledModelsTree = false;

    private string _part2script = null;
    private GameSprite _sprite = null;

    public GameSprite sprite
    {
        get
        {
            return _sprite;
        }
        set
        {
            _sprite = value;
            if (treeImageIndex >= 0)
            {
                if (sprite != null && treeNode != null)
                {
                    treeNode.ImageIndex = _sprite.treeImageIndex;
                    treeNode.SelectedImageIndex = _sprite.treeImageIndex;
                }
                else
                {
                    Global.form1.treeImages.Images[treeImageIndex] = new System.Drawing.Bitmap(1, 1);
                    treeNode.ImageIndex = treeImageIndex;
                    treeNode.SelectedImageIndex = treeImageIndex;
                }
                Global.form1?.RefreshTreeView();
            }
        }
    }

    public new ObjectEditor editor
    {
        get
        {
            if (editorClosed)
            {
                base.editor = new ObjectEditor(this);
            }
            return base.Editor as ObjectEditor;
        }
        set
        {
            base.editor = value;
        }
    }

    public GameObject(string name) : base(name)
    {
        getEditor += (s, e) =>
        {
            e = this.editor;
        };
        editor = new ObjectEditor(this);
        base.NameChanged += (s, e) =>
        {
            compiledModelsTree = false;
        };
    }

    public bool solid = false;
    public int depth;
    public GameObject parent;

    internal EventScripts? GetEventScripts(ObjectEvent ev)
    {
        return EventScripts.FirstOrDefault(es => es.Event == ev);
    }
}