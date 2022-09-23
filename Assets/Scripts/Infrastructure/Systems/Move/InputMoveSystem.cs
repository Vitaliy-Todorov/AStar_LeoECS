using Assets.Scripts.Component;
using Leopotam.Ecs;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class InputMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter<MoveComponent, GameObjectComponent> _filter;
        private EcsFilter<PathFindingComponent> _filterPathFinding;

        private InputSystem _inputService;
        private Click _click;
        private MoveData _moveComponent;

        public void Init()
        {
            _click = _inputService.Click;

            foreach (int index in _filter)
                _filter.Get1(index).MoveIn = _filter.Get2(index).gameObject.transform.position;
        }

        public void Run()
        {
            if (_click.Up && !_inputService.LeftShift)
            {
                foreach (int index in _filter)
                {
                    ref EcsEntity entity = ref _filter.GetEntity(index);
                    var onCollisionEnterEvent = entity.Get<OnCollisionEnterEvent>();

                    ref MoveComponent moveComponent = ref _filter.Get1(index);
                    GameObject gameObject = _filter.Get2(index).gameObject;

                    moveComponent.MoveIn = _click.StartPosition;

                    PathFindingComponent x = entity.Get<PathFindingComponent>();
                    entity.Get<PathFindingComponent>()
                        = CreatePathFindingComponent(gameObject.transform.position, _click.StartPosition);
                }
            }
        }

        private PathFindingComponent CreatePathFindingComponent(Vector3 startPosition, Vector3 endPosition)
        {
            PathFindingComponent pathFindingComponent = new PathFindingComponent();

            pathFindingComponent.StartPosition = new float2(startPosition.x, startPosition.y);
            pathFindingComponent.StartPosition = new float2(endPosition.x, endPosition.y);

            return pathFindingComponent;
        }
    }
}