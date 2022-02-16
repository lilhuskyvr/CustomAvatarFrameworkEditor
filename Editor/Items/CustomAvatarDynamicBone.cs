using System;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarDynamicBone : MonoBehaviour
    {
        [Tooltip("apply gravity to this dynamic bone")]
        public bool useGravity;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.05f);
        }
    }
}