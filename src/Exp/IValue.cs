namespace Exp;

public interface IValue
{
    bool IsBool => false;
    bool Bool { get { Unexpected(ValueHelper.tbool); throw null; } } // set => Unexpected(ValueHelper.tbool); }
    bool IsChar => false;
    char Char { get { Unexpected(ValueHelper.tchar); throw null; } } // set => Unexpected(ValueHelper.tchar); }
    bool IsNumber => false;
    double Number { get { Unexpected(ValueHelper.tnum); throw null; } } // set => Unexpected(ValueHelper.tnum); }
    bool IsInst => false;
    Instance Inst { get { Unexpected(ValueHelper.tinst); throw null; } } // set => Unexpected(ValueHelper.tinst); }
    bool IsFunc => false;
    FuncPntr FuncPntr { get { Unexpected(ValueHelper.tfunc); throw null; } } // set => Unexpected(ValueHelper.tfunc); }
    object Object => IsBool ? Bool : IsChar ? Char : IsNumber ? Number : this;

    string TypeName { get; }

    void Unexpected(string expected) => Interpreter.Activated.ThrowRuntime($"A value of type {expected} was expected, but {TypeName} received.", RuntimeException.INVALID_ARGUMENT);

    IValue Pass() =>
        // unneccsary since value types (bool, char, number) are now readonly
        //if (IsBool)
        //    return Bool.ToExp();
        //if (IsChar)
        //    return Char.ToExp();
        //if (IsNumber)
        //    return Number.ToExp();
        this;
}
