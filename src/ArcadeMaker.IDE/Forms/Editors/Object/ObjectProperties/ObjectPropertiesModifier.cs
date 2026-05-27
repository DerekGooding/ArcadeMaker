using ArcadeMaker.IDE.Items;
using System.Data;

namespace ArcadeMaker.IDE.Editors.Object.ObjectProperties
{
    public partial class ObjectPropertiesModifier : UserControl
    {
        public GameObject GameObject { get; private set; }

        public ObjectPropertiesModifier(GameObject? gameObject = null)
        {
            InitializeComponent();

            if (gameObject != null)
            {
                Init(gameObject);
            }
        }

        public ObjectPropertiesModifier()
        {
            InitializeComponent();
        }

        public void Init(GameObject gameObject)
        {
            this.GameObject = gameObject;

            foreach (var pro in GameObject.ExtraProperties)
            {
                LoadProperty(pro);
            }
        }

        private readonly Dictionary<CheckBox, ObjectPropertyModifier> propertySelectors = [];
        private const int modifiersSpacing = 5;
        private int lastModifierY = modifiersSpacing;

        private void LoadProperty(IDEObjectProperty pro)
        {
            ObjectPropertyModifier modifier = new(pro);
            modifier.Location = new(20, lastModifierY);
            modifier.Size = new(panel.Size.Width - modifier.Location.X, modifier.Size.Height);

            panel.Controls.Add(modifier);
            CheckBox selectorBox = new() { Location = new(5, lastModifierY) };
            selectorBox.CheckedChanged += (s, e) => { DeleteBtn.Enabled = propertySelectors.Any(pair => pair.Key.Checked); };
            propertySelectors.Add(selectorBox, modifier);
            panel.Controls.Add(selectorBox);
            lastModifierY += modifier.Size.Height + modifiersSpacing;
        }

        private void ObjectPropertiesModifier_Load(object sender, EventArgs e)
        {
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            IDEObjectProperty pro = new(GameObject);
            int i = 1;
            string name;
            do
            {
                name = "property" + i++;
            } while (!pro.IsValidName(name));
            pro.Name = name;

            GameObject.ExtraProperties.Add(pro);
            LoadProperty(pro);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            foreach (var pair in propertySelectors.Where(pair => pair.Key.Checked))
            {
                GameObject.ExtraProperties.Remove(pair.Value.Property);
                panel.Controls.Remove(pair.Value);
                panel.Controls.Remove(pair.Key);
                lastModifierY -= pair.Value.Size.Height + modifiersSpacing;
                pair.Value.Dispose();
                pair.Key.Dispose();
            }
            DeleteBtn.Enabled = false;
        }
    }
}