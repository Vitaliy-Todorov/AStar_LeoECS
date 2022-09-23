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

        public void SetComponentFromGObj(EcsEntity entity)
        {
            _entity = entity;
            //_entity.Get<PathFindingComponent>();

            GameObjectComponent gameObjectComponent = new GameObjectComponent
            {
                gameObject = gameObject
            };

            entity.Get<GameObjectComponent>() = gameObjectComponent;

            MonoLink[] monoLinks = GetComponents<MonoLink>();

            foreach(MonoLink monoLink in monoLinks)
                monoLink.SetOnEntity(ref entity);
        }

        private void OnDestroy()
        {
            _entity.Destroy();
        }
    }
}