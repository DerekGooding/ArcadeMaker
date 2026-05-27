using System.Text.RegularExpressions;

namespace ArcadeMaker.IDE.IntelliSense;

public static class Translator
{
    public static string CleanCode(string code)
    {
        // Remove single-line comments
        code = Regex.Replace(code, @"\/\/.*$", "", RegexOptions.Multiline);

        // Remove multi-line comments
        code = Regex.Replace(code, @"\/\*[\s\S]*?\*\/", "");

        // Replace string literals with an empty string
        code = Regex.Replace(code, @"(""[^""\\\n]*(?:\\.[^""\\\n]*)*"")|(@""[^""]*"")", "\"\"");

        // Remove unnecessary whitespace
        code = Regex.Replace(code, @"\s+", " ");
        code = code.Replace("\n ", "\n");
        code = code.Replace(" \n", "\n");
        code = code.Trim();

        return code;
    }

    public static TranslatorItem[] TranslateItems(string src)
    {
        List<TranslatorItem> items = new List<TranslatorItem>();

        int pos = 0;
        Func<string> NextWord = new Func<string>(() =>
        {
            string word = "";
            while (pos < src.Length && src[pos] == ' ')
                pos++;
            while (pos < src.Length && src[pos] != ' ')
            {
                word += src[pos];
                pos++;
            }
            return word;
        });

        bool inContext = false;
        Type contextType = null;
        bool contextStatic = false;
        bool contextConst = false;
        string contextHolds = null;
        AccessModifier contextModifier = AccessModifier.None;
        string contextName = null;
        string contextReturnType;
        int contextOpenBrackets = 0, contextCloseBrackets = 0;
        string contextSrc = "";

        while (pos < src.Length)
        {
            // skip comments & strings
            {
                // skip comments
                if (src[pos] == '/' && pos + 1 < src.Length && src[pos + 1] == '*')
                {
                    pos += 2;
                    while (!(pos < src.Length && src[pos] == '*' && pos + 1 < src.Length && src[pos + 1] == '/'))
                        pos++;
                    pos += 2;
                }
                else if (src[pos] == '/' && pos + 1 < src.Length && src[pos + 1] == '/')
                {
                    pos += 2;
                    while (pos < src.Length && src[pos] != '\n')
                        pos++;
                    pos++;
                }
                // skip strings
                else if (src[pos] == '@' && pos + 1 < src.Length && src[pos + 1] == '\"')
                {
                    pos += 2;
                    while (pos < src.Length && src[pos] != '\"')
                        pos++;
                    pos++;
                }
                else if (src[pos] == '\"')
                {
                    bool esc = false;
                    pos++;
                    while (pos < src.Length && src[pos] != '\"' && !esc)
                    {
                        esc = src[pos] == '\\';
                        pos++;
                    }
                    pos++;
                }
            }

            string word = null;
            if (contextType == null)
                word = NextWord();

            if (inContext)
            {
                char c = src[pos];
                if (c == '{')
                    contextOpenBrackets++;
                else if (c == '}')
                {
                    contextCloseBrackets++;
                    if (contextCloseBrackets == contextOpenBrackets)
                    {
                        TranslatorItem item = null;

                        if (contextType == typeof(Namespace))
                        {
                            var nitems = TranslateItems(contextSrc);
                            foreach (TranslatorItem nitem in nitems)
                            {
                                if (!(nitem is NamespaceItem))
                                    throw new Exception("Illegal item in namespace\n\nItem: " + nitem);
                            }
                            item = new Namespace(contextName, nitems as NamespaceItem[]);
                        }
                        else if (contextType == typeof(Class))
                        {
                            var citem = TranslateItems(contextSrc);
                            foreach (TranslatorItem nitem in citem)
                            {
                                if (!(nitem is NamespaceItem))
                                    throw new Exception("Illegal item in namespace\n\nItem: " + nitem);
                            }
                            item = new Class(contextName, contextModifier, citem as ClassItem[], contextStatic);
                        }
                        else if (contextType == typeof(Property))
                        {
                        }

                        if (item == null)
                            throw new Exception("Translate error: Unknown type\n\nType name: " + contextType);

                        items.Add(item);

                        contextOpenBrackets = 0;
                        contextCloseBrackets = 0;
                        contextType = null;
                        contextName = null;
                        contextSrc = "";
                        inContext = false;
                    }
                }
                else
                {
                    contextSrc += c;
                }
            }

            // check access modifier, static state and constant
            if (word == "public")
            {
                contextModifier = AccessModifier.Public;
            }
            else if (word == "private")
            {
                contextModifier = AccessModifier.Private;
            }
            else if (word == "protected")
            {
                if (contextModifier == AccessModifier.Private)
                    contextModifier = AccessModifier.PrivateProtected;
                else
                    contextModifier = AccessModifier.Protected;
            }
            else if (word == "internal")
            {
                if (contextModifier == AccessModifier.Protected)
                    contextModifier = AccessModifier.ProtectedInternal;
                else
                    contextModifier = AccessModifier.Internal;
            }
            else if (word == "static")
            {
                contextStatic = true;
            }
            else if (word == "const")
            {
                contextConst = true;
            }

            // check type
            else if (word == "namespace")
            {
                inContext = true;
                contextType = typeof(Namespace);
                contextName = NextWord();
            }
            else if (word == "class")
            {
                contextType = typeof(Class);
                contextName = NextWord();
            }
            else
            {
                contextType = typeof(ClassItem);
            }

            if (contextType != null)
            {
                while ((char.IsWhiteSpace(src[pos]) || src[pos] == '\n') && pos < src.Length - 1)
                    pos++;
                if (src[pos] == '<')
                {
                    contextHolds = NextWord();
                }
                else if (src[pos] == '(' && contextType == typeof(ClassItem))
                {
                    contextType = typeof(Method);
                    MethodArgument[] args = ReadMethodArguments(src, pos, out pos);
                }
                else if (src[pos] == '{' && contextType == typeof(ClassItem))
                {
                    contextType = typeof(Property);
                }
                else if (char.IsLetterOrDigit(src[pos]) || src[pos] == '_' && contextType == typeof(ClassItem))
                {
                    pos--;
                    contextReturnType = NextWord();
                    contextName = NextWord();
                }
                else
                    pos--;
            }

            pos++;
        }

        return items.ToArray();
    }

    private static MethodArgument[] ReadMethodArguments(string src, int pos, out int cnt)
    {
        cnt = pos;
        List<MethodArgument> args = new List<MethodArgument>();
        string argName = null, argType = null, argDefVal = null;

        string word = "";

        while (src.Contains("//"))
        {
            int sind = src.IndexOf("//");
            if (src.IndexOf('\n', sind) > 0)
                src = src.Remove(sind, src.IndexOf('\n') - sind);
            else
                src = src.Remove(sind);
        }
        while (src.Contains("/*"))
        {
            int sind = src.IndexOf("/*");
            if (src.IndexOf("*/", sind) > 0)
                src = src.Remove(sind, src.IndexOf("*/") - sind);
            else
                src = src.Remove(sind);
        }

        while (true)
        {
            while (char.IsLetterOrDigit(src[pos]) || src[pos] == '_')
            {
                word += src[pos];
            }
            if (word.Length > 0)
            {
                if (argDefVal != null)
                {
                    argDefVal = word;
                }
                if (word == "out" || word == "ref" || word == "param")
                    continue;
                else if (argType == null)
                    argType = word;
                else if (argName == null)
                    argName = word;
                else
                    throw new Exception("Unexpected argument word");
                word = "";
            }
            if (src[pos] == '=')
            {
                argDefVal = "";
            }
            cnt++;
        }
    }
}

public class TranslatorItem
{
    public string Name { get; private set; }
    public AccessModifier AccessModifier { get; private set; }

    public TranslatorItem(string name, AccessModifier accessModifier)
    {
        if (!name.IsLegallName())
            throw new ArgumentException("Illegal item name", "name");
        Name = name;
        AccessModifier = accessModifier;
    }
}

public class Namespace : TranslatorItem
{
    public List<NamespaceItem> Items { get; private set; }

    public Namespace(string name, NamespaceItem[] items) : base(name, AccessModifier.None)
    {
        if (items == null)
            throw new ArgumentNullException("items");
        Items = items.ToList();
    }
}

public class NamespaceItem : TranslatorItem
{
    public NamespaceItem(string name, AccessModifier accessModifier) : base(name, accessModifier)
    {
        if (accessModifier == AccessModifier.Private || accessModifier == AccessModifier.Protected || accessModifier == AccessModifier.ProtectedInternal)
        {
            throw new ArgumentException("Illegal class modifier", "accessModifier");
        }
    }
}

public class Class : TranslatorItem
{
    public List<ClassItem> Items { get; private set; }
    public bool IsStatic { get; private set; }

    public Class(string name, AccessModifier accessModifier, ClassItem[] items, bool isStatic = false) : base(name, accessModifier)
    {
        if (items == null)
            throw new ArgumentNullException("items");
        Items = items.ToList();
        IsStatic = isStatic;
    }
}

public class ClassItem : TranslatorItem
{
    public string Type { get; private set; }
    public bool IsStatic { get; private set; }

    public ClassItem(string name, AccessModifier accessModifier, string type, bool isStatic = false) : base(name, accessModifier)
    {
        if (!Actions.IsLegallName(type))
            throw new ArgumentException("Ilegall type for method or property", "type");
        Type = type;
        IsStatic = isStatic;
    }
}

public class Property : ClassItem
{
    public AccessModifier Get { get; set; }
    public AccessModifier Set { get; private set; }

    public Property(string name, AccessModifier accessModifier, string type, AccessModifier get = AccessModifier.Public, AccessModifier set = AccessModifier.Public) : base(name, accessModifier, type)
    {
        Get = get;
        Set = set;
    }
}

public class Method : ClassItem
{
    public List<MethodArgument> Arguments { get; private set; }

    public Method(string name, AccessModifier accessModifier, string returnType, MethodArgument[] arguments) : base(name, accessModifier, returnType)
    {
        if (arguments == null)
            throw new ArgumentNullException("arguments");
        Arguments = arguments.ToList();
    }
}

public class MethodArgument
{
    public string Name { get; private set; }
    public string Type { get; private set; }

    public bool HasDefaultValue { get; private set; }

    public MethodArgument(string name, string type, bool hasDefaultValue = false)
    {
        if (!name.IsLegallName())
            throw new ArgumentException("Ilegall name for argument", "name");
        if (!type.IsLegallName())
            throw new ArgumentException("Ilegall name for type", "type");
        Name = name;
        Type = type;
        HasDefaultValue = hasDefaultValue;
    }
}

public enum AccessModifier
{
    Private,
    Protected,
    Public,
    Internal,
    PrivateProtected,
    ProtectedInternal,

    None
}

internal static class Actions
{
    public static bool IsLegallName(this string name)
    {
        if (name == null)
            return false;
        return true;
    }
}