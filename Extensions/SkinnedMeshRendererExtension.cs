using CustomAvatarFramework.Editor.Items;
using ThunderRoad;
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

    public static void AddRevealDecal(this SkinnedMeshRenderer skinnedMeshRenderer)
    {
        var revealDecal = skinnedMeshRenderer.GetComponent<RevealDecal>();

        if (revealDecal == null)
        {
            revealDecal = skinnedMeshRenderer.gameObject.AddComponent<RevealDecal>();
        }
        
        revealDecal.SetMaskResolutionFull();
    }
}