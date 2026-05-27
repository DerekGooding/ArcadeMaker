namespace ArcadeMaker.IDE.Items;

public class GameBackground : GameItem
{
    /* do not change property name!!! */
    public static Bitmap icon { get; } = Properties.Resources.background;

    public Bitmap image
    {
        get;
        set
        {
            field = value;

            if (treeImageIndex >= 0)
            {
                var w = Global.form1.treeImages.ImageSize.Width;
                var h = Global.form1.treeImages.ImageSize.Height;
                Global.form1.treeImages.Images[treeImageIndex] = field == null ? new Bitmap(1, 1) : field.ResizeImage(w, h);
                Global.form1.RefreshTreeView();
            }
        }
    } = null;

    public new BackgroundEditor editor
    {
        get
        {
            if (editorClosed)
            {
                base.editor = new BackgroundEditor(this);
            }
            return base.Editor as BackgroundEditor;
        }

        set => base.editor = value;
    }

    public GameBackground(string name) : base(name)
    {
        instances.Add(this);
        CheckInvitation();
        getEditor += (s, e) =>
        {
            var activateGet = editor;
        };
        editor = new BackgroundEditor(this);
    }

    private static readonly List<GameBackground> instances = [];

    private static readonly List<GameBackgroundInvitation> invitations = [];

    public static void Invite(RoomBackground sender, string name)
    {
        invitations.Add(new GameBackgroundInvitation(name, sender));
        for (var i = instances.Count - 1; i >= 0; i--)
            instances[i].CheckInvitation();
    }

    private void CheckInvitation()
    {
        foreach (var invitation in invitations)
        {
            if (invitation.name == name)
                invitation.sender.image = this;
        }
    }
}

public struct GameBackgroundInvitation(string name, RoomBackground sender)
{
    public string name = name;
    public RoomBackground sender = sender;
}