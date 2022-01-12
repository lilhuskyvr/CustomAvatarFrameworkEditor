using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatar : MonoBehaviour
    {
        public GameObject baseMeshGameObjectPrefab;
        protected GameObject baseMeshGameObject;
        protected Animator baseMeshAnimator;

        public Animator animator;

        public Transform Rig_Mesh;
        public Transform Head_Mesh;
        public Transform Neck_Mesh;
        public Transform Jaw_Mesh;
        public Transform Spine1_Mesh;
        public Transform Spine_Mesh;
        public Transform Hips_Mesh;
        public Transform LeftUpLeg_Mesh;
        public Transform LeftLeg_Mesh;
        public Transform LeftFoot_Mesh;
        public Transform LeftToeBase_Mesh;
        public Transform RightUpLeg_Mesh;
        public Transform RightLeg_Mesh;
        public Transform RightFoot_Mesh;
        public Transform RightToeBase_Mesh;
        public Transform LeftShoulder_Mesh;
        public Transform LeftArm_Mesh;
        public Transform LeftForeArm_Mesh;
        public Transform LeftHand_Mesh;
        public Transform RightShoulder_Mesh;
        public Transform RightArm_Mesh;
        public Transform RightForeArm_Mesh;
        public Transform RightHand_Mesh;
        public Transform LeftFingerIndexProximal_Mesh;
        public Transform LeftFingerIndexIntermediate_Mesh;
        public Transform LeftFingerIndexDistal_Mesh;
        public Transform LeftFingerLittleProximal_Mesh;
        public Transform LeftFingerLittleIntermediate_Mesh;
        public Transform LeftFingerLittleDistal_Mesh;
        public Transform LeftFingerMiddleProximal_Mesh;
        public Transform LeftFingerMIddleIntermediate_Mesh;
        public Transform LeftFingerMiddleDistal_Mesh;
        public Transform LeftFingerRingProximal_Mesh;
        public Transform LeftFingerRingIntermediate_Mesh;
        public Transform LeftFingerRingDistal_Mesh;
        public Transform LeftFingerThumbProximal_Mesh;
        public Transform LeftFingerThumbIntermediate_Mesh;
        public Transform LeftFingerThumbDistal_Mesh;
        public Transform RightFingerIndexProximal_Mesh;
        public Transform RightFingerIndexIntermediate_Mesh;
        public Transform RightFingerIndexDistal_Mesh;
        public Transform RightFingerLittleProximal_Mesh;
        public Transform RightFingerLittleIntermediate_Mesh;
        public Transform RightFingerLittleDistal_Mesh;
        public Transform RightFingerMiddleProximal_Mesh;
        public Transform RightFingerMIddleIntermediate_Mesh;
        public Transform RightFingerMiddleDistal_Mesh;
        public Transform RightFingerRingProximal_Mesh;
        public Transform RightFingerRingIntermediate_Mesh;
        public Transform RightFingerRingDistal_Mesh;
        public Transform RightFingerThumbProximal_Mesh;
        public Transform RightFingerThumbIntermediate_Mesh;
        public Transform RightFingerThumbDistal_Mesh;

        public Quaternion Rig_MeshExtraRotation;
        public Quaternion Head_MeshExtraRotation;
        public Quaternion Neck_MeshExtraRotation;
        public Quaternion Jaw_MeshExtraRotation;
        public Quaternion Spine1_MeshExtraRotation;
        public Quaternion Spine_MeshExtraRotation;
        public Quaternion Hips_MeshExtraRotation;
        public Quaternion LeftUpLeg_MeshExtraRotation;
        public Quaternion LeftLeg_MeshExtraRotation;
        public Quaternion LeftFoot_MeshExtraRotation;
        public Quaternion LeftToeBase_MeshExtraRotation;
        public Quaternion RightUpLeg_MeshExtraRotation;
        public Quaternion RightLeg_MeshExtraRotation;
        public Quaternion RightFoot_MeshExtraRotation;
        public Quaternion RightToeBase_MeshExtraRotation;
        public Quaternion LeftShoulder_MeshExtraRotation;
        public Quaternion LeftArm_MeshExtraRotation;
        public Quaternion LeftForeArm_MeshExtraRotation;
        public Quaternion LeftHand_MeshExtraRotation;
        public Quaternion RightShoulder_MeshExtraRotation;
        public Quaternion RightArm_MeshExtraRotation;
        public Quaternion RightForeArm_MeshExtraRotation;
        public Quaternion RightHand_MeshExtraRotation;
        public Quaternion LeftFingerIndexProximal_MeshExtraRotation;
        public Quaternion LeftFingerIndexIntermediate_MeshExtraRotation;
        public Quaternion LeftFingerIndexDistal_MeshExtraRotation;
        public Quaternion LeftFingerLittleProximal_MeshExtraRotation;
        public Quaternion LeftFingerLittleIntermediate_MeshExtraRotation;
        public Quaternion LeftFingerLittleDistal_MeshExtraRotation;
        public Quaternion LeftFingerMiddleProximal_MeshExtraRotation;
        public Quaternion LeftFingerMIddleIntermediate_MeshExtraRotation;
        public Quaternion LeftFingerMiddleDistal_MeshExtraRotation;
        public Quaternion LeftFingerRingProximal_MeshExtraRotation;
        public Quaternion LeftFingerRingIntermediate_MeshExtraRotation;
        public Quaternion LeftFingerRingDistal_MeshExtraRotation;
        public Quaternion LeftFingerThumbProximal_MeshExtraRotation;
        public Quaternion LeftFingerThumbIntermediate_MeshExtraRotation;
        public Quaternion LeftFingerThumbDistal_MeshExtraRotation;
        public Quaternion RightFingerIndexProximal_MeshExtraRotation;
        public Quaternion RightFingerIndexIntermediate_MeshExtraRotation;
        public Quaternion RightFingerIndexDistal_MeshExtraRotation;
        public Quaternion RightFingerLittleProximal_MeshExtraRotation;
        public Quaternion RightFingerLittleIntermediate_MeshExtraRotation;
        public Quaternion RightFingerLittleDistal_MeshExtraRotation;
        public Quaternion RightFingerMiddleProximal_MeshExtraRotation;
        public Quaternion RightFingerMIddleIntermediate_MeshExtraRotation;
        public Quaternion RightFingerMiddleDistal_MeshExtraRotation;
        public Quaternion RightFingerRingProximal_MeshExtraRotation;
        public Quaternion RightFingerRingIntermediate_MeshExtraRotation;
        public Quaternion RightFingerRingDistal_MeshExtraRotation;
        public Quaternion RightFingerThumbProximal_MeshExtraRotation;
        public Quaternion RightFingerThumbIntermediate_MeshExtraRotation;
        public Quaternion RightFingerThumbDistal_MeshExtraRotation;

        public static Dictionary<string, HumanBodyBones> boneMappers = new Dictionary<string, HumanBodyBones>
        {
            { "Hips_Mesh", HumanBodyBones.Hips },
            { "Spine_Mesh", HumanBodyBones.Spine },
            { "Spine1_Mesh", HumanBodyBones.Chest },
            { "Neck_Mesh", HumanBodyBones.Neck },
            { "Head_Mesh", HumanBodyBones.Head },
            { "Jaw_Mesh", HumanBodyBones.Jaw },
            //legs
            { "LeftUpLeg_Mesh", HumanBodyBones.LeftUpperLeg },
            { "LeftLeg_Mesh", HumanBodyBones.LeftLowerLeg },
            { "LeftFoot_Mesh", HumanBodyBones.LeftFoot },
            { "LeftToeBase_Mesh", HumanBodyBones.LeftToes },
            { "RightUpLeg_Mesh", HumanBodyBones.RightUpperLeg },
            { "RightLeg_Mesh", HumanBodyBones.RightLowerLeg },
            { "RightFoot_Mesh", HumanBodyBones.RightFoot },
            { "RightToeBase_Mesh", HumanBodyBones.RightToes },
            //arm
            { "LeftShoulder_Mesh", HumanBodyBones.LeftShoulder },
            { "LeftArm_Mesh", HumanBodyBones.LeftUpperArm },
            { "LeftForeArm_Mesh", HumanBodyBones.LeftLowerArm },
            { "LeftHand_Mesh", HumanBodyBones.LeftHand },
            { "RightShoulder_Mesh", HumanBodyBones.RightShoulder },
            { "RightArm_Mesh", HumanBodyBones.RightUpperArm },
            { "RightForeArm_Mesh", HumanBodyBones.RightLowerArm },
            { "RightHand_Mesh", HumanBodyBones.RightHand },
            //left fingers
            { "LeftFingerIndexProximal_Mesh", HumanBodyBones.LeftIndexProximal },
            { "LeftFingerIndexIntermediate_Mesh", HumanBodyBones.LeftIndexIntermediate },
            { "LeftFingerIndexDistal_Mesh", HumanBodyBones.LeftIndexDistal },
            { "LeftFingerMiddleProximal_Mesh", HumanBodyBones.LeftMiddleProximal },
            { "LeftFingerMIddleIntermediate_Mesh", HumanBodyBones.LeftMiddleIntermediate },
            { "LeftFingerMiddleDistal_Mesh", HumanBodyBones.LeftMiddleDistal },
            { "LeftFingerRingProximal_Mesh", HumanBodyBones.LeftRingProximal },
            { "LeftFingerRingIntermediate_Mesh", HumanBodyBones.LeftRingIntermediate },
            { "LeftFingerRingDistal_Mesh", HumanBodyBones.LeftRingDistal },
            { "LeftFingerLittleProximal_Mesh", HumanBodyBones.LeftLittleProximal },
            { "LeftFingerLittleIntermediate_Mesh", HumanBodyBones.LeftLittleIntermediate },
            { "LeftFingerLittleDistal_Mesh", HumanBodyBones.LeftLittleDistal },
            { "LeftFingerThumbProximal_Mesh", HumanBodyBones.LeftThumbProximal },
            { "LeftFingerThumbIntermediate_Mesh", HumanBodyBones.LeftThumbIntermediate },
            { "LeftFingerThumbDistal_Mesh", HumanBodyBones.LeftThumbDistal },
            //right fingers
            { "RightFingerIndexProximal_Mesh", HumanBodyBones.RightIndexProximal },
            { "RightFingerIndexIntermediate_Mesh", HumanBodyBones.RightIndexIntermediate },
            { "RightFingerIndexDistal_Mesh", HumanBodyBones.RightIndexDistal },
            { "RightFingerMiddleProximal_Mesh", HumanBodyBones.RightMiddleProximal },
            { "RightFingerMIddleIntermediate_Mesh", HumanBodyBones.RightMiddleIntermediate },
            { "RightFingerMiddleDistal_Mesh", HumanBodyBones.RightMiddleDistal },
            { "RightFingerRingProximal_Mesh", HumanBodyBones.RightRingProximal },
            { "RightFingerRingIntermediate_Mesh", HumanBodyBones.RightRingIntermediate },
            { "RightFingerRingDistal_Mesh", HumanBodyBones.RightRingDistal },
            { "RightFingerLittleProximal_Mesh", HumanBodyBones.RightLittleProximal },
            { "RightFingerLittleIntermediate_Mesh", HumanBodyBones.RightLittleIntermediate },
            { "RightFingerLittleDistal_Mesh", HumanBodyBones.RightLittleDistal },
            { "RightFingerThumbProximal_Mesh", HumanBodyBones.RightThumbProximal },
            { "RightFingerThumbIntermediate_Mesh", HumanBodyBones.RightThumbIntermediate },
            { "RightFingerThumbDistal_Mesh", HumanBodyBones.RightThumbDistal },
        };

        public void LoadBaseMesh()
        {
            baseMeshGameObject = Instantiate(baseMeshGameObjectPrefab, transform.position, transform.rotation);
            baseMeshAnimator = baseMeshGameObject.GetComponent<Animator>();
        }

        public void MapBones()
        {
            transform.position = Vector3.one;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            if (animator == null)
            {
                Debug.LogError("Can't map bones. Animator is missing'");
                return;
            }

            if (!animator.isHuman)
            {
                Debug.LogError("Can't map bones. Avatar isn't humanoid'");
                return;
            }

            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (!skinnedMeshRenderer.sharedMesh.isReadable)
                {
                    Debug.LogError("SkinnedMeshRenderer is not readable. Please turn on Read/Write enable");
                    return;
                }
            }

            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                skinnedMeshRenderer.updateWhenOffscreen = true;
            }

            foreach (var boneMapper in boneMappers)
            {
                if (boneMapper.Value == null)
                    continue;
                GetType().GetField(boneMapper.Key)
                    .SetValue(this, animator.GetBoneTransform((HumanBodyBones)boneMapper.Value));
            }

            Debug.Log("Bone Mapping Completed Successfully");
        }

        public void GenerateJSONFiles()
        {
            var directories = new Stack<string>();
            var templatesPath = Application.dataPath + "/CustomAvatarFramework/Editor/Templates";
            var prefabName = gameObject.name;
            directories.Push(templatesPath);

            var files = new List<string>();

            while (directories.Count > 0)
            {
                var directory = directories.Pop();

                foreach (var newDirectory in Directory.GetDirectories(directory))
                {
                    directories.Push(newDirectory);
                }

                files.AddRange(Directory.GetFiles(directory));
            }


            var exportsPath = Application.dataPath + "/CustomAvatarFramework/Exports/" + prefabName;

            if (!Directory.Exists(exportsPath))
                Directory.CreateDirectory(exportsPath);

            foreach (var file in files)
            {
                if (Path.GetExtension(file) == ".json")
                {
                    var newFile = file.Replace(templatesPath, exportsPath);
                    newFile = newFile.Replace("CAF", prefabName);
                    var templateContent = File.ReadAllText(file);
                    var exportContent = templateContent.Replace("CAF", prefabName);
                    var exportDirectory = Path.GetDirectoryName(newFile);
                    if (!Directory.Exists(exportDirectory))
                        Directory.CreateDirectory(exportDirectory);
                    File.WriteAllText(newFile, exportContent);
                }
            }
        }

        public Transform GetRootBone()
        {
            return Rig_Mesh;
        }

        public void CalibrateCharacter()
        {
            LoadBaseMesh();
            ResizeCharacterModel();
            CalculateBoneRotations();
        }

        public void ResizeCharacterModel()
        {
            var baseArmLength = CalculateArmLength(baseMeshAnimator);

            var baseHeight = CalculateHeight(baseMeshAnimator);

            var characterModel = animator.gameObject;

            if (characterModel.transform.localScale != Vector3.one)
            {
                characterModel.transform.localScale = new Vector3(1, 1, 1);
            }

            var imitatorArmLength = CalculateArmLength(animator);

            var imitatorHeight = CalculateHeight(animator);

            var zScale = baseArmLength / imitatorArmLength;

            var yScale = baseHeight / imitatorHeight;

            characterModel.transform.localScale = new Vector3(zScale, yScale, zScale);
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

        public void CalculateBoneRotations()
        {
            animator.runtimeAnimatorController = Instantiate(baseMeshAnimator.runtimeAnimatorController);

            // var tPose = baseMeshAnimator.runtimeAnimatorController.animationClips[0];
            //
            // //trigger tpose
            // tPose.SampleAnimation(baseMeshGameObject, 0);
            //
            // tPose.SampleAnimation(animator.gameObject, 0);

            foreach (var boneMapper in boneMappers)
            {
                GetType().GetField(boneMapper.Key + "ExtraRotation")
                    .SetValue(this, Quaternion.identity);
                
                if (boneMapper.Value == null)
                    continue;

                var objectBone = animator.GetBoneTransform((HumanBodyBones)boneMapper.Value);

                if (objectBone == null)
                    continue;

                var baseMeshBone = baseMeshAnimator.GetBoneTransform((HumanBodyBones)boneMapper.Value);

                if (baseMeshBone == null)
                    continue;

                Debug.Log("Rotating bone: " + boneMapper.Key);

                var extraRotation = Quaternion.Euler((Quaternion.Inverse(objectBone.rotation) * baseMeshBone.rotation).eulerAngles);

                GetType().GetField(boneMapper.Key + "ExtraRotation")
                    .SetValue(this, extraRotation);

                objectBone.rotation = baseMeshBone.rotation;
            }

            // animator.runtimeAnimatorController = null;
            Debug.Log("Calculate Bone Rotation Successfully");
        }
    }
}