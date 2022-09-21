using Assets.Scripts.Component;
using Assets.Scripts.Data;
using Assets.Scripts.Infrastructure.Systems;
using Assets.Scripts.Logic.FindingPath.GridFolder;
using Leopotam.Ecs;
using System;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public class MainStartup : MonoBehaviour
    {
        [SerializeField]
        private StaticData _staticData;
        [SerializeField]
        private SceneData _sceneData;
        [SerializeField]
        private GameObject _player;

        private EcsWorld _world;
        private EcsSystems _systems;
        private EcsSystems _fixedSystems;
        private InputSystem _inputSystem;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world, "UpdateSystems");
            _fixedSystems = new EcsSystems(_world, "FixedUpdateSystems");

            InitializedEntities();
            InitializedServices();
            InitializeObserver();

            InitializedUpdateSystems();
            InitializeFixedSystems();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void FixedUpdate()
        {
            _fixedSystems?.Run();
        }

        private void InitializedEntities()
        {
            foreach (Vector3 spawnPosition in _sceneData.SpawnPositions)
            {
                GameObject player = Instantiate(_player, spawnPosition, Quaternion.identity);
                MonoEntity monoEntity = player.GetComponent<MonoEntity>();
                monoEntity.SetComponentFromGObj(_world);
            }
        }

        private void InitializedServices()
        {
            _inputSystem = new InputSystem();
        }

        private void InitializedUpdateSystems()
        {
            _systems
                .Add(_inputSystem)
                .Add(new GridSystem())
                .Inject(_world)
                .Inject(_inputSystem)
                .Init();
        }

        private void InitializeFixedSystems()
        {
            _fixedSystems
                .Add(new MoveSystem())
                .Inject(_inputSystem)
                .Inject(_staticData.MoveData)
                .Init();
        }

        private void InitializeObserver()
        {
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_fixedSystems);
#endif
        }
    }
}
