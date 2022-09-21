using Leopotam.Ecs;
using UnityEngine;

namespace Assets.Scripts.Component
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyMonoLink : MonoLink
    {
        public Rigidbody2D Rigidbody;

        public override void SetOnEntity(ref EcsEntity entity)
        {
            RigidbodyComponent rigidbodyComponent = new RigidbodyComponent
            {
                Rigidbody = Rigidbody
            };

            entity.Get<RigidbodyComponent>() = rigidbodyComponent;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Rigidbody == null) { }
                Rigidbody = GetComponent<Rigidbody2D>();
        }
#endif
    }
    public struct RigidbodyComponent
    {
        public Rigidbody2D Rigidbody;
    }
}