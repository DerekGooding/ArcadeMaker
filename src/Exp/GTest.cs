namespace Exp;

public class GTest<T>
{
    public void Met(T obj) => obj.Println();

    public static object Testing(object[] args)
    {
        return null;
        //var (c1, s1) = args.ValidateArguments<char, string>();

        //return !string.IsNullOrEmpty(s1) && (s1[0] == c1);
    }

    private static void F(int a)
    { }

    private static void F(int a, int b)
    { }
}
