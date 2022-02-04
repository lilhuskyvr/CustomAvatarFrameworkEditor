#if UNITY_EDITOR
using UnityEngine;

public static class GameObjectExtension
{
    public static void AddCustomAvatarHeads(this GameObject gameObject)
    {
        var animator = gameObject.GetComponent<Animator>();

        if (animator == null)
            return;

        var head = animator.GetBoneTransform(HumanBodyBones.Head);

        if (head == null)
            return;

        var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        var extraPosition = 1000;
        var minDistance = extraPosition*0.9f;
        
        //move the head far away from the body
        head.position += extraPosition * head.transform.forward;

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            var bakedMesh = new Mesh();
            
            skinnedMeshRenderer.BakeMesh(bakedMesh);
            
            foreach (var bakedMeshVertex in bakedMesh.vertices)
            {
                if (bakedMeshVertex.magnitude > minDistance)
                {
                    skinnedMeshRenderer.AddCustomAvatarHead();
                    break;
                }
            }
        }

        //put it back to original position
        head.position -= extraPosition * head.transform.forward;
    }
}

#endif