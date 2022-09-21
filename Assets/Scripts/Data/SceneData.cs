using Assets.Scripts.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Config/SceneData", fileName = "SceneData", order = 0)]
    public class SceneData : ScriptableObject
    {
        public List<Vector3> SpawnPositions;
    }
}