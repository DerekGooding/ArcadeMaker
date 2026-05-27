namespace Exp;

public static class CSBasicTypes
{
    internal static Instance AsExtern(this object obj)
    {
        if (obj == null)
            return null;

        return new ExternTypeInstance(obj);
    }

    internal static Array MinArray(Instance arr)
    {
        if (arr == null || !arr.IsArray)
            Interpreter.Activated.Error($"Invalid argument: {nameof(arr)} must be an array.");
        if (arr.ArrayValues.Length == 0)
            return arr.ArrayValues;

        Type highest = (arr.Vars[0]?.Value as Instance)?.Vars[1]?.Value as Type; // exp Array.csType.instance

        if (highest == null)
        {
            // determine the item that extends the fewest number of types
            int hdepth = -1, i = -1;
            foreach (var item in arr.ArrayValues)
            {
                i++;

                int DepthOf(Type type)
                {
                    var d = 0;
                    while (type != typeof(object))
                    {
                        d++;
                        type = type.BaseType;
                    }
                    return d;
                }

                var t = item.GetType();
                var d = DepthOf(t);
                if (d < hdepth || hdepth < 0)
                {
                    highest = t;
                    hdepth = d;
                }
            }
        }

        // validate that all items extend this item
        foreach (var item in arr.ArrayValues)
        {
            bool Extends(Type c, Type p)
            {
                if (p != null && c == p)
                    return true;
                while (c != null)
                {
                    c = c.BaseType;
                    if (p != null && c == p)
                        return true;
                }
                return false;
            }

            if (!Extends(item.GetType(), highest))
                return arr.ArrayValues;
        }

        var res = Array.CreateInstance(highest, arr.ArrayValues.Length);
        Array.Copy(arr.ArrayValues, 0, res, 0, res.Length);
        return res;
    }
}
