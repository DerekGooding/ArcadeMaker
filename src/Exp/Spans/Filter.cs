namespace Exp.Spans;

public static class Filter
{
    public static string[] Operators { get; } = ["=", "==", ">", "<", "!=", ">=", "<=", "!", "&", "|", "+", "-", "++", "--", "+=", "-=", "/", "*", "%", "=>", "->"];
    public static string[] Keywords { get; } = ["using", "namespace", "class", "func", "constructor", "private", "static", "if", "while", "for", "foreach", "basearray", "in", "from", "to", "is", "not", "throw", "true", "false", "new", "try", "catch", "when", "finally", "var", "const", "return", "break", "continue", "bool", "char", "number", "attribute", "null", "function", "this", "extern", "lenof", "counter", "id"];
}