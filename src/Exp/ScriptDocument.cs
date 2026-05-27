using Exp.Operations;
using Exp.Spans;

namespace Exp;

public class ScriptDocument
{
    public HashSet<ExpError> SettingsErrors { get; } = [];
    public string? Description { get; set; }
    public string Name { get; set; }
    public string Script { get; internal set; }
    public TextSpan[] TextSpans { get; private set; }
    internal Span[] CodeSpans { get; private protected set; }
    public bool IsPrepared => CodeSpans != null;
    public HashSet<string> Usings { get; } = [];
    public string? Namespace { get; set; }
    internal IOperation[] Operations { get; set; }

    protected ScriptDocument(string script, string name)
    {
        ArgumentNullException.ThrowIfNull(script);

        Name = name;
        Script = script;

        TextSpans = Spanner.GetTextSpans(Script);
        ReadDocSettings();
        foreach (var span in TextSpans)
            span.Doc = this;
    }

    private void ReadDocSettings()
    {
        ReadDocSettings(Name, TextSpans, out var updatedTextSpans, out var description, out var @namespace, out var usings, out var settingsErrors);
        (TextSpans, Description, Namespace) = (updatedTextSpans, description, @namespace);
        Usings.AddRange(usings);
        SettingsErrors.AddRange(settingsErrors);
    }

    public static void ReadDocSettings(string Name, TextSpan[] TextSpans, out TextSpan[] updatedTextSpans, out string? Description, out string? Namespace, out HashSet<string> Usings, out HashSet<ExpError> SettingsErrors)
    {
        updatedTextSpans = TextSpans;
        Description = null;
        Namespace = null;
        Usings = [];
        SettingsErrors = [];

        int spanIndex = 0, line = 0, col = 1;
        TextSpan? NextSpan()
        {
            if (spanIndex >= TextSpans.Length)
                return null;

            TextSpan span = TextSpans[spanIndex];

            // skip spaces
            while (span.type == SpanType.Space)
            {
                if (span.text == "\n")
                {
                    line++;
                    col = 1;
                }
                else
                {
                    col += span.text.Length;
                }

                spanIndex++;
                return NextSpan();
            }

            return TextSpans[spanIndex++];
        }

        bool anySettingsRead = false;

        // read doc description
        var next = NextSpan();
        if (next?.type == SpanType.Comment && next.text.StartsWith('/'))
        {
            Description = next.text[3..].Trim();
            anySettingsRead = true;
            next = NextSpan();
        }

        // read usings
        while (next?.text == UsingWordSpan.Keyword)
        {
            anySettingsRead = true;
            string? use = NextSpan()?.text;
            if (use == null)
                SettingsErrors.Add(new(Name, line, col, "Namespace name expected."));
            else if (!use.IsLiterallyValidName())
                SettingsErrors.Add(new(Name, line, col, $"Invalid namespace name '{use}'."));
            else
            {
                if (!Usings.Add(use))
                    SettingsErrors.Add(new(Name, line, col, $"Namespace '{use}' is already imported."));
            }
            next = NextSpan();
        }

        // read namespace
        if (next?.text == NamespaceWordSpan.Keyword)
        {
            anySettingsRead = true;
            string? ns = NextSpan()?.text;
            if (ns == null)
                SettingsErrors.Add(new(Name, line, col, "Namespace name expected."));
            else if (!ns.IsLiterallyValidName())
                SettingsErrors.Add(new(Name, line, col, $"Invalid namespace name '{ns}'."));
            else
            {
                if (Namespace != null)
                    SettingsErrors.Add(new(Name, line, col, $"Namespace is already declared as '{Namespace}'."));
                else
                    Namespace = ns;
            }

            if (NextSpan()?.text != ":")
            {
                SettingsErrors.Add(new(Name, line, col, "':' expected after namespace declaration."));
            }
        }

        if (anySettingsRead)
            updatedTextSpans = TextSpans[(spanIndex - 1)..];
    }

    public virtual bool TryPrepare(Interpreter compiler, out ExpError[] errors)
    {
        ArgumentNullException.ThrowIfNull(compiler);

        var errorsBefore = compiler.Errors.ToArray();

        CodeSpans = compiler.GetCodeSpans(TextSpans);
        Operations = compiler.ReadOperations(CodeSpans, null);

        errors = compiler.Errors.ToArray().Remove(err => errorsBefore.Contains(err)).AppendRange(SettingsErrors).ToArray();
        return errors.Length == 0;
    }

    public virtual void Run(Interpreter compiler)
    {
        ArgumentNullException.ThrowIfNull(compiler);
        compiler.Run(this);
    }

    public static ScriptDocument FromString(string script, string name)
    {
        return new ScriptDocument(script, name);
    }

    public static ScriptDocument FromFile(string path)
    {
        char endd = '\\';
#if ANDROID
        endd = '/';
#endif

        return new ScriptDocument(File.ReadAllText(path), path.Contains(endd) ? path[(path.LastIndexOf(endd) + 1)..] : path);
    }

    public static ScriptDocument[] FromFiles(string[] paths)
    {
        var docs = new ScriptDocument[paths.Length];
        for (int i = 0; i < paths.Length; i++)
            docs[i] = FromFile(paths[i]);
        return docs;
    }

    public override string ToString() => Name;
}
