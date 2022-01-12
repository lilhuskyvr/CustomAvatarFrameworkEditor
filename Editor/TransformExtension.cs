using UnityEngine;

namespace CustomAvatarFramework.Editor
{
    public static class TransformExtension
    {
        public static void AlignWith(this Transform transform, Transform alignWith)
        {
            transform.position = alignWith.position;
            transform.rotation = alignWith.rotation;
        }
    }
}