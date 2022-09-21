using Assets.Scripts.Infrastructure.Systems;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Config/StaticData", fileName = "StaticData", order = 0)]
    public class StaticData : ScriptableObject
    {
        public MoveData MoveData;
    }}