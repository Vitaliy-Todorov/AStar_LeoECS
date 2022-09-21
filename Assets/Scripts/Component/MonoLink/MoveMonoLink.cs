using Leopotam.Ecs;
using UnityEngine;

namespace Assets.Scripts.Component
{
    public class MoveMonoLink : MonoLink
    {
        public override void SetOnEntity(ref EcsEntity entity) => 
            entity.Get<MoveComponent>() = new MoveComponent();
    }

    public struct MoveComponent // : IEcsIgnoreInFilter
    {
        public Vector3 MoveIn;
    }
}