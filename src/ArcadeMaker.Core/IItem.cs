namespace ArcadeMaker.Core;

internal interface IItem
{
    string Name { get; }
}

internal interface ISetsID : IItem
{
    int ID { get; }
}