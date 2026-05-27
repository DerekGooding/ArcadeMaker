using ArcadeMaker.Core.Models;
using Exp;

namespace ArcadeMaker.Core.Runtime;

public class RoomInstance
{
    public RoomModel Model { get; }
    public List<Instance> Instances { get; } = [];

    public IEnumerable<Instance> SortedInstances
    {
        get
        {
            if (!isSorted)
            {
                Instances.Sort((a, b) => a.Depth.Value!.Number.CompareTo(b.Depth.Value!.Number));
                isSorted = true;
            }
            return Instances;
        }
    }

    private bool isSorted = false;

    public RoomInstance(RoomModel model)
    {
        Model = model;

        // add all instances from the init map
        foreach (var item in model.InitMap.Items)
        {
            var instance = new Instance(item.Object);
            instance.X.Value = item.X.ToExp();
            instance.Y.Value = item.Y.ToExp();
            instance.DepthChanged += MarkDepthChanged;
            AddInstance(instance);
        }
    }

    public void AddInstance(Instance instance)
    {
        Instances.Add(instance);
        isSorted = false;
    }

    public void RemoveInstance(Instance instance)
    {
        Instances.Remove(instance);
        instance.DepthChanged -= MarkDepthChanged;
    }

    public void MarkDepthChanged(object? sender, double depth) => isSorted = false;
}