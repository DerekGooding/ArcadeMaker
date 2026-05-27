namespace ArcadeMaker.Core.ExpSrc;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class ExpFuncAttribute(params int[] paramsCount) : Attribute
{
    internal int[] ParamsCounts => paramsCount;
    public string? CustomName { get; init; }
    public bool IsNonStaticFuncOfGameObjects { get; init; }
}