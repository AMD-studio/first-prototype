using UnityEngine;

namespace Climbing
{
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}