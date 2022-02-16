using System;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarDynamicBoneCollider : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position, 0.05f * Vector3.one);
        }
    }
}