using Assets.Scripts.Component;
using Assets.Scripts.Infrastructure.Systems;
using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Logic.FindingPath.GridFolder
{
    public partial class GridSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private InputSystem _inputService;
        private Click _click;

        private Grid _grid;
        private float _scalleGrid = 1.5f;

        private string _addressPrefabWall = "Wall";
        private GameObject _prefabWall;

        public void Init()
        {
            _prefabWall = Resources.Load<GameObject>(_addressPrefabWall);

            _click = _inputService.Click;

            _grid = new Grid(9, 5, _scalleGrid, new Vector3(-4, -2, 0) * _scalleGrid);
        }

        public void Run()
        {
            if (_click.Up 
                && _inputService.LeftShift
                && _grid.PositionToGrid(_click.StaryPosition))
                SetIsWall(_click.StaryPosition);
        }

        public void SetIsWall(Vector3 positon)
        {
            if (_grid.IsWall(positon))
                _grid.DestroyWall(positon, _world);
            else
            {
                EcsEntity wall = GObjToEntity(_prefabWall);
                _grid.SetWall(positon, wall);
            }
        }

        private EcsEntity GObjToEntity(GameObject prefab)
        {
            GameObject GObj = Object.Instantiate(_prefabWall);

            MonoEntity monoEntity = GObj.GetComponent<MonoEntity>();
            monoEntity.SetComponentFromGObj(_world);
            EcsEntity entity = monoEntity.Entity;

            return entity;
        }
    }
}