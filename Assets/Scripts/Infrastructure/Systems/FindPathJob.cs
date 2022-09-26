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

        public int2 _gridSize;
        public Grid _grid;

        public NativeList<int2> _path;

        public void Execute()
        {
            int2 gridSize = _gridSize;

            NativeArray<GridNode> gridArray = FillGrid(gridSize);

            NativeArray<int2> neighourOffsetArray = NeighourOffsetArray();

            int endNodeIndex = CalculatedIndex(_endPosition, gridSize.x);

            GridNode startNod = gridArray[CalculatedIndex(_startPosition, gridSize.x)];
            startNod.Cost.G = 0;
            gridArray[startNod.Index] = startNod;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNod.Index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, gridArray);
                GridNode currentNode = gridArray[currentNodeIndex];

                // Достигли конца
                if (currentNodeIndex == endNodeIndex)
                    break;

                // Удаляем пройденый узел
                for (int i = 0; i < openList.Length; i++)
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighourOffsetArray.Length; i++)
                {
                    int2 neighourOffset = neighourOffsetArray[i];
                    int2 neighourPosition = new int2(currentNode.Position.x + neighourOffset.x, currentNode.Position.y + neighourOffset.y);

                    // Принадлежит сетке
                    if (!IsPositionInsideGrid(neighourPosition, gridSize))
                        continue;

                    int neighbourNodeIndex = CalculatedIndex(neighourPosition, gridSize.x);
                    // Проходили ли по этому узлу
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
                    _path.Add(pointNoPath);

                path.Dispose();
            }

            openList.Dispose();
            neighourOffsetArray.Dispose();
            closedList.Dispose();
            gridArray.Dispose();
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

        private NativeArray<GridNode> FillGrid(int2 gridSize)
        {
            NativeArray<GridNode> pathNodeArray = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                {
                    int2 currentPosition = new int2(x, y);

                    Cost costNode = new Cost
                    {
                        G = int.MaxValue,
                        H = CalculatedDistanceCost(currentPosition, _endPosition)
                    };

                    GridNode pathNode = new GridNode
                    {
                        Index = CalculatedIndex(currentPosition, gridSize.x),
                        Position = currentPosition,
                        Cost = costNode,

                        IsWalkable = true,
                        CameFromNodeIndex = -1 //
                    };

                    pathNodeArray[pathNode.Index] = pathNode;
                }

            return pathNodeArray;
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
