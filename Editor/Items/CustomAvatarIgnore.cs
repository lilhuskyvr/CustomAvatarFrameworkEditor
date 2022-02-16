#if UNITY_EDITOR
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarIgnore : MonoBehaviour
    {
        [Tooltip("lil husky builder will not add custom avatar head to this game object")]
        public bool ignoreCustomAvatarHead;
        [Tooltip("lil husky builder will not add custom avatar dynamic bone to this game object")]
        public bool ignoreCustomAvatarDynamicBone;
        [Tooltip("lil husky builder will not convert materials to MOES for this game object")]
        public bool ignoreMOES;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, 0.05f * Vector3.one);
        }
    }
}

#endif