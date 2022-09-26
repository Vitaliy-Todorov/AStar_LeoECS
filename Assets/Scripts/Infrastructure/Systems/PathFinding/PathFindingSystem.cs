using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Leopotam.Ecs;
using Assets.Scripts.Component;
using Assets.Scripts.Infrastructure.Systems.GridFolder;

namespace Assets.Scripts.Infrastructure.Systems
{
    // [UpdateAfter(typeof(UnitMoveOrderSystem))]
    public partial class PathFindingSystem : IEcsRunSystem
    {
        private EcsFilter<PathFindingComponent> _filter;
        private GridSystem _gridSystem;

        public void Construct(GridSystem gridSystem) =>
            _gridSystem = gridSystem;

        public void Run()
        {
            PathFinding(_filter);
        }

        private void PathFinding(EcsFilter<PathFindingComponent> filter)
        {
            foreach (int index in filter)
            {
                PathFindingComponent pathFindingComponent = filter.Get1(index);

                FindPathJob findPathJob = new FindPathJob
                {
                    _startPosition = _gridSystem.Grid.PositionInGrid(pathFindingComponent.StartPosition.GetVector3()),
                    _endPosition = _gridSystem.Grid.PositionInGrid(pathFindingComponent.EndPosition.GetVector3()),

                    _gridSize = _gridSystem.Grid.Size,
                    _grid = _gridSystem.Grid,

                    _path = new NativeList<float3>(Allocator.Persistent)
                };

                JobHandle handle = findPathJob.Schedule();

                handle.Complete();

                SetPathComponent(_filter.GetEntity(index), findPathJob);

                // path.Dispose();

                filter
                    .GetEntity(index)
                    .Del<PathFindingComponent>();
            }
        }

        private void SetPathComponent(EcsEntity entity, FindPathJob findPathJob)
        {
            if (!entity.Has<PathComponent>())
                entity.Get<PathComponent>() = new PathComponent
                {
                    Path = findPathJob._path,
                    NextNodeNumberOfPath = findPathJob._path.Length - 1
                };
        }
    }
}
