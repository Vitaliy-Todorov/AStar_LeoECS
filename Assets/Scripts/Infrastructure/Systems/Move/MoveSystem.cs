using Assets.Scripts.Component;
using Leopotam.Ecs;
using System;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class MoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter<MoveComponent, RigidbodyComponent, GameObjectComponent> _filter;

        private InputSystem _inputService;
        private Click _click;
        private MoveData _moveComponent;

        public void Init()
        {
            _click = _inputService.Click;
        }

        public void Run()
        {
            foreach(int index in _filter)
            {

                Rigidbody2D rigidbody = _filter.Get2(index).Rigidbody;
                GameObject gameObject = _filter.Get3(index).gameObject;
                ref MoveComponent moveComponent = ref _filter.Get1(index);

                Vector3 moveIn = moveComponent.MoveIn - gameObject.transform.position;

                if (rigidbody.velocity.magnitude < _moveComponent.MaxSpeed
                    && !_inputService.LeftShift)
                    Acceleration(rigidbody, moveIn.normalized, _moveComponent.Acceleration);
            }
        }

        private void Acceleration(Rigidbody2D rigidbody, Vector2 moveIn, float acceleration) => 
            rigidbody.velocity += moveIn * acceleration;
    }
}