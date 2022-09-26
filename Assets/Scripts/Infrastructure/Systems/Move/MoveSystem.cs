using Assets.Scripts.Component;
using Leopotam.Ecs;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Infrastructure.Systems
{
    public class MoveSystem : IEcsRunSystem
    {
        private EcsFilter<MoveComponent, RigidbodyComponent, GameObjectComponent, PathComponent> _filter;

        // private InputSystem _inputService = null;
        private MoveData _moveComponent = null;

        public void Run()
        {
            foreach(int index in _filter)
            {
                ref MoveComponent moveComponent = ref _filter.Get1(index);
                // Rigidbody2D rigidbody = _filter.Get2(index).Rigidbody;
                GameObject gameObject = _filter.Get3(index).gameObject;
                ref PathComponent pathFindingComponent = ref _filter.Get4(index);

                if (pathFindingComponent.Path.IsEmpty)
                {
                    DelPathFindingComponent(index, pathFindingComponent);
                    return;
                }

                Vector3 NextNodePosition = pathFindingComponent.Path[pathFindingComponent.NextNodeNumberOfPath];
                Vector3 moveIn = NextNodePosition - gameObject.transform.position;

                /*if (rigidbody.velocity.magnitude < _moveComponent.MaxSpeed
                    && !_inputService.LeftShift)
                    {
                        Debug.Log($"LeftShift {NextNodePositionInt2}");
                        Acceleration(rigidbody, moveIn.normalized, _moveComponent.Acceleration);
                    }*/

                if (moveIn.magnitude > 0.2f)
                    Move(gameObject.transform, moveIn.normalized, _moveComponent.MaxSpeed);
                else
                    if(pathFindingComponent.NextNodeNumberOfPath > 0)
                        pathFindingComponent.NextNodeNumberOfPath -= 1;
                else
                    DelPathFindingComponent(index, pathFindingComponent);
            }
        }

        private void DelPathFindingComponent(int index, PathComponent pathFindingComponent)
        {
            pathFindingComponent.Path.Dispose();
            _filter.GetEntity(index).Del<PathComponent>();
        }

        private void Move(Transform transform, Vector3 moveIn, float maxSpeed)
        {
            transform.position += moveIn * maxSpeed * Time.deltaTime;
        }

        private void Acceleration(Rigidbody2D rigidbody, Vector2 moveIn, float acceleration) => 
            rigidbody.velocity += moveIn * acceleration;
    }
}