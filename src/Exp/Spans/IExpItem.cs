namespace Exp.Spans;

internal interface IExpItem
{
    static abstract string ItemName { get; }

    string GetItemName()
    {
        if (this is ClassDefSpan)
            return ClassDefSpan.ItemName;
        if (this is ExternClassDefSpan)
            return ExternClassDefSpan.ItemName;
        if (this is AttributeDefSpan)
            return AttributeDefSpan.ItemName;
        if (this is ConstructorDefSpan)
            return ConstructorDefSpan.ItemName;
        if (this is FuncDefSpan)
            return FuncDefSpan.ItemName;
        if (this is IfConditionSpan)
            return IfConditionSpan.ItemName;
        if (this is WhileConditionSpan)
            return WhileConditionSpan.ItemName;
        if (this is ForLoopSpan)
            return ForLoopSpan.ItemName;
        if (this is ForEachLoopSpan)
            return ForEachLoopSpan.ItemName;
        return GetType().GetProperty(nameof(ItemName), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)?.GetValue(null) as string ?? "Item";
    }
}
