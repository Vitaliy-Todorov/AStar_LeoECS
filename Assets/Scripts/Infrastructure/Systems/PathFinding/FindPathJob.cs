using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Assets.Scripts.Infrastructure.Systems.GridFolder;
using Grid = Assets.Scripts.Infrastructure.Systems.GridFolder.Grid;

namespace Assets.Scripts.Infrastructure.Systems
{
    [BurstCompile]
    public struct FindPathJob : IJob
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public int2 _startPosition;
        public int2 _endPosition;

        public Grid _grid;

        public NativeList<float3> _path;

        public void Execute()
        {
            int2 gridSize = _grid.Size;
            int maxSaize = _grid.GridArray.MaxSaize;

            NativeArray <GridNode> gridArray = new NativeArray<GridNode>(_grid.GridArray.Arrray, Allocator.Temp);

            for(int i = 0; i < gridArray.Length; i++)
            {
                GridNode gridNode = gridArray[i];
                gridNode.Cost.H = CalculatedDistanceCost(gridArray[i].Position, _endPosition);
                gridArray[i] = gridNode;
            }

            NativeArray<int2> neighourOffsetArray = NeighourOffsetArray();

            int endNodeIndex = CalculatedIndex(_endPosition, maxSaize);

            GridNode startNod = gridArray[CalculatedIndex(_startPosition, maxSaize)];
            startNod.Cost.G = 0;
            gridArray[startNod.Index] = startNod;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNod.Index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, gridArray);
                GridNode currentNode = gridArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                    break;

                DeleteNode(ref openList, ref closedList, currentNodeIndex);

                for (int i = 0; i < neighourOffsetArray.Length; i++)
                {
                    int2 neighourPosition = NeighourPosition(ref neighourOffsetArray, currentNode, i);

                    // Принадлежит сетке
                    if (!IsPositionInsideGrid(neighourPosition, gridSize))
                        continue;

                    int neighbourNodeIndex = CalculatedIndex(neighourPosition, maxSaize);
                    if (closedList.Contains(neighbourNodeIndex))
                        continue;

                    GridNode neighbourNode = gridArray[neighbourNodeIndex];
                    if (!neighbourNode.IsWalkable)
                        continue;

                    int2 currentNodePosition = currentNode.Position;

                    int tentativeGCost = currentNode.Cost.G + CalculatedDistanceCost(currentNodePosition, neighourPosition);
                    if (tentativeGCost < neighbourNode.Cost.G)
                    {
                        neighbourNode.CameFromNodeIndex = currentNodeIndex;
                        neighbourNode.Cost.G = tentativeGCost;
                        gridArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.Index))
                            openList.Add(neighbourNode.Index);
                    }
                }
            }

            GridNode endNode = gridArray[endNodeIndex];
            if (endNode.CameFromNodeIndex == -1)
                Debug.Log("Didn't find a path");
            else
            {
                NativeList<int2> path = CalculatePath(gridArray, endNode);

                foreach (int2 pointNoPath in path)
                    _path.Add(_grid.GetWorldPosition(pointNoPath.x, pointNoPath.y));

                path.Dispose();
            }

            openList.Dispose();
            neighourOffsetArray.Dispose();
            closedList.Dispose();
            gridArray.Dispose();
        }

        private static void DeleteNode(ref NativeList<int> openList, ref NativeList<int> closedList, int currentNodeIndex)
        {
            for (int i = 0; i < openList.Length; i++)
                if (openList[i] == currentNodeIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }

            closedList.Add(currentNodeIndex);
        }

        private static int2 NeighourPosition(ref NativeArray<int2> neighourOffsetArray, GridNode currentNode, int i)
        {
            int2 neighourOffset = neighourOffsetArray[i];
            int2 neighourPosition = new int2(currentNode.Position.x + neighourOffset.x, currentNode.Position.y + neighourOffset.y);
            return neighourPosition;
        }

        private static NativeArray<int2> NeighourOffsetArray()
        {
            NativeArray<int2> neighourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighourOffsetArray[0] = new int2(-1, 0);
            neighourOffsetArray[1] = new int2(1, 0);
            neighourOffsetArray[2] = new int2(0, 1);
            neighourOffsetArray[3] = new int2(0, -1);
            neighourOffsetArray[4] = new int2(-1, -1);
            neighourOffsetArray[5] = new int2(-1, 1);
            neighourOffsetArray[6] = new int2(1, -1);
            neighourOffsetArray[7] = new int2(1, 1);
            return neighourOffsetArray;
        }

        private NativeList<int2> CalculatePath(NativeArray<GridNode> pathNodeArray, GridNode endNode)
        {
            if (endNode.CameFromNodeIndex == -1)
                return new NativeList<int2>(Allocator.Temp);
            else
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(endNode.Position);

                GridNode currentNode = endNode;
                while (currentNode.CameFromNodeIndex != -1)
                {
                    GridNode cameFromNode = pathNodeArray[currentNode.CameFromNodeIndex];
                    path.Add(cameFromNode.Position);
                    currentNode = cameFromNode;
                }

                return path;
            }
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<GridNode> pathNodeArray)
        {
            GridNode lowestCastPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                GridNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.Cost.F < lowestCastPathNode.Cost.F)
                    lowestCastPathNode = testPathNode;
            }

            return lowestCastPathNode.Index;
        }

        private int CalculatedIndex(int2 position, int gridWidth) =>
            position.x + position.y * gridWidth;

        // Эмпирическое растояние до финиша
        private int CalculatedDistanceCost(int2 currentPosition, int2 endPosition)
        {
            int xDistance = math.abs(endPosition.x - currentPosition.x);
            int yDistance = math.abs(endPosition.y - currentPosition.y);
            // В прямоугольник размеров xDistance X yDistance можно поместить квадрат,
            // размер такого максимального квадрата равен диагональным перемещениям.
            // straight - это не диагональные перемещения (максимальная высота/ширина - минимальная)
            int straight = math.abs(xDistance - yDistance);
            // здесь мы перемножаем количество диагональных и прямых перемещений на их длинну.
            return MOVE_STRAIGHT_COST * straight + MOVE_DIAGONAL_COST * math.min(xDistance, yDistance);
        }
    }
}
