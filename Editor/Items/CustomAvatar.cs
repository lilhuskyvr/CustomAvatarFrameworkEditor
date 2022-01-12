using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatar : MonoBehaviour
    {
        protected GameObject baseMeshGameObject;
        protected Animator baseMeshAnimator;

        public Animator animator;
        
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

        // public void LoadBaseMesh()
        // {
        //     var baseMeshGameObjectPrefab =
        //         AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CustomAvatarFramework/Resources/BaseMesh.prefab");
        //     baseMeshGameObject = Instantiate(baseMeshGameObjectPrefab, transform.position, transform.rotation);
        //     baseMeshAnimator = baseMeshGameObject.GetComponent<Animator>();
        // }
        //
        // public void MapBones()
        // {
        //     transform.position = Vector3.one;
        //     transform.rotation = Quaternion.identity;
        //     transform.localScale = Vector3.one;
        //
        //     if (animator == null)
        //     {
        //         Debug.LogError("Can't map bones. Animator is missing'");
        //         return;
        //     }
        //
        //     if (!animator.isHuman)
        //     {
        //         Debug.LogError("Can't map bones. Avatar isn't humanoid'");
        //         return;
        //     }
        //
        //     foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        //     {
        //         if (!skinnedMeshRenderer.sharedMesh.isReadable)
        //         {
        //             Debug.LogError("SkinnedMeshRenderer is not readable. Please turn on Read/Write enable");
        //             return;
        //         }
        //     }
        //
        //     foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        //     {
        //         skinnedMeshRenderer.updateWhenOffscreen = true;
        //     }
        //
        //     foreach (var boneMapper in boneMappers)
        //     {
        //         if (boneMapper.Value == null)
        //             continue;
        //         GetType().GetField(boneMapper.Key)
        //             .SetValue(this, animator.GetBoneTransform((HumanBodyBones)boneMapper.Value));
        //     }
        //
        //     Debug.Log("Bone Mapping Completed Successfully");
        // }
        //
        // public void GenerateJSONFiles()
        // {
        //     var directories = new Stack<string>();
        //     var templatesPath = Application.dataPath + "/CustomAvatarFramework/Editor/Templates";
        //     var prefabName = gameObject.name;
        //     directories.Push(templatesPath);
        //
        //     var files = new List<string>();
        //
        //     while (directories.Count > 0)
        //     {
        //         var directory = directories.Pop();
        //
        //         foreach (var newDirectory in Directory.GetDirectories(directory))
        //         {
        //             directories.Push(newDirectory);
        //         }
        //
        //         files.AddRange(Directory.GetFiles(directory));
        //     }
        //
        //
        //     var exportsPath = Application.dataPath + "/CustomAvatarFramework/Exports/" + prefabName;
        //
        //     if (!Directory.Exists(exportsPath))
        //         Directory.CreateDirectory(exportsPath);
        //
        //     foreach (var file in files)
        //     {
        //         if (Path.GetExtension(file) == ".json")
        //         {
        //             var newFile = file.Replace(templatesPath, exportsPath);
        //             newFile = newFile.Replace("CAF", prefabName);
        //             var templateContent = File.ReadAllText(file);
        //             var exportContent = templateContent.Replace("CAF", prefabName);
        //             var exportDirectory = Path.GetDirectoryName(newFile);
        //             if (!Directory.Exists(exportDirectory))
        //                 Directory.CreateDirectory(exportDirectory);
        //             File.WriteAllText(newFile, exportContent);
        //         }
        //     }
        // }
        //
        // public void CalibrateCharacter()
        // {
        //     LoadBaseMesh();
        //     ResizeCharacterModel();
        // }
        //
    }
}