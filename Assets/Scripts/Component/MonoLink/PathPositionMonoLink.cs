using Leopotam.Ecs;
using Unity.Mathematics;

namespace Assets.Scripts.Component
{
    public class PathPositionMonoLink : MonoLink
    {
        public override void SetOnEntity(ref EcsEntity entity) => 
            entity.Get<PathPositionComponent>() = new PathPositionComponent();
    }

    public struct PathPositionComponent // : IEcsIgnoreInFilter
    {
        public int2 pathPosition;
    }
}