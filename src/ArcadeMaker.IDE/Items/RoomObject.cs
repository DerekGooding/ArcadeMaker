using ArcadeMaker.Core.Resources.Serializeables;

namespace ArcadeMaker.IDE.Items;

public class RoomObject : IContainsScript
{
    public readonly string id;
    public int x, y;
    public GameObject obj;

    public RoomObject(string id, int x, int y, GameObject obj)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.obj = obj;

        var use = new string[] { "System", "System.Collections.Generic", "System.Linq", "System.Threading.Tasks", "ArcadeMaker", "ArcadeMaker.Models", "ArcadeMaker.Controls", "ArcadeMaker.GameItems", "ArcadeMaker.Drawing" };
        defaultCreationCode = "";
        foreach (var ns in use)
            defaultCreationCode += "using " + ns + ";\n";
        defaultCreationCode += $"\nnamespace Game\n{{\tpublic static partial class CreationCodes\n\t{{\n\t\tpublic static void {id}_Create({obj.name} instance)\n\t\t{{\n\t\t\t\n\t\t}}\n\t}}\n}}";
        //Script = defaultCreationCode;
    }

    public readonly string defaultCreationCode = null;
    public string Script { get; set; } = null;
    public bool CompiledSyntaxTree { get; set; } = false;

    public bool HasCustomCreationCode() => defaultCreationCode != Script;

    public string ScriptOrDefaultCreationCode => Script ?? defaultCreationCode;
}
