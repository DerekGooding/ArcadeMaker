using ArcadeMaker.Core.Resources;

namespace ArcadeMaker.IDE.Items;

public class GameSound : GameItem
{
    /* do not change property name!!! */
    public static System.Drawing.Bitmap icon { get; } = Properties.Resources.sound;
    public string filePath { get; set; } = null;

    /// <summary>
    /// The extension of the file, without dot ("wav", "mp3" "mid"...)
    /// </summary>
    public string fileExtension => filePath != null && filePath.Contains('.') ? filePath.Substring(filePath.LastIndexOf('.') + 1) : null;

    public float volume = 1.0F;
    public Sound.Types Type { get; set; } = Sound.Types.SoundEffect;

    public new SoundEditor editor
    {
        get
        {
            if (editorClosed)
            {
                base.editor = new SoundEditor(this);
            }
            return base.Editor as SoundEditor;
        }

        set => base.editor = value;
    }

    public float Pan { get; internal set; }
    public float Pitch { get; internal set; }

    public GameSound(string name, string filePath = null) : base(name)
    {
        this.filePath = filePath;
        base.getEditor += (s, e) => e = editor;
        editor = new SoundEditor(this);
    }
}