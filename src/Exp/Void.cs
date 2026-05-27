//using Exp.Compiler;

namespace Exp;

// when a func returns instance of this, it means it didn't return a value
public sealed class Void : IValue
{
    public string TypeName => "void";
    public static Void Return { get; } = new();

    private Void()
    { }

    public override string ToString() => TypeName;
}