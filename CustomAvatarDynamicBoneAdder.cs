#if UNITY_EDITOR
using EasyButtons;
using UnityEngine;

namespace CustomAvatarFramework.Editor
{
    public class CustomAvatarDynamicBoneAdder: MonoBehaviour
    {
        public GameObject inputGameObject;

        [Button]
        public void AddCustomAvatarDynamicBones()
        {
            inputGameObject.AddCustomAvatarDynamicBones();
        }
    }
}

#endif