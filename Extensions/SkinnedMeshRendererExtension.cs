using CustomAvatarFramework.Editor.Items;
using UnityEngine;

public static class SkinnedMeshRendererExtension
{
    public static void AddCustomAvatarHead(this SkinnedMeshRenderer skinnedMeshRenderer)
    {
        var customAvatarHead = skinnedMeshRenderer.GetComponent<CustomAvatarHead>();

        if (customAvatarHead != null)
            return;

        skinnedMeshRenderer.gameObject.AddComponent<CustomAvatarHead>();
    }
}