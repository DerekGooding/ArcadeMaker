using ArcadeMaker.IDE.Items;
using System.ComponentModel;

namespace ArcadeMaker.IDE;

public partial class GameResourcePickerBox<T> : UserControl where T : GameItem
{
    public readonly ContextMenuStrip Menu = new ContextMenuStrip();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public T Resource
    {
        get;
        set
        {
            field = value;
            if (field != null)
                nameBox.Text = field.name;
            else
                nameBox.Text = defaultItemTitle;
        }
    } = null;

    public event EventHandler<T> SelectionChanged;

    private string defaultItemTitle;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DefaultItemTitle
    {
        get => defaultItemTitle;
        set
        {
            defaultItemTitle = value;
            if (noResItem != null)
                noResItem.Text = value;
            if (Resource == null)
                nameBox.Text = value;
        }
    }

    public GameResourcePickerBox(string defaultItemTitle = "<None>", T defaultRes = null)
    {
        InitializeComponent();
        this.defaultItemTitle = defaultItemTitle;
        nameBox.Text = defaultItemTitle;
        Resource = defaultRes;
    }

    private ToolStripMenuItem noResItem = null;

    private void LoadMenu()
    {
        try
        {
            Menu.Items.Clear();

            noResItem = new ToolStripMenuItem(defaultItemTitle);
            noResItem.Click += (s, e) => SelectResource(null);
            Menu.Items.Add(noResItem);

            LoadFolderMenu(Global.form1.GetProjectStruct<T>(), null);
        }
        catch (Exception ex)
        {
        }
    }

    private void LoadFolderMenu(ProjectFolderTreeStruct<T> folder, ToolStripMenuItem menuItem)
    {
        foreach (var str in folder.Structs)
        {
            if (str is ProjectItemTreeStruct<T> resStr)
            {
                T res = resStr.Item as T;

                ToolStripMenuItem item = new ToolStripMenuItem(res.name);
                res.NameChanged += (s, e) => item.Text = res.name;

                if (typeof(T) == typeof(GameSprite) && (res as GameSprite).image != null)
                    item.Image = (res as GameSprite).image;
                else if (typeof(T) == typeof(GameObject) && (res as GameObject).sprite?.image != null)
                    item.Image = (res as GameObject).sprite?.image;
                if (typeof(T) == typeof(GameBackground) && (res as GameBackground).image != null)
                    item.Image = (res as GameBackground).image;

                item.Click += (s, e) => SelectResource(res);

                if (menuItem != null)
                    menuItem.DropDownItems.Add(item);
                else
                    Menu.Items.Add(item);
            }
            else if (str is ProjectFolderTreeStruct<T> folderStr)
            {
                ToolStripMenuItem newFolder = new ToolStripMenuItem(folderStr.Name);
                newFolder.Image = null;// Properties.Resources.folder;
                LoadFolderMenu(folderStr, newFolder);

                if (menuItem != null)
                    menuItem.DropDownItems.Add(newFolder);
                else
                    Menu.Items.Add(newFolder);
            }
        }
    }

    private void ShowMenu(Control ctrl, Point position) => Menu.Show(this, position);

    private void nameBox_MouseClick(object sender, MouseEventArgs e) => ShowMenu(nameBox, e.Location);

    private void SelectResource(T res)
    {
        Resource = res;
        SelectionChanged?.Invoke(this, Resource);
        if (Resource != null)
            nameBox.Text = Resource.name;
        else
            nameBox.Text = defaultItemTitle;
    }

    private void menuBtn_Click(object sender, EventArgs e) => ShowMenu(toolStrip1, toolStrip1.Location);

    private void GameResourcePickerBox_Load(object sender, EventArgs e)
    {
        Menu.Items.Clear();
        LoadMenu();
        if (Environment.project != null)
        {
            Environment.project.items.CollectionChanged += (s, ea) =>
            {
                if (Environment.project.items.Contains(Resource))
                    Resource = null;
                LoadMenu();
            };
        }
    }
}

public class GameObjectPickerBox : GameResourcePickerBox<GameObject>;

public class GameSpritePickerBox : GameResourcePickerBox<GameSprite>;

public class GameBackgroundPickerBox : GameResourcePickerBox<GameBackground>;