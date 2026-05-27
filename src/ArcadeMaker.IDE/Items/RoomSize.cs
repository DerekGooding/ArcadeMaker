namespace ArcadeMaker.IDE.Items;

public class RoomSize(int width, int height)
{
    public int width = width, height = height;

    public Size ToFormSize() => new Size(width, height);
}
