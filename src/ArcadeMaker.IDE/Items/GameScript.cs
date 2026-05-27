using ArcadeMaker.Core.Resources.Serializeables;

namespace ArcadeMaker.IDE.Items;

public class GameScript : GameItem, IContainsScript
{
    /* do not change property name!!! */
    public static System.Drawing.Bitmap icon { get; } = Properties.Resources.script;

    public string Script { get; set; } = "";
    public bool CompiledSyntaxTree { get; set; } = false;

    public new ScriptEditor editor
    {
        get
        {
            if (editorClosed)
            {
                base.editor = new ScriptEditor(this, Script);
                (base.editor as ScriptEditor).OKClicked += (s, e) => Script = e;
            }
            return base.Editor as ScriptEditor;
        }
        set
        {
            value?.OKClicked += (s, e) => Script = e;
            base.editor = value;
        }
    }

    public GameScript(string name, string code = null) : base(name)
    {
        if (code != null)
            Script = code;
        base.getEditor += (s, e) =>
        {
            e = editor;
        };
        editor = new ScriptEditor(this, Script);
    }

    public void InitDefaultCode()
    {
        string code = $"func {name}()\n{{\n\t\n}}";
        Script = code;
    }
}