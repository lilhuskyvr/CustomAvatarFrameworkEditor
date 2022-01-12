using System.Collections;
using System.Collections.Generic;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;

public class CustomAvatarMapper : MonoBehaviour
{
    public GameObject baseGameObject;

    public GameObject imitatorGameObject;
    // Start is called before the first frame update

    [Button("Map")]
    void Calibrate()
    {
        var baseAnimator = baseGameObject.GetComponent<Animator>();
        var imitatorAnimator = imitatorGameObject.GetComponent<Animator>();
        
        var customAva

        // foreach (var skeletonBone in baseAnimator.avatar.humanDescription.skeleton)
        // {
        //     if (!skeletonBone.name.Contains("Head"))
        //         return;
        //     Debug.Log(skeletonBone.name);
        //     Debug.Log(skeletonBone.rotation);
        // }

        // imitatorAnimator.avatar.humanDescription.
        // imitatorAnimator.GetBoneTransform(HumanBodyBones.Head).rotation =
        //     baseAnimator.GetBoneTransform(HumanBodyBones.Head).rotation;

    }
    
    // public void LoadBaseMesh()
    // {
    //     baseMeshGameObject = Instantiate(baseMeshGameObject, transform.position, transform.rotation);
    //     baseMeshAnimator = baseMeshGameObject.GetComponent<Animator>();
    // }
    
    // public void CalculateBoneRotations()
    // {
    //     animator.runtimeAnimatorController = Instantiate(baseMeshAnimator.runtimeAnimatorController);
    //
    //     var tPose = baseMeshAnimator.runtimeAnimatorController.animationClips[0];
    //
    //     //trigger tpose
    //     tPose.SampleAnimation(baseMeshGameObject, 0);
    //
    //     tPose.SampleAnimation(animator.gameObject, 0);
    //
    //     var head = animator.GetBoneTransform(HumanBodyBones.Head);
    //     var baseHeadRotation = baseMeshAnimator.GetBoneTransform(HumanBodyBones.Head).rotation;
    //     var originalRotation = head.rotation;
    //
    //
    //     foreach (var boneMapper in boneMappers)
    //     {
    //         if (boneMapper.Value == null)
    //             continue;
    //
    //
    //         var objectBone = GetType().GetField(boneMapper.Key).GetValue(this) as Transform;
    //
    //         if (objectBone == null)
    //         {
    //             GetType().GetField(boneMapper.Key + "ExtraRotation")
    //                 .SetValue(this, Quaternion.identity);
    //             continue;
    //         }
    //
    //         var baseMeshBone = baseMeshAnimator.GetBoneTransform((HumanBodyBones)boneMapper.Value);
    //
    //         var baseMeshBoneRotation = baseMeshBone == null ? Quaternion.identity : baseMeshBone.rotation;
    //             
    //         GetType().GetField(boneMapper.Key + "ExtraRotation")
    //             .SetValue(this, Quaternion.Inverse(objectBone.rotation) * baseMeshBoneRotation );
    //     }
    //         
    //     Debug.Log("Calculate Bone Rotation Successfully");
    // }

}