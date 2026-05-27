using Exp.Operations;

namespace Exp.Spans;

public interface IContext : IVarSystem
{
    //IVarSystem OuterVarSystem { get; }
    Span[] InnerSource { get; set; }

    IOperation[] Operations { get; set; }
    //IContext Context { get; set; }
}
