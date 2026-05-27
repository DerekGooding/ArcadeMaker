using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ArcadeMaker.IDE.Scripting
{
    [XmlRoot(Roots.Main)]
    public class TypeMemberInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement(Roots.Summary)]
        public Description Summary { get; set; }

        [XmlElement(Roots.TypeParam)]
        public TypeParam[] TypeParams { get; set; }

        [XmlElement(Roots.Param)]
        public Param[] Params { get; set; }

        [XmlElement(Roots.Returns)]
        public Description Returns { get; set; }

        [XmlElement(Roots.Value)]
        public Description Value { get; set; }

        [XmlElement(Roots.Exception)]
        public Scripting.Exception[] Exceptions { get; set; }

        [XmlElement(Roots.InheritDoc)]
        public InheritDoc InheritDoc { get; set; }
    }

    public interface IDescription
    {
        Description Description { get; set; }
    }

    public class Description : IXmlSerializable
    {
        public interface IInnerElement;

        public List<string> TextSpans { get; set; } = new List<string>();
        public List<IInnerElement> InnerElements { get; set; } = new List<IInnerElement>();
        public List<int> InnerElementsIndices { get; set; } = new List<int>();

        public string GetContent(Func<Relation, string> ReadRelation, Func<InheritDoc, string> ReadInheritDoc)
        {
            string content = "";
            foreach (string span in TextSpans)
                content += span;

            string[] relationsTexts = new string[InnerElements.Count];
            for (int i = 0; i < InnerElements.Count; i++)
            {
                if (InnerElements[i] is Relation relation)
                    relationsTexts[i] = ReadRelation(relation) ?? "";
                else if (InnerElements[i] is ParamRef param)
                    relationsTexts[i] = param.Name ?? "";
                else if (InnerElements[i] is ISpacing spacing)
                    relationsTexts[i] = spacing.Text;
                else if (InnerElements[i] is CodeSpan code)
                    relationsTexts[i] = code.Code;
                else if (InnerElements[i] is Example example)
                    relationsTexts[i] = example.GetContent(ReadRelation, ReadInheritDoc);
                else if (InnerElements[i] is InheritDoc doc)
                    relationsTexts[i] = ReadInheritDoc(doc);
                else if (InnerElements[i] is List list)
                    relationsTexts[i] = list.GetContent(ReadRelation, ReadInheritDoc);
            }

            content = content.InsertRange(relationsTexts, InnerElementsIndices.ToArray());

            return content;
        }

        public void WriteXml(XmlWriter writer)
        {
        }

        public void ReadXml(XmlReader reader)
        {
            XElement element;
            try
            {
                element = XElement.Parse(reader.ReadOuterXml());
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error deserializing Scripting.Description:\n" + ex);
                //reader.Read();
                return;
            }

            int index = 0;
            foreach (XNode node in element.Nodes())
            {
                if (node is XText text)
                {
                    TextSpans.Add(text.Value);
                    index += text.Value.Length;
                }
                else if (node is XElement innerElement)
                {
                    using (XmlReader relationReader = innerElement.CreateReader())
                    {
                        try
                        {
                            Type type;
                            if (innerElement.Name == Roots.Relation)
                                type = typeof(Relation);
                            else if (innerElement.Name == Roots.SeeAlsoRelation)
                                type = typeof(SeeAlsoRelation);
                            else if (innerElement.Name == Roots.ParamRef)
                                type = typeof(ParamRef);
                            else if (innerElement.Name == Roots.TypeParamRef)
                                type = typeof(TypeParamRef);
                            else if (innerElement.Name == Roots.Paragraph)
                                type = typeof(Paragraph);
                            else if (innerElement.Name == Roots.LineBreak)
                                type = typeof(LineBreak);
                            else if (innerElement.Name == Roots.CodeSpan)
                                type = typeof(CodeSpan);
                            else if (innerElement.Name == Roots.SingleLineCodeSpan)
                                type = typeof(SingleLineCodeSpan);
                            else if (innerElement.Name == Roots.Example)
                                type = typeof(Example);
                            else if (innerElement.Name == Roots.InheritDoc)
                                type = typeof(InheritDoc);
                            else if (innerElement.Name == Roots.List)
                                type = typeof(List);
                            else if (innerElement.Name == Roots.ListItem)
                                type = typeof(List.Item);
                            else
                                throw new System.Exception($"Unknown description inner element tag: <{innerElement.Name}>");

                            XmlSerializer serializer = new XmlSerializer(type);
                            IInnerElement relation = serializer.Deserialize(relationReader) as IInnerElement;
                            InnerElements.Add(relation);
                        }
                        catch (System.Exception ex)
                        {
                            InnerElements.Add(new Relation { Cref = innerElement.Attribute("cref")?.Value });
                            Debug.WriteLine(ex);
                        }
                        InnerElementsIndices.Add(index);
                    }
                }
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
    }

    [XmlRoot(Roots.TypeParam)]
    public class TypeParam : Param
    {
        protected override string Root { get; set; } = Roots.TypeParam;
    }

    [XmlRoot(Roots.Param)]
    public class Param : IXmlSerializable, IDescription
    {
        /// <summary>
        /// Override this with the root name of the inherit element
        /// </summary>
        protected virtual string Root { get; set; } = Roots.Param;

        public string Name { get; set; }

        public Description Description { get; set; }

        public void WriteXml(XmlWriter writer)
        {
        }

        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute("name");
            XmlSerializer serializer = new XmlSerializer(typeof(Description), new XmlRootAttribute(Root));
            Description = serializer.Deserialize(reader) as Description;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
    }

    [XmlRoot(Roots.Exception)]
    public class Exception
    {
        [XmlAttribute("cref")]
        public string Cref
        {
            get;
            set
            {
                field = value;
                if (value != null)
                {
                    if (value.Length > 2 && value[1] == ':')
                    {
                        field = value.Substring(2);
                    }
                }
            }
        }
    }

    [XmlRoot(Roots.Relation)]
    public class Relation : Description.IInnerElement
    {
        [XmlText]
        public string Text { get; set; }

        [XmlIgnore]
        public RelationKind Kind { get; set; }

        [field: XmlIgnore]
        [XmlAttribute("cref")]
        public string Cref
        {
            get;
            set
            {
                field = value;
                Kind = RelationKind.None;

                if (value != null)
                {
                    if (value.Length > 2 && value[1] == ':')
                    {
                        Kind = (RelationKind)value[0];
                        field = value.Substring(2);
                    }
                }
            }
        }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("langword")]
        public string LangWord { get; set; }
    }

    [XmlRoot(Roots.SeeAlsoRelation)]
    public class SeeAlsoRelation : Relation;

    [XmlRoot(Roots.TypeParamRef)]
    public class TypeParamRef : ParamRef;

    [XmlRoot(Roots.ParamRef)]
    public class ParamRef : Description.IInnerElement
    {
        [XmlAttribute("name")]
        public string Name;
    }

    public interface ISpacing
    {
        string Text { get; }
    }

    [XmlRoot(Roots.Paragraph)]
    public class Paragraph : Description.IInnerElement, ISpacing
    {
        public string Text { get; } = "\n\n";
    }

    [XmlRoot(Roots.LineBreak)]
    public class LineBreak : Description.IInnerElement, ISpacing
    {
        public string Text { get; } = "\n";
    }

    [XmlRoot(Roots.CodeSpan)]
    public class CodeSpan : Description.IInnerElement
    {
        [XmlText]
        public string Code { get; set; }
    }

    [XmlRoot(Roots.SingleLineCodeSpan)]
    public class SingleLineCodeSpan : CodeSpan;

    [XmlRoot(Roots.Example)]
    public class Example : Description, Description.IInnerElement;

    [XmlRoot(Roots.InheritDoc)]
    public class InheritDoc : Description.IInnerElement
    {
        [XmlIgnore]
        public RelationKind Kind { get; set; }

        [field: XmlIgnore]
        [XmlAttribute("cref")]
        public string Cref
        {
            get;
            set
            {
                field = value;
                Kind = RelationKind.None;

                if (value != null)
                {
                    if (value.Length > 2 && value[1] == ':')
                    {
                        Kind = (RelationKind)value[0];
                        field = value.Substring(2);
                    }
                }
            }
        }

        [field: XmlIgnore]
        [XmlAttribute("path")]
        public string Path
        {
            get;
            set
            {
                field = value;

                try
                {
                    List<PathNode> nodes = new List<PathNode>();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] == '/')
                            nodes.Add(new PathNode { NodeName = "" });
                        else if (value[i] == '[')
                        {
                            nodes.Last().SpecificAttributeName = "";
                            nodes.Last().SpecificAttributeValue = "";
                            int s;
                            bool setName = true;
                            for (s = i + 1; value[s] != ']'; s++)
                            {
                                if (s == i + 1)
                                {
                                    if (value[s] != '@')
                                        throw new System.Exception("Attribute name must begin with '@'");
                                    else
                                        continue;
                                }
                                if (setName)
                                {
                                    if (value[s] == ' ' || value[s] == '=')
                                        setName = false;
                                    else
                                    {
                                        nodes.Last().SpecificAttributeName += value[s];
                                        continue;
                                    }
                                }
                                if (value[s] == '=')
                                {
                                    int start = value.IndexOf('\'', s + 1) + 1;
                                    int length = value.IndexOf('\'', start + 1) - start;

                                    nodes.Last().SpecificAttributeValue = value.Substring(start, length);

                                    s = start + length + 1;
                                    char nextChar = value[s];
                                    if (nextChar != ']')
                                        throw new System.Exception($"Index {start + length}: Unexpected symbol '{nextChar}'");
                                    else
                                        break;
                                }
                                else if (value[s] == ' ')
                                    continue;
                            }
                            i = s + 1;
                        }
                        else
                        {
                            nodes.Last().NodeName += value[i];
                        }
                    }
                    PathNodes = nodes.ToArray();

                    string nodesText = "";
                    foreach (var node in PathNodes)
                    {
                        nodesText += node.NodeName;
                        if (node.SpecificAttributeName != null)
                        {
                            nodesText += ": " + node.SpecificAttributeName + " = " + node.SpecificAttributeValue;
                            nodesText += "\n\n";
                        }
                    }
                }
                catch (System.Exception ex)
                {
#if DEBUG
                    MessageBox.Show("[Debug Mode]\n" + ex);
#endif
                }
            }
        }

        public class PathNode
        {
            public string NodeName { get; set; }
            public string SpecificAttributeName { get; set; }
            public string SpecificAttributeValue { get; set; }
        }

        [XmlIgnore]
        public PathNode[] PathNodes { get; private set; }
    }

    [XmlRoot(Roots.List)]
    public class List : Description.IInnerElement
    {
        [XmlRoot(Roots.ListItem)]
        public class Item : Description.IInnerElement
        {
            [XmlElement(Roots.ListItemTerm)]
            public Description[] Terms { get; set; }

            [XmlElement(Roots.ListItemDescription)]
            public Description Description { get; set; }
        }

        public enum ListType
        {
            Error,

            Bullet,
            Number,
            Table
        }

        [XmlIgnore]
        public ListType Type { get; set; } = ListType.Error;

        [XmlAttribute("type")]
        public string TypeName
        {
            get => Type.ToString();
            set
            {
                try
                {
                    Type = (ListType)Enum.Parse(typeof(ListType), value, ignoreCase: true);
                }
                catch
                {
                    Type = ListType.Error;
                }
            }
        }

        [XmlElement(Roots.ListHeader)]
        public Item ListHeader { get; set; }

        [XmlElement(Roots.ListItem)]
        public Item[] Items { get; set; }

        public string GetContent(Func<Relation, string> ReadRelation, Func<InheritDoc, string> ReadInheritDoc)
        {
            if (Type == ListType.Error)
                return "";

            string ReadItem(Item item, bool endl = true)
            {
                if (item != null)
                {
                    string text = "";
                    if (item.Terms != null && item.Terms.Length >= 1)
                        text += item.Terms.First().GetContent(ReadRelation, ReadInheritDoc) + ": ";
                    if (item.Description != null)
                        text += item.Description.GetContent(ReadRelation, ReadInheritDoc);

                    return text + (endl ? "\n" : "");
                }
                return "";
            }

            string content = "";
            if (Type != ListType.Table)
            {
                content = ReadItem(ListHeader);

                string[] itemsTexts = new string[Items.Length];
                for (int i = 0; i < Items.Length; i++)
                    itemsTexts[i] = ReadItem(Items[i]);

                string itemSpacing = "  ";
                if (Type == ListType.Bullet)
                {
                    foreach (string item in itemsTexts)
                        content += itemSpacing + "• " + item;
                }
                else if (Type == ListType.Number)
                {
                    for (int i = 0; i < itemsTexts.Length; i++)
                        content += itemSpacing + (i + 1) + ": " + itemsTexts[i];
                }
            }
            else
            {
                List<string[]> rows = new List<string[]>();
                if (ListHeader != null)
                    rows.Add(ListHeader.Terms.ToArray(term => term.GetContent(ReadRelation, ReadInheritDoc)));
                foreach (Item item in Items)
                {
                    rows.Add(item.Terms.ToArray(term => term.GetContent(ReadRelation, ReadInheritDoc)));
                }
                content += '\n' + TextSharp.TextSharp.BuildTable(rows.ToArray());
            }

            return content;
        }
    }

    public enum RelationKind
    {
        None = 0,

        Namespace = 'N',
        Type = 'T',
        Field = 'F',
        Property = 'P',
        Method = 'M',
        Event = 'E',
        ErrorString = '!'
    }

    internal static class Roots
    {
        public const string Main = "member";
        public const string Summary = "summary";
        public const string TypeParam = "typeparam";
        public const string Param = "param";
        public const string Returns = "returns";
        public const string Value = "value";
        public const string Exception = "exception";
        public const string Relation = "see";
        public const string SeeAlsoRelation = "seealso";
        public const string TypeParamRef = "typeparamref";
        public const string ParamRef = "paramref";
        public const string Paragraph = "para";
        public const string LineBreak = "br";
        public const string CodeSpan = "code";
        public const string SingleLineCodeSpan = "c";
        public const string Example = "example";
        public const string InheritDoc = "inheritdoc";

        public const string List = "list";
        public const string ListHeader = "listheader";
        public const string ListItem = "item";
        public const string ListItemTerm = "term";
        public const string ListItemDescription = "description";
    }
}

namespace ArcadeMaker.IDE.TextSharp
{
    public static class TextSharp
    {
        /// <summary>
        /// Builds a string representation of a table with borders and padding.
        /// </summary>
        /// <param name="table">A 2D string array representing the table, where each inner array is a row and each element in the inner array is a cell value.</param>
        /// <returns>A string representing the table. Each cell value is padded with spaces to align with the column width. The first row values are centered, while the rest are left-aligned. Each row's height is determined by the cell with the most lines.</returns>
        public static string BuildTable(string[][] table)
        {
            // get widths and max lines
            int[] widths = new int[table[0].Length];
            int[] maxLines = new int[table.Length];
            for (int w = 0; w < widths.Length; w++)
            {
                int max = 0;
                for (int row = 0; row < table.Length; row++)
                {
                    if (table[row][w] == null)
                        table[row][w] = "null";
                    string[] lines = table[row][w].Split('\n');
                    if (lines.Length > maxLines[row])
                        maxLines[row] = lines.Length;
                    foreach (string line in lines)
                    {
                        if (line.Length > max)
                            max = line.Length;
                    }
                }
                widths[w] = max;
            }
            // create border
            string border = "+";
            for (int i = 0; i < widths.Length; i++)
            {
                for (int c = 0; c < widths[i] + 4; c++)
                    border += '-';
                border += '+';
            }
            // fill values
            string panel = border + '\n';
            for (int i = 0; i < table.Length; i++)
            {
                string[] vals = table[i];
                for (int line = 0; line < maxLines[i]; line++)
                {
                    for (int j = 0; j < vals.Length; j++)
                    {
                        string val = vals[j];
                        string[] lines = val.Split('\n');
                        string currentLine = line < lines.Length ? lines[line] : "";
                        string spaces = "  ";
                        string rect;
                        if (i > 0)
                        { // text alignment: left
                            rect = '|' + spaces + currentLine;
                            while (rect.Length < widths[j] + 5)
                                rect += ' ';
                        }
                        else
                        { // text alignment: center
                            for (int s = 0; s < (widths[j] / 2.0) - (currentLine.Length / 2.0); s++) spaces += ' ';
                            rect = '|' + spaces + currentLine;
                            if (rect.Length + spaces.Length > widths[j] + 5)
                            {
                                spaces = spaces.Substring(1);
                            }
                            else if (rect.Length + spaces.Length < widths[j] + 5)
                            {
                                spaces += ' ';
                            }
                            rect += spaces;
                        }
                        panel += rect;
                    }
                    panel += "|\n";
                }
                panel += border + '\n';
            }
            return panel;
        }
    }
}