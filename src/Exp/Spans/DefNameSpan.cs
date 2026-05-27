namespace Exp.Spans;

internal class DefNameSpan : WordSpan
{
    internal string SpecificNs { get; }
    internal string Name { get; }
    internal ClassDefSpan Class { get; private set; }
    internal FuncDefSpan Func { get; }
    internal ExternClassDefSpan Extern { get; private set; }
    internal AttributeDefSpan Attr { get; private set; }
    internal bool IsUnknownItem { get; private set; }
    internal IDefinition Defination => Class ?? Func ?? (IDefinition)Extern ?? Attr;

    internal event EventHandler Resolved;

    internal bool CancelResolve { get; set; }
    internal static bool CancelResolveForNewOnes { get; set; }

    internal DefNameSpan(string specNs, string name, ScriptDocument doc, int docLoc, Interpreter compiler) : base(name)
    {
        (SpecificNs, Name, Document, DocumentLocation) = (specNs, name, doc, docLoc);
        void Resolve(object sender, EventArgs e)
        {
            if (CancelResolve)
                return;

            compiler ??= Interpreter.Activated;
            var defs = compiler.definations.Where(d => specNs == null ? d.Namespace == null || d.Namespace == Document.Namespace || (Document.Usings?.Contains(d.Namespace) == true) : d.Namespace == specNs);
            var matches = defs.Where(d => d.Name == name);

            // check ambiguous reference
            var def = matches.FirstOrDefault(d => d.Namespace == null);
            var matchesCount = matches.Count();
            if (specNs == null && matchesCount >= 2 && def == null)
            {
                const string advice = "Specify namespace to select the wanted one";
                if (matchesCount == 2)
                    compiler.Error($"'{name}' is an ambiguous reference between '{matches.First().FullName}' and '{matches.ElementAt(1).FullName}'. {advice}.", this);
                else if (matchesCount >= 3)
                    compiler.Error($"'{name}' is an ambiguous reference between '{matches.Map(m => m.FullName).ToString("', '")}'. {advice}.", this);
            }
            def ??= matches.FirstOrDefault();

            if (def is ClassDefSpan cls)
                Class = cls;
            else if (def is AttributeDefSpan attr)
                Attr = attr;
            else if (def is ExternClassDefSpan ext)
                Extern = ext;
            else
            {
                compiler.Error($"Unknown type '{FullText}'.", this);
                IsUnknownItem = true;
            }
            Resolved?.Invoke(this, null);
            compiler.CollectDefsCompleted -= Resolve;
        }
        ;

        if (!CancelResolveForNewOnes)
        {
            if (compiler.CollectedDefs)
                Resolve(compiler, null);
            else
                compiler.CollectDefsCompleted += Resolve;
        }
    }

    internal override string FullText => (SpecificNs == null ? "" : (SpecificNs + NamespaceSpecificationSpan.Symbol)) + Name;
}
