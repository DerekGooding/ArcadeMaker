using Exp.Spans;

namespace Exp;

/// <summary>
/// Representes a variable.
/// </summary>
public class Variable : IExpItem, INamedValue
{
    /// <summary>
    /// This variable is returned by <see cref="Operations.PointingOrFuncCall.Read()"/> when the experssion is in format <c>x?.y = z</c> and <c>x</c> is <c>null</c>. The <see cref="Assignment"/> operation is cancelling itself if it gets that var.
    /// </summary>
    internal static Variable Futile { get; } = new("<<futile_variable>>", null, null);

    public static string ItemName { get; } = "variable";
    public string Name { get; }
    internal bool firstSet = true;

    public bool IsVar => true;

    public virtual IValue? Value
    {
        get;
        set
        {
            if (Const && !firstSet)
            {
                Interpreter.Activated.ThrowRuntime($"This variable ('{Name}') is marked as constant and cannot be set.", RuntimeException.INVALID_OPERATION);
                return;
            }
            field = value;
            firstSet = false;
        }
    }

    public void SetSkippingConstant(IValue value)
    {
        firstSet = true;
        Value = value;
    }

    public bool Private { get; set; }
    internal bool Const { get; }
    internal Span? SettingSpan { get; }

    public Variable(string name, IValue? value, Span? settingSpan = null, bool prvt = false, bool cons = false)
    {
        Name = name;
        Value = value;
        SettingSpan = settingSpan;
        Private = prvt;
        Const = cons;
    }
}
