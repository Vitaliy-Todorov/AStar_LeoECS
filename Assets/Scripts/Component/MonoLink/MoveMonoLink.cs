using Leopotam.Ecs;
using Unity.Mathematics;
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


        public bool PathFound;
        public float2 StartPosition;
        public float2 EndPosition;
    }
}