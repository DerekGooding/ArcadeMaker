using Exp.Spans;

namespace Exp.Operations;

internal class Assignment(PointingOrFuncCall pointing, OperatorSpan opertor, IReadingOperation? readingOperation) : IOperation
{
    public void Make()
    {
        IValue? readingResult = null;
        var known = pointing.Next == null ? pointing.KnownPointer : null;
        if (known == null)
        {
            readingResult = pointing.Read();
            known = (readingResult as SpecialValue<INamedValue>)?.Value as Variable;
        }

        if (known == Variable.Futile)
            return;
        else if (known != null)
            known.Value = opertor.Result(known.Value, readingOperation?.Read());
        else
            throw new Exception($"Pointing did not return a variable but '{readingResult.GetExpTypeName(true)}'.");
    }
}
