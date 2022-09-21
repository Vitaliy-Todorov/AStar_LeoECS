using Assets.Scripts.Component;
using Leopotam.Ecs;
using System;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class MoveSystem : IEcsRunSystem
    {
        private EcsFilter<MoveComponent, RigidbodyComponent, GameObjectComponent> _filter;

        private Click _click;
        private MoveData _moveComponent;

        public void Run()
        {
            foreach(int index in _filter)
            {
                Rigidbody2D rigidbody = _filter.Get2(index).Rigidbody;
                GameObject gameObject = _filter.Get3(index).gameObject;

                Vector3 moveIn = _click.StaryPosition - gameObject.transform.position;

                // Vector3 distance = moveIn - gameObject.transform.position;

                if (rigidbody.velocity.magnitude < _moveComponent.MaxSpeed)
                    Acceleration(rigidbody, moveIn.normalized, _moveComponent.Acceleration);
            }
        }

        private void Acceleration(Rigidbody2D rigidbody, Vector2 moveIn, float acceleration) => 
            rigidbody.velocity += moveIn * acceleration;
    }
}