using Unity.Collections;
using Unity.Mathematics;

namespace Assets.Scripts.Component
{
    public struct PathComponent
    {
        public NativeList<float3> Path;

        public int NextNodeNumberOfPath;
    }
}
