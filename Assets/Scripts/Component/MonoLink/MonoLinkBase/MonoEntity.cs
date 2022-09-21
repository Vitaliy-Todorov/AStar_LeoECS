using Leopotam.Ecs;
using System;
using UnityEngine;

namespace Assets.Scripts.Component
{
    [Serializable]
    public partial class MonoEntity : MonoBehaviour
    {
        private EcsEntity _entity;

        public EcsEntity Entity { get => _entity; }

        public void SetComponentFromGObj(EcsWorld world)
        {
            _entity = world.NewEntity();

            GameObjectComponent gameObjectComponent = new GameObjectComponent
            {
                gameObject = gameObject
            };

            _entity.Get<GameObjectComponent>() = gameObjectComponent;

            MonoLink[] monoLinks = GetComponents<MonoLink>();

            foreach(MonoLink monoLink in monoLinks)
                monoLink.SetOnEntity(ref _entity);
        }

        private void OnDestroy()
        {
            _entity.Destroy();
        }
    }
}