using Assets.Scripts.Component;
using Assets.Scripts.FindingPath.Grid;
using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Logic.FindingPath.GridFolder
{
    public struct Grid
    {
        private GridNode[,] _gridArray;
        private Vector3 _originPosition;
        private float _cellSize;

        public float CellSize { get => _cellSize; }

        public Grid(int width, int height, float cellSize, Vector3 originPosition)
        {
            _gridArray = new GridNode[width, height];
            _originPosition = originPosition;
            _cellSize = cellSize;

            for (int x = 0; x < _gridArray.GetLength(0); x++)
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    int2 currentPosition = new int2(x, y);

                    Cost costNode = new Cost();
                    /*{
                        G = int.MaxValue,
                        H = CalculatedDistanceCost(currentPosition, endPosition)
                    };*/

                    _gridArray[x, y] = new GridNode
                    {
                        Index = CalculatedIndex(currentPosition),
                        Position = currentPosition,
                        Cost = costNode,

                        IsWalkable = true,
                        CameFromNodeIndex = -1 //
                    };
                }

            for (int x = 0; x <= _gridArray.GetLength(0); x++)
                Debug.DrawLine(GridCorners(x, 0), GridCorners(x, _gridArray.GetLength(1)), Color.green, 100);

            for (int y = 0; y <= _gridArray.GetLength(1); y++)
                Debug.DrawLine(GridCorners(0, y), GridCorners(_gridArray.GetLength(0), y), Color.green, 100);
        }

        public int CalculatedIndex(int2 currentPosition)
        {
            int max = math.max(_gridArray.GetLength(0), _gridArray.GetLength(1));
            return currentPosition.x + currentPosition.y * max;
        }

        private Vector3 GridCorners(int x, int y) =>
            GetWorldPosition(x, y) - new Vector3(1, 1, 0) * .5f * _cellSize;

        #region Wall
        public bool IsWall(Vector3 positionInWorld)
        {
            int2 positionInGrid = PositionInGrid(positionInWorld);

            return PositionToGrid(positionInWorld) && _gridArray[positionInGrid.x, positionInGrid.y].IsWall;
        }

        public void SetWall(Vector3 positon, EcsEntity entity)
        {
            int2 positionInGrid = PositionInGrid(positon);
            Vector3 positionInGridWorld = GetWorldPosition(positionInGrid.x, positionInGrid.y);

            Transform transformWorld = entity.Get<GameObjectComponent>().gameObject.transform;
            transformWorld.position = positionInGridWorld;
            transformWorld.localScale = new Vector3(1, 1, 1) * _cellSize;

            _gridArray[positionInGrid.x, positionInGrid.y].IsWall = true;
            _gridArray[positionInGrid.x, positionInGrid.y].WallIdEntity = entity.GetInternalId();
        }

        public void DestroyWall(Vector3 positon, EcsWorld world)
        {
            int2 positionInGrid = PositionInGrid(positon);
            _gridArray[positionInGrid.x, positionInGrid.y].IsWall = false;
            int wallIdEntity = _gridArray[positionInGrid.x, positionInGrid.y].WallIdEntity;
            EcsEntity wallEntity = world.RestoreEntityFromInternalId(wallIdEntity);
            Object.Destroy(wallEntity.Get<GameObjectComponent>().gameObject);
        }
        #endregion

        #region Position
        public bool PositionToGrid(Vector3 positon)
        {
            int2 positionInGrid = PositionInGrid(positon);

            if (positionInGrid.x < _gridArray.GetLength(0) &&
                positionInGrid.x >= 0 &&
                positionInGrid.y < _gridArray.GetLength(1) &&
                positionInGrid.y >= 0)
                return true;
            else
                return false;
        }

        public Vector3 PositionInGridWorld(Vector3 positionInWorld)
        {
            int2 positionInGridInt = PositionInGrid(positionInWorld);
            return GetWorldPosition(positionInGridInt.x, positionInGridInt.y);
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0) * _cellSize + _originPosition;
        }

        private int2 PositionInGrid(Vector3 positionInWorld)
        {
            Vector3 positionInGrid = (positionInWorld - _originPosition) / _cellSize;
            int x = Mathf.RoundToInt(positionInGrid.x);
            int y = Mathf.RoundToInt(positionInGrid.y);

            return new int2(x, y);
        }
        #endregion
    }
}