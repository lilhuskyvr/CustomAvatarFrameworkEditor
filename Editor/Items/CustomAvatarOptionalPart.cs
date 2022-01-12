using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarOptionalPart : MonoBehaviour
    {
        [Tooltip("the visibility of this part is based on the parent part")]
        public Transform parentPart;
        [Range(0, 100)] public float percentage;
    }
}