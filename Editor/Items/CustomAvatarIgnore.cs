#if UNITY_EDITOR
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarIgnore : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, 0.05f * Vector3.one);
        }
    }
}

#endif