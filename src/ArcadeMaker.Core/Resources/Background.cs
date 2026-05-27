namespace ArcadeMaker.Core.Resources;

public class Background(string name, string filePath) : ISetsID
{
    private static int idCounter = 0;
    public int ID { get; } = idCounter++;

    public string Name => name;
    public string FilePath => filePath;
}