using ArcadeMaker.Core.Resources;
using ArcadeMaker.Core.Resources.Serializeables;
using Exp;
using Exp.Spans;

namespace ArcadeMaker.Core.Models
{
    public class ObjectModel(string name, Sprite? sprite, ObjectEventScripts eventScripts, ObjectProperty[] extraProperties) : IModel
    {
        public string Name => name;
        public Sprite? Sprite => sprite;
        public ObjectEventScripts EventScripts => eventScripts;
        public ClassDefSpan Class { get; } = GetClass(name, extraProperties);
        public (int Depth, bool Visible, bool Solid) InitValues { get; set; }
        public ObjectProperty[] ExtraProperties => extraProperties;

        public static ClassDefSpan GetClass(string name, ObjectProperty[] extraProps)
        {
            // create properties
            List<Property> props = [];
            foreach (var property in Runtime.Instance.csProperties)
            {
                props.Add(new(def: null /*(assigned at ClassDefSpan's ctor)*/, false, property.Key.Name.StartWithLowerCase(), false, false));
            }
            foreach (var property in extraProps)
            {
                props.Add(new(def: null /*(assigned at ClassDefSpan's ctor)*/, property.Constant, property.Name, property.Private, false));
            }

            return new ClassDefSpan(name, [.. props], []);
        }
    }

    public class ObjectEventScripts(InstanceScriptDocument[]? create, InstanceScriptDocument[]? step, InstanceScriptDocument[]? draw)
    {
        public List<InstanceScriptDocument> Create { get; } = new(create ?? []);
        public InstanceScriptDocument[]? Step => step;
        public InstanceScriptDocument[]? Draw => draw;
    }
}