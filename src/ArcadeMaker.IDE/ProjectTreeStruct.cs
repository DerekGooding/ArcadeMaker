using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE;

public class ProjectTreeStruct<T> where T : GameItem;

public class ProjectItemTreeStruct<T>(GameItem item) : ProjectTreeStruct<T> where T : GameItem
{
    public GameItem Item = item;
}

public class ProjectFolderTreeStruct<T>(string name, bool isBaseFolder) : ProjectTreeStruct<T> where T : GameItem
{
    public readonly List<ProjectTreeStruct<T>> Structs = [];

    public string Name = name;
    public bool IsBaseFolder = isBaseFolder;

    public ProjectFolderTreeStruct(string name, bool isBaseFolder, object[] structs) : this(name, isBaseFolder)
    {
        foreach (var obj in structs)
        {
            if (obj is ProjectTreeStruct<T> @struct)
                Structs.Add(@struct);
        }
    }
}