namespace ArcadeMaker.IDE.Items;

public class RoomView
{
    private static int Count = 0;
    private readonly int Index = 0;

    public bool Visible = false;
    public int X = 0, Y = 0, Width = 640, Height = 480, PortX = 0, PortY = 0, PortWidth = 640, PortHeight = 480;

    public GameObject ObjFollow = null;

    public int FollowHBor = 32, FollowVBor = 32, FollowHSp = -1, FollowVSp = -1;

    public RoomView() => Index = Count++;

    public override string ToString() => "View" + Index;
}