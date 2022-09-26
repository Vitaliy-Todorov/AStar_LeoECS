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
                    _startPosition = (int2)pathFindingComponent.StartPosition,
                    _endPosition = (int2)pathFindingComponent.EndPosition,

                    _gridSize = _gridSystem.Grid.Size,
                    _grid = _gridSystem.Grid,

                    _path = new NativeList<int2>(Allocator.Persistent)
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

        /*private NativeArray<GridNode> FillGrid()
        {
            int2 gridSize;
            NativeArray<GridNode> pathNodeArray = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                {
                    int2 currentPosition = new int2(x, y);

                    Cost costNode = new Cost
                    {
                        G = int.MaxValue,
                        H = FindPathJob.CalculatedDistanceCost(currentPosition, _endPosition)
                    };

                    GridNode pathNode = new GridNode
                    {
                        Index = currentPosition.x + currentPosition.y * gridSize.x,
                        Position = currentPosition,
                        Cost = costNode,

                        IsWalkable = true,
                        CameFromNodeIndex = -1 //
                    };

                    pathNodeArray[pathNode.Index] = pathNode;
                }

            return pathNodeArray;
        }*/

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
