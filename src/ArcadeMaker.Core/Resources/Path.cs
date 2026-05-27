using ArcadeMaker.Core.ExpSrc;

namespace ArcadeMaker.Core.Resources;

public class Path(string name, double startX, double startY, Path.Step[] steps) : ISetsID
{
    public readonly record struct Step
    {
        public double Width { get; }
        public double Height { get; }
        public double Speed { get; }
        public double Direction { get; }
        public Step(double width, double height, double speed)
        {
            Width = width;
            Height = height;
            Speed = speed;

            // calculate direction
            Direction = Math.Formulas.AngleBetween(0, 0, Width, Height) - 90;
        }
    }

    public string Name { get; } = name;

    private static int idCounter = 0;
    public int ID { get; } = idCounter++;

    public double StartPositionX { get; } = startX;
    public double StartPositionY { get; } = startY;
    public Step[] Steps { get; } = steps;
}

[ExpEnum]
public enum PathEndAction
{
    Stop,
    Restart,
    Reverse
}