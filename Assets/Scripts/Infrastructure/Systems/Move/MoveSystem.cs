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

            foreach (int index in _filter)
                _filter.Get1(index).MoveIn = _filter.Get3(index).gameObject.transform.position;
        }

        public void Run()
        {
            foreach(int index in _filter)
            {
                if (_click.Up && !_inputService.LeftShift)
                    _filter.Get1(index).MoveIn = _click.StaryPosition;

                Rigidbody2D rigidbody = _filter.Get2(index).Rigidbody;
                GameObject gameObject = _filter.Get3(index).gameObject;

                Vector3 moveIn = _filter.Get1(index).MoveIn - gameObject.transform.position;

                if (rigidbody.velocity.magnitude < _moveComponent.MaxSpeed
                    && !_inputService.LeftShift)
                    Acceleration(rigidbody, moveIn.normalized, _moveComponent.Acceleration);
            }
        }

        private void Acceleration(Rigidbody2D rigidbody, Vector2 moveIn, float acceleration) => 
            rigidbody.velocity += moveIn * acceleration;
    }
}