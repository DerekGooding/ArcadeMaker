namespace Exp;

public class Test : IDisposable
{
    public void Dispose()
    { }

    public static void Met(double d)
    { }

    public void Met(object[] i) => $"Great o[] {i.Length}".Print();

    public static void Met(Test[] i) => $"Great t[] {i.Length}".Print();

    //public Test() { }
    public double D { get; set; } = 355;

    public static object Reado { get; } = 777;
    public static object Writeo { set => $"setted to {value}".Println(); }

    public override string ToString() => $"[Amazing Test where d = {D}]";
}
