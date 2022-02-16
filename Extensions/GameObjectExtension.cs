#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using CustomAvatarFramework.Editor.Items;
using UnityEngine;

public static class GameObjectExtension
{
    public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
    {
        var result = gameObject.GetComponent<T>();

        if (result != null)
            return result;

        return gameObject.AddComponent<T>();
    }

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
        var minDistance = extraPosition * 0.9f;

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

    public static void AddCustomAvatarDynamicBones(this GameObject gameObject)
    {
        //create bones hashset here

        var animator = gameObject.GetComponent<Animator>();

        if (animator == null)
            return;

        var mainBones = new HashSet<Transform>();
        var subBones = new HashSet<Transform>();

        var humanBodyBones = new List<HumanBodyBones>()
        {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.UpperChest,
            HumanBodyBones.Chest,
            HumanBodyBones.Head,
            HumanBodyBones.Neck,
            HumanBodyBones.LeftShoulder,
            HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.LeftToes,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.RightFoot,
            HumanBodyBones.RightToes,
            HumanBodyBones.LeftEye,
            HumanBodyBones.RightEye,
            HumanBodyBones.Jaw,
            HumanBodyBones.LeftIndexDistal,
            HumanBodyBones.LeftIndexIntermediate,
            HumanBodyBones.LeftIndexProximal,
            HumanBodyBones.LeftMiddleDistal,
            HumanBodyBones.LeftMiddleIntermediate,
            HumanBodyBones.LeftMiddleProximal,
            HumanBodyBones.LeftRingDistal,
            HumanBodyBones.LeftRingIntermediate,
            HumanBodyBones.LeftRingProximal,
            HumanBodyBones.LeftLittleDistal,
            HumanBodyBones.LeftLittleIntermediate,
            HumanBodyBones.LeftLittleProximal,
            HumanBodyBones.LeftThumbDistal,
            HumanBodyBones.LeftThumbIntermediate,
            HumanBodyBones.LeftThumbProximal,
            HumanBodyBones.RightIndexDistal,
            HumanBodyBones.RightIndexIntermediate,
            HumanBodyBones.RightIndexProximal,
            HumanBodyBones.RightMiddleDistal,
            HumanBodyBones.RightMiddleIntermediate,
            HumanBodyBones.RightMiddleProximal,
            HumanBodyBones.RightRingDistal,
            HumanBodyBones.RightRingIntermediate,
            HumanBodyBones.RightRingProximal,
            HumanBodyBones.RightLittleDistal,
            HumanBodyBones.RightLittleIntermediate,
            HumanBodyBones.RightLittleProximal,
            HumanBodyBones.RightThumbDistal,
            HumanBodyBones.RightThumbIntermediate,
            HumanBodyBones.RightThumbProximal
        };

        foreach (var humanBodyBone in humanBodyBones)
        {
            var bone = animator.GetBoneTransform(humanBodyBone);

            if (bone == null)
                continue;

            mainBones.Add(bone);
        }

        foreach (var mainBone in mainBones)
        {
            var subBoneTransforms = mainBone.GetImmediateChildTransforms();

            foreach (var subBoneTransform in subBoneTransforms)
            {
                if (mainBones.Contains(subBoneTransform) || !subBoneTransform.HasChildren())
                    continue;

                //if sub bone is a parent of 1 main bone, continue
                if (subBoneTransform.GetChildTransforms().Any(c => mainBones.Contains(c)))
                    continue;

                subBones.Add(subBoneTransform);
            }
        }

        foreach (var subBone in subBones)
        {
            var customAvatarIgnore = subBone.gameObject.GetComponent<CustomAvatarIgnore>();
            if (customAvatarIgnore != null)
            {
                if (customAvatarIgnore.ignoreCustomAvatarDynamicBone)
                    continue;
            }

            subBone.gameObject.AddOrGetComponent<CustomAvatarDynamicBone>();
        }
    }

    public static void AddCustomAvatarDynamicBoneColliders(this GameObject gameObject)
    {
        //create bones hashset here

        var animator = gameObject.GetComponent<Animator>();

        if (animator == null)
            return;

        var mainBones = new HashSet<Transform>();
        var subBones = new HashSet<Transform>();

        var humanBodyBones = new List<HumanBodyBones>()
        {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.UpperChest,
            HumanBodyBones.Head,
            HumanBodyBones.LeftShoulder,
            HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.RightLowerLeg,
        };

        foreach (var humanBodyBone in humanBodyBones)
        {
            var bone = animator.GetBoneTransform(humanBodyBone);

            if (bone == null)
                continue;

            mainBones.Add(bone);
        }

        foreach (var mainBone in mainBones)
        {
            mainBone.gameObject.AddOrGetComponent<CustomAvatarDynamicBoneCollider>();
        }
    }

    public static bool CanBeMadeIntoCustomAvatar(this GameObject gameObject, out List<string> errors)
    {
        var animator = gameObject.GetComponent<Animator>();

        if (animator == null)
        {
            errors = new List<string> { "No Animator component found" };
            return false;
        }

        if (!animator.isHuman)
        {
            errors = new List<string> { "Animator is not human" };
            return false;
        }

        foreach (var skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (skinnedMeshRenderer.sharedMesh == null)
            {
                errors = new List<string> { "SkinnedMeshRenderer has no mesh" };
                return false;
            }

            if (skinnedMeshRenderer.sharedMesh.isReadable == false)
            {
                errors = new List<string> { "SkinnedMeshRenderer mesh is not readable" };
                return false;
            }
        }

        errors = new List<string>();

        return true;
    }
}

#endif