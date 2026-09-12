using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Fish flees ", story: "Fish flees", category: "Action", id: "255644b3a06176775ef0d151e72d20a3")]
public partial class FishFleesAction : Action
{
    private BehaviorGraphAgent agent;
    private Fish fish;

    protected override Status OnStart()
    {
        agent = GameObject.GetComponent<BehaviorGraphAgent>();
        fish = GameObject.GetComponent<Fish>();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        switch(fish)
        {
            case SalmonFish fish:
                break;
            case CircuitFryFish fish:
                break;
            case BassFish fish:
                break;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

