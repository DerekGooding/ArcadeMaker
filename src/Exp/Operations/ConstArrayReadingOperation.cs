using Exp.Spans;

namespace Exp.Operations;

internal class ConstArrayReadingOperation(ConstValueReadingOperation[] readings) : ConstValueReadingOperation(new Instance(ClassDefSpan.ExpArrayDef, readings.Select(r => r.Read()).ToArray()));
