using Leopotam.Ecs;
using UnityEngine;

namespace Assets.Scripts.Component
{
    public abstract class MonoLink : MonoBehaviour
    {
        public abstract void SetOnEntity(ref EcsEntity entity);
    }
}