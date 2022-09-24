using Assets.Scripts.Component;
using Leopotam.Ecs;
using Unity.Mathematics;

namespace Assets.Scripts.Infrastructure.Systems.GridFolder
{
    public struct GridNode
    {
        public int Index;
        public int2 Position;

        public bool IsWalkable;
        public int CameFromNodeIndex;

        public Cost Cost;

        public bool IsWall;
        public int WallIdEntity;
    }
}