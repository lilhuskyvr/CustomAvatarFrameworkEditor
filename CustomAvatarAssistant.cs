#if UNITY_EDITOR
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;


public class CustomAvatarAssistant : MonoBehaviour
{
    [Button]
    public void AddCustomAvatarDynamicBoneColliders()
    {
        var customAvatar = GetComponentInChildren<CustomAvatar>();
        if (customAvatar == null)
        {
            Debug.LogError("No CustomAvatar found in children");
            return;
        }

        customAvatar.animator.gameObject.AddCustomAvatarDynamicBoneColliders();
    }
}

#endif