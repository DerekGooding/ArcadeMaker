using ArcadeMaker.Core.Resources.Serializeables;
using ArcadeMaker.IDE.Items;

namespace ArcadeMaker.IDE;

public partial class ObjectEditor : Form
{
    private GameObject obj = null;

    public GameSprite sprite
    {
        get
        {
            return obj.sprite;
        }
    }

    public ObjectEditor(GameObject obj)
    {
        InitializeComponent();

        this.obj = obj;
        if (obj == null)
        {
            throw new ArgumentNullException("obj");
        }

        // update name box when object is renamed
        obj.NameChanged += (s, e) =>
        {
            if (!renaming)
                nameBox.Text = e.newName;
        };

        nameBox.Text = obj.name;
    }

    private bool renaming = false;

    private void nameBox_TextChanged(object sender, EventArgs e)
    {
        renaming = true;
        try
        {
            obj.name = nameBox.Text;
            if (nameBox.BackColor == Color.Red)
                nameBox.BackColor = Color.White;
        }
        catch
        {
            nameBox.BackColor = Color.Red;
        }
        renaming = false;
    }

    private void okBtn_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ObjectEditor_Load(object sender, EventArgs e)
    {
        PropertiesModifier.Init(obj);
        foreach (EventScripts ev in obj.EventScripts)
        {
            var item = ev.Event;
            eventsListView.Items.Add(item);
        }
        solidBox.Checked = obj.solid;
        depthBox.Value = obj.depth;
        Environment.project.items.CollectionChanged += (s, ea) =>
        {
            LoadSpriteBox();
        };
        SelectEventDialog.EventSelected += (s, ea) =>
        {
            if (!obj.EventScripts.Any(sc => sc.Event == ea))
            {
                var evscripts = new EventScripts(ea, []);
                obj.EventScripts.Add(evscripts);
                eventsListView.Items.Add(ea);
            }
            else
            {
                eventsListView.SelectedItem = ea;
            }
        };
        LoadSpriteBox();
        parentBox.Resource = obj.parent;
        parentBox.DefaultItemTitle = "<No Parent>";
    }

    private void LoadSpriteBox()
    {
        spriteBox.DefaultItemTitle = "<No Sprite>";
        if (sprite != null)
            spriteBox.Resource = sprite;
    }

    private SelectEventDialog SelectEventDialog = new SelectEventDialog();

    private void addEventBtn_Click(object sender, EventArgs e)
    {
        SelectEventDialog.ShowDialog();
    }

    private void solidBox_CheckedChanged(object sender, EventArgs e)
    {
        obj.solid = solidBox.Checked;
    }

    private bool firstEdit = true;
    //private void fullCodeBtn_Click(object sender, EventArgs e)
    //{
    //    /*
    //    Form editForm = new Form {  };
    //    RichTextBox scriptBox = new RichTextBox { Text = obj.part2script.Replace("classname", obj.name), AcceptsTab = true,
    //        Size = new Size(editForm.Size.Width, editForm.Size.Height - 20),
    //        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
    //    Button okEditBtn = new Button { Text = "OK", Location = new Point(5, scriptBox.Size.Height + 5), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
    //    okEditBtn.Click += (s, ea) =>
    //    {
    //        editForm.Close();
    //    };
    //    editForm.FormClosed += (s, ea) =>
    //    {
    //        obj.part2script = scriptBox.Text;
    //    };
    //    editForm.Controls.Add(scriptBox);
    //    editForm.Controls.Add(okEditBtn);
    //    editForm.ShowDialog();
    //    */
    //    if (eventsListView.SelectedItem == null)
    //    {
    //        MessageBox.Show("Select an event to edit its script.");
    //        return;
    //    }
    //    if (firstEdit)
    //    {
    //        firstEdit = false;
    //        //obj.Script = obj.Script;
    //    }
    //    ScriptEditor editor = new(eventsListView.SelectedItem as EventScript, classname: obj.name);
    //    editor.Owner = this;
    //    editor.OKClicked += (s, script) =>
    //    {
    //        if (eventsListView.SelectedItem?.ToString() == ObjectEvent.Create.ToString())
    //            obj.createEventScripts.Script = script;
    //        else if (eventsListView.SelectedItem?.ToString() == ObjectEvent.Step.ToString())
    //            obj.stepEventScripts.Script = script;
    //        else if (eventsListView.SelectedItem?.ToString() == ObjectEvent.Draw.ToString())
    //            obj.drawEventScripts.Script = script;
    //        else
    //            throw new NotImplementedException();
    //    };
    //    editor.ShowDialog();
    //}

    private void depthBox_ValueChanged(object sender, EventArgs e)
    {
        obj.depth = (int)depthBox.Value;
    }

    private void deleteEventBtn_Click(object sender, EventArgs e)
    {
        if (eventsListView.SelectedItems.Count == 0) return;

        try
        {
            var item = eventsListView.SelectedItems[0];
            ObjectEvent ev = (ObjectEvent)item;
            if (obj.EventScripts.FirstOrDefault(evs => evs.Event == ev) is EventScripts evscripts)
                obj.EventScripts.Remove(evscripts);
            eventsListView.Items.Remove(item);
        }
        catch (Exception ex)
        {
            string err = "Cannot delete or change event";
#if DEBUG
            err += "\n\nException:\n" + ex;
#endif
            MessageBox.Show(err, "Error");
        }
    }

    private void changeEventBtn_Click(object sender, EventArgs e)
    {
        deleteEventBtn_Click(null, null);
        addEventBtn_Click(null, null);
    }

    private void editSpriteBtn_Click(object sender, EventArgs e)
    {
        if (sprite != null)
        {
            try
            {
                sprite.editor.MdiParent = Global.form1;
                sprite.editor.Show();
            }
            catch { }
        }
        else
        {
            MessageBox.Show("Select sprite to edit");
        }
    }

    private void newSpriteBtn_Click(object sender, EventArgs e)
    {
        // create sprite by simulating click on the 'create sprite' button on form1
        var args = new CreateGameItemEventArgs(null);
        Global.form1.createSpriteBtn_Click(this, args);
        obj.sprite = args.GameItem as GameSprite;

        try
        {
            spriteBox.Resource = obj.sprite;
        }
        catch { }
    }

    private void parentBox_SelectionChanged(object sender, GameObject e)
    {
        if (e != obj)
            obj.parent = e;
        else
        {
            parentBox.Resource = obj.parent;
            MessageBox.Show("This will create a loop in parents.");
        }
    }

    private void spriteBox_SelectionChanged(object sender, GameSprite e)
    {
        obj.sprite = spriteBox.Resource;
    }

    private void addScriptBtn_Click(object sender, EventArgs e)
    {
        if (eventsListView.SelectedItem is not ObjectEvent ev)
        {
            MessageBox.Show("Select an event to add script to.");
            return;
        }

        EventScript script = new("");
        obj.EventScripts.FirstOrDefault(es => es.Event == ev)?.Scripts.Add(script);
        scriptsListView.Items.Add(script);
    }

    private void eventsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadScriptsListView();
    }

    private void LoadScriptsListView()
    {
        scriptsListView.Items.Clear(); // clear scripts view

        // if no event is selected, we're done here
        if (eventsListView.SelectedItem is not ObjectEvent ev)
            return;

        // load the scripts of the event to the scripts view
        scriptsListView.Items.AddRange(obj.EventScripts.FirstOrDefault(es => es.Event == ev)?.Scripts.ToArray() ?? []);
    }

    private void scriptsListView_DoubleClick(object sender, EventArgs e)
    {
        if (scriptsListView.SelectedItem is not EventScript script)
            return;

        ScriptEditor editor = new(script);
        editor.Owner = this;

        editor.OKClicked += (s, e) =>
        {
            script.Script = e;
            scriptsListView.Update(); // to update script desc
        };

        editor.ShowDialog();
    }

    private void deleteScriptBtn_Click(object sender, EventArgs e)
    {
        if (eventsListView.SelectedItem is not ObjectEvent ev)
            return;

        if (scriptsListView.SelectedItem is not EventScript script)
            return;

        obj.EventScripts.FirstOrDefault(ess => ess.Event == ev)?.Scripts.Remove(script);
        scriptsListView.Items.Remove(script);
    }

    private void scriptsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        scriptsListView.Invalidate();
    }

    private void scriptsListView_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        Font font = e.Font!;

        // draw rectangle
        //if (scriptsListView.SelectedIndex == e.Index)
        //    e.Graphics.FillRectangle(Brushes.RoyalBlue, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, font.Height);
        e.DrawBackground();

        bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        // draw row index
        e.Graphics.DrawString((e.Index + 1).ToString(), font, Brushes.Red, new PointF(0, e.Bounds.Y));

        // get the item
        object? item = scriptsListView.Items[e.Index];

        // draw the item text
        e.Graphics.DrawString(item?.ToString(), font, selected ? Brushes.White : Brushes.Black, new PointF(30, e.Bounds.Y));
    }
}