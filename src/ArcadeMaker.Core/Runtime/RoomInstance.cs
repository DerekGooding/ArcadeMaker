using ArcadeMaker.Core.Models;
using Exp;

namespace ArcadeMaker.Core.Runtime;

public class RoomInstance
{
    public RoomModel Model { get; }
    private readonly List<Instance> instances = [];
    public List<Instance> Instances => instances;

    public IEnumerable<Instance> SortedInstances
    {
        get
        {
            if (!isSorted)
            {
                instances.Sort((a, b) => a.Depth.Value!.Number.CompareTo(b.Depth.Value!.Number));
                isSorted = true;
            }
            return instances;
        }
    }

    private bool isSorted = false;

    public RoomInstance(RoomModel model)
    {
        this.Model = model;

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
        instances.Add(instance);
        isSorted = false;
    }

    public void RemoveInstance(Instance instance)
    {
        instances.Remove(instance);
        instance.DepthChanged -= MarkDepthChanged;
    }

    public void MarkDepthChanged(object? sender, double depth)
    {
        isSorted = false;
    }
}