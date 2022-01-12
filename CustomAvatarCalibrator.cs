using System.Collections;
using System.Collections.Generic;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;

public class CustomAvatarCalibrator : MonoBehaviour
{
    public GameObject baseGameObject;
    public GameObject imitatorGameObject;
    // Start is called before the first frame update

    [Button("Calibrate")]
    void Calibrate()
    {
        if (imitatorGameObject.transform.localScale != Vector3.one)
        {
            imitatorGameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        var baseAnimator = baseGameObject.GetComponent<Animator>();
        var imitatorAnimator = imitatorGameObject.GetComponent<Animator>();

        var baseArmLength = CalculateArmLength(baseAnimator);
        
        Debug.Log("base arm length" + baseArmLength);
        
        var imitatorArmLength = CalculateArmLength(imitatorAnimator);
        
        var baseHeight = CalculateHeight(baseAnimator);
        var imitatorHeight = CalculateHeight(imitatorAnimator);
        
        var zScale = baseArmLength / imitatorArmLength;

        Debug.Log("base height" + baseHeight);
        var yScale = baseHeight / imitatorHeight;
        
        imitatorGameObject.transform.localScale = new Vector3(zScale, yScale, zScale);
    }

    private float CalculateArmLength(Animator animator)
    {
        var foreArmLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).position,
            animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position
        );
        var armLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position,
            animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position
        );
        
        var shoulderLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position,
            animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position
        );

        return shoulderLength + (foreArmLength + armLength) * 2;
    }
    
    private float CalculateHeight(Animator animator)
    {
        var calfLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftToes).position,
            animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position
        );
        var thighLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position,
            animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position
        );
        
        var torsoLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.Head).position,
            animator.GetBoneTransform(HumanBodyBones.Hips).position
        );

        return torsoLength + (thighLength + calfLength);
    }
}
