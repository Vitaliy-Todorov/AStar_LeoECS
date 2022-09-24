using Unity.Collections;
using Unity.Mathematics;

namespace Assets.Scripts.Component
{
    public struct PathComponent
    {
        public NativeList<int2> Path;

        public int NextNodeNumberOfPath;
    }
}
