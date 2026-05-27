using System.Xml.Serialization;

namespace ArcadeMaker.IDE.Items;

public class GameItem
{
    [XmlIgnore]
    public int treeImageIndex = -1;

    [XmlIgnore]
    public TreeNode treeNode = null;

    [field: XmlIgnore]
    public string name
    {
        get;
        set
        {
            if (value.IsPossibleName(field))
            {
                if (value != field)
                {
                    GameItemNameChangedEventArgs args = new GameItemNameChangedEventArgs(field, value);
                    field = value;
                    var handler = NameChanged;
                    handler?.Invoke(this, args);
                }
            }
            else
                throw new Exception("Item name must be uniq and contain English letters and digits only");
        }
    }

    public event EventHandler<GameItemNameChangedEventArgs> NameChanged;

    public event EventHandler<Form> getEditor;

    [XmlIgnore]
    public Form editor
    {
        get
        {
            if (Editor == null || editorClosed)
            {
                var handler = getEditor;
                handler?.Invoke(this, null);
            }
            return Editor;
        }
        set
        {
            Editor = value;
            if (!value.IsDisposed)
                editorClosed = false;
            if (value != null)
            {
                Editor.FormClosed += (s, e) => editorClosed = true;
                Editor.Owner = Global.form1;
                Editor.ShowInTaskbar = false;
                Editor.ShowIcon = false;
                //Editor.SetCloseAsHide();
            }
        }
    }

    [XmlIgnore]
    protected Form Editor;

    [XmlIgnore]
    protected bool editorClosed = false;

    public GameItem(string name) => this.name = name;

    // used by XML seralizition
    public GameItem() : this(Global.GenerateRandomGameItemName()) { }

    public override string ToString() => name;
}

public class GameItemNameChangedEventArgs(string oldName, string newName) : EventArgs
{
    public string oldName { get; } = oldName;
    public string newName { get; } = newName;
}

public static class SeperatingAxisTheorem
{
    public static bool AreRectanglesColliding(double x1, double y1, double w1, double h1, double angle1, double ox1, double oy1, double x2, double y2, double w2, double h2, double angle2, double ox2, double oy2)
    {
        // convert angles to radians
        angle1 *= Math.PI / 180;
        angle2 *= Math.PI / 180;

        // calculate the four corners of each rectangle
        var r1Corners = CalculateRotatedRectCorners(x1, y1, w1, h1, angle1, ox1, oy1);
        var r2Corners = CalculateRotatedRectCorners(x2, y2, w2, h2, angle2, ox2, oy2);

        // calculate the axes to project onto
        var axes = CalculateAxes(r1Corners, r2Corners);

        // check for overlap on each axis
        for (var i = 0; i < axes.Length; i++)
        {
            var r1Projection = ProjectOntoAxis(r1Corners, axes[i]);
            var r2Projection = ProjectOntoAxis(r2Corners, axes[i]);
            if (!IsOverlap(r1Projection, r2Projection))
            {
                return false;
            }
        }

        return true;
    }

    // helper method to calculate the corners of a rotated rectangle
    private static double[] CalculateRotatedRectCorners(double x, double y, double width, double height, double angle, double originX, double originY)
    {
        var radians = angle * Math.PI / 180;
        var cos = Math.Cos(radians);
        var sin = Math.Sin(radians);
        var dx = x - originX;
        var dy = y - originY;
        var x1 = dx * cos - dy * sin + originX;
        var y1 = dx * sin + dy * cos + originY;
        var x2 = x1 + width * cos;
        var y2 = y1 + width * sin;
        var x3 = x2 - height * sin;
        var y3 = y2 + height * cos;
        var x4 = x1 - height * sin;
        var y4 = y1 + height * cos;
        return new double[] { x1, y1, x2, y2, x3, y3, x4, y4 };
    }

    // helper method to calculate the axes to project onto
    private static double[] CalculateAxes(double[] r1Corners, double[] r2Corners)
    {
        var axes = new double[4];
        axes[0] = CalculateAxis(r1Corners[0], r1Corners[1], r1Corners[2], r1Corners[3]); // top edge of r1
        axes[1] = CalculateAxis(r1Corners[2], r1Corners[3], r1Corners[4], r1Corners[5]); // right edge of r1
        axes[2] = CalculateAxis(r2Corners[0], r2Corners[1], r2Corners[2], r2Corners[3]); // top edge of r2
        axes[3] = CalculateAxis(r2Corners[2], r2Corners[3], r2Corners[4], r2Corners[5]); // right edge of r2
        return axes;
    }

    // helper method to calculate an axis perpendicular to two points
    private static double CalculateAxis(double x1, double y1, double x2, double y2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return -dy; // perpendicular to the line connecting the two points
    }

    // helper method to project a set of corners onto an axis
    private static double[] ProjectOntoAxis(double[] corners, double axis)
    {
        var min = Double.PositiveInfinity;
        var max = Double.NegativeInfinity;
        for (var i = 0; i < corners.Length; i += 2)
        {
            var dot = corners[i] * axis + corners[i + 1] * (axis == 0 ? 1 : 0); // handle axis = 0 case
            min = Math.Min(min, dot);
            max = Math.Max(max, dot);
        }
        return new double[] { min, max };
    }

    // helper method to check if two projections overlap
    private static bool IsOverlap(double[] p1, double[] p2) => !(p1[1] < p2[0] || p2[1] < p1[0]);
}