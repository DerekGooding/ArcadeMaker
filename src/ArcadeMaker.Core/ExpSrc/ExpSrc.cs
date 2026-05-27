using Exp;
using Exp.Spans;
using System.Reflection;

namespace ArcadeMaker.Core.ExpSrc
{
    public static class ExpSrc
    {
        public const string EngineNamespace = "ArcadeMaker";
        public const string GameNamespace = "game";

        public const string CURRENT_VIEW_INDEX_ARG_NAME = "currentViewIndex";

        public static HashSet<string> GlobalUsings { get; } = ["system", EngineNamespace];

        public static InstanceScriptDocument CreateInstanceScriptDocument(string name, ClassDefSpan def, string script, params string[] args)
        {
            InstanceScriptDocument doc = new(name, def, script, args);
            doc.Namespace = GameNamespace;
            doc.Usings.AddRange(GlobalUsings);
            return doc;
        }

        public static IEnumerable<Type> GetEnums(Assembly? assembly = null)
        {
            List<Type> types = [];

            // get all enums in this assembly marked with [ExpEnum]
            foreach (var type in (assembly ?? typeof(ExpSrc).Assembly).GetTypes())
            {
                if (type.IsEnum && type.GetCustomAttribute<ExpEnumAttribute>() != null)
                    types.Add(type);
            }

            return types;
        }
    }
}