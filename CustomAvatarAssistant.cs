#if UNITY_EDITOR
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;

/// <summary>
/// This class to provide an easy UI to edit the avatar.
/// </summary>
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