namespace Exp;

public static class ValueHelper
{
    public const string tbool = "bool", tchar = "char", tnum = "number", tfunc = "function", tinst = "instance";

    internal static void Unexpected(string exp, string rec)
    {
        Interpreter.Activated.ThrowRuntime($"A value of type {exp} was expected, but {rec} received.", RuntimeException.INVALID_ARGUMENT);
    }
}