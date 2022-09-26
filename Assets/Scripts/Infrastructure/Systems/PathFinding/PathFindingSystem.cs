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
            foreach (int index in _filter)
            {
                PathFindingComponent pathFindingComponent = _filter.Get1(index);
                ref EcsEntity entity = ref _filter.GetEntity(index);

                if (PositionToGrid(pathFindingComponent))
                {
                    entity.Del<PathFindingComponent>();
                    return;
                }

                DelPathComponent(entity);

                FindPathJob findPathJob = new FindPathJob
                {
                    _startPosition = _gridSystem.Grid.PositionInGrid(pathFindingComponent.StartPosition.GetVector3()),
                    _endPosition = _gridSystem.Grid.PositionInGrid(pathFindingComponent.EndPosition.GetVector3()),

                    _grid = _gridSystem.Grid,

                    _path = new NativeList<float3>(Allocator.Persistent)
                };

                JobHandle handle = findPathJob.Schedule();

                handle.Complete();

                SetPathComponent(entity, findPathJob);

                entity.Del<PathFindingComponent>();
            }
        }

        private bool PositionToGrid(PathFindingComponent pathFindingComponent)
        {
            return !_gridSystem.Grid.PositionToGrid(pathFindingComponent.StartPosition.GetFloat3())
                                || !_gridSystem.Grid.PositionToGrid(pathFindingComponent.EndPosition.GetFloat3());
        }

        private static void DelPathComponent(EcsEntity entity)
        {
            if (entity.Has<PathComponent>())
            {
                entity.Get<PathComponent>().Path.Dispose();
                entity.Del<PathComponent>();
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
