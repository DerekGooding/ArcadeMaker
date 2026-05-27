namespace Exp.Spans;

internal interface ILoopContext : IContext
{
    bool Break { get; set; }
    bool Continue { get; set; }
    string Counter { get; set; }
    string Id { get; set; }
}
