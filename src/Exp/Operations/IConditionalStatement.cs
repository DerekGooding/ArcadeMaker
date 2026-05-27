using Exp.Spans;

namespace Exp.Operations;

internal interface IConditionalStatement : IOperationWithInnerSource
{
    ConditionSpan ConditionSpan { get; }
    IReadingOperation ConditionReading { get; }
}
