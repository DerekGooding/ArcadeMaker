namespace ArcadeMaker.IDE.Items;

public class RoomBackground
{
    private static int count = 0;
    private readonly int index = 0;

    [System.Xml.Serialization.XmlIgnore]
    public GameBackground image = null;

    public string gameBackgroundName
    {
        get
        {
            if (image != null)
                return image.name;
            return null;
        }

        set => GameBackground.Invite(this, value);
    }

    public bool visible = false;
    public bool foreground = false;
    public bool tileHor = true, tileVer = true;
    public int x = 0, y = 0;
    public bool stretch = false;
    public int horSpd = 0, verSpd = 0;

    public RoomBackground() => index = count++;

    public override string ToString() => "background " + index;
}
