using ArcadeMaker.Core.Resources;
using System.Drawing;

namespace ArcadeMaker.Core.Models
{
    public class RoomModel(string name, string caption, int w, int h, Color backgroundColor, RoomInitMap initMap) : IModel, ISetsID
    {
        private static int idCounter = 0;
        public int ID { get; } = idCounter++;
        public string Name => name;
        public string Caption => caption;
        public RoomInitMap InitMap => initMap;
        public int Width => w;
        public int Height => h;
        public Color BackgroundColor => backgroundColor;
        public Background Background { get; init; }
        public List<RoomView> Views { get; } = [];
    }

    public class RoomInitMap(RoomInitMap.Item[] items)
    {
        public record struct Item(double X, double Y, ObjectModel Object);
        public Item[] Items => items;
    }

    public class RoomView(double x, double y)
    {
        public bool Visible { get; set; }
        public double X { get; private set; } = x;
        public double Y { get; private set; } = y;
        public double Width { get; set; }
        public double Height { get; set; }
        public int PortX { get; set; }
        public int PortY { get; set; }
        public int PortWidth { get; set; }
        public int PortHeight { get; set; }
        public ObjectModel? Following { get; set; }
        public double Follow_HBorder { get; set; }
        public double Follow_VBorder { get; set; }
        public double Follow_HSpeed { get; set; }
        public double Follow_VSpeed { get; set; }

        public event EventHandler? PositionChanged;

        public void SetPosition(double x, double y)
        {
            this.X = x;
            this.Y = y;
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}