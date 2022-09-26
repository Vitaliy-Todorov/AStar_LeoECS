using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Extension 
    {
        public static float2 GetFloat2(this Vector3 vector3) =>
            new float2(vector3.x, vector3.y);

        public static Vector3 GetVector3(this float2 vector) =>
            new Vector3(vector.x, vector.y);

        public static int2 GetInt2(this float2 vector)
        {
            int x = Mathf.RoundToInt(vector.x);
            int y = Mathf.RoundToInt(vector.y);
            return new int2(x, y);
        }
    }
}
