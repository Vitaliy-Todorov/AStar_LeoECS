using Assets.Scripts.Component;
using Leopotam.Ecs;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class InputMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter<MoveComponent, GameObjectComponent> _filter;

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
            foreach (int index in _filter)
            {
                ref MoveComponent moveComponent = ref _filter.Get1(index);

                if (_click.Up && !_inputService.LeftShift)
                {
                    moveComponent.MoveIn = _click.StaryPosition;
                }
            }
        }
    }
}