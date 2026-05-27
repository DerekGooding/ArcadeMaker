namespace ArcadeMaker.Core.Resources;

public class TextureAtlasMap
{
    public class Item
    {
        public string SpriteName { get; set; }
        public int ImageIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }

    public string AtlasFilePath { get; set; }
    public Item[] Items { get; set; }
}