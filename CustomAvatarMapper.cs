#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CustomAvatarFramework.Editor;
using CustomAvatarFramework.Editor.Items;
using Newtonsoft.Json;
using ThunderRoad;
using UniGLTF;
using UnityEditor;
using UnityEngine;

public class CustomAvatarMapper : MonoBehaviour
{
    [HideInInspector] public GameObject baseGameObject;
    [HideInInspector] public GameObject selectedImitatorGameObject;
    [HideInInspector] public GameObject imitatorGameObject;

    public string gameObjectName { get; set; }

    [HideInInspector] public RuntimeAnimatorController tPoseController;
    [HideInInspector] public RuntimeAnimatorController skinningTestController;

    [HideInInspector] public GameObject sampleBaseGameObject;
    [HideInInspector] public GameObject sampleImitatorGameObject;

    [HideInInspector] public Animator baseAnimator;
    [HideInInspector] public Animator imitatorAnimator;

    [HideInInspector] public Animator sampleBaseAnimator;

    [HideInInspector] public Animator sampleImitatorAnimator;
    // Start is called before the first frame update

    [HideInInspector] public string bonesJSON;

    public Dictionary<string, Vector3> bones = new Dictionary<string, Vector3>();

    public bool mapped;

    private void Start()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void LoadData()
    {
        if (baseGameObject == null)
        {
            baseGameObject =
                Instantiate(
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CustomAvatarFramework/Resources/BaseMesh.prefab"),
                    transform.position, transform.rotation);
        }

        baseAnimator = baseGameObject.GetComponent<Animator>();

        if (tPoseController == null)
        {
            tPoseController =
                Instantiate(
                    AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                        "Assets/CustomAvatarFramework/Resources/TPose.controller"));
        }

        if (skinningTestController == null)
        {
            skinningTestController =
                Instantiate(
                    AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                        "Assets/CustomAvatarFramework/Resources/SkinningTest.controller"));
        }

        baseAnimator.runtimeAnimatorController = Instantiate(tPoseController);
    }

    public void OpenFile()
    {
        var path = EditorUtility.OpenFilePanel("Select model", Application.dataPath, "prefab,fbx,dae,dxf,obj"); 

        if (path == null)
            return;

        selectedImitatorGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path.ToUnityRelativePath());

        if (selectedImitatorGameObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Unable to load file", "OK");
            return;
        }

        imitatorGameObject = Instantiate(selectedImitatorGameObject, transform.position, transform.rotation);

        imitatorAnimator = imitatorGameObject.GetComponent<Animator>();

        if (imitatorAnimator == null)
        {
            EditorUtility.DisplayDialog("Error", "This mesh doesnt have animator", "OK");
            imitatorGameObject = null;
            return;
        }

        if (!imitatorAnimator.isHuman)
        {
            EditorUtility.DisplayDialog("Error", "This mesh isn't humanoid", "OK");
            imitatorGameObject = null;
            return;
        }

        foreach (var skinnedMeshRenderer in imitatorGameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (!skinnedMeshRenderer.sharedMesh.isReadable)
            {
                EditorUtility.DisplayDialog("Error", "Mesh isn't read/write enable", "OK");
                imitatorGameObject = null;
                return;
            }
        }

        foreach (var skinnedMeshRenderer in imitatorGameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedMeshRenderer.updateWhenOffscreen = true;
        }

        imitatorAnimator.runtimeAnimatorController = Instantiate(tPoseController);

        //Load data here
        LoadData();
    }

    public bool ValidateGameObject(GameObject go, string type)
    {
        if (go == null)
        {
            EditorUtility.DisplayDialog("Error", string.Format("{0} game object is null", type), "OK");
            return false;
        }
        
        if (go == null)
        {
            EditorUtility.DisplayDialog("Error", type +" game object is null", "OK");
            return false;
        }
        return true;
    }
    
    public bool ValidateCalibrate()
    {
        if (baseAnimator == null)
        {
            EditorUtility.DisplayDialog("Error", "base animator is null", "OK");
            return false;
        }
        
        if (imitatorAnimator == null)
        {
            EditorUtility.DisplayDialog("Error", "imitator animator is null", "OK");
            return false;
        }
        
        return true;
    }
    
    public void Calibrate()
    {
        if (!Application.isPlaying)
            return;
        
        if (!ValidateCalibrate())
            return;

        baseGameObject.transform.position = transform.position;
        baseGameObject.transform.rotation = transform.rotation;
        baseGameObject.transform.localScale = Vector3.one;

        imitatorGameObject.transform.position = transform.position;
        imitatorGameObject.transform.rotation = transform.rotation;
        imitatorGameObject.transform.localScale = Vector3.one;

        ResizeCharacterModel();


        foreach (var boneMapper in CustomAvatar.boneMappers)
        {
            var baseBone = baseAnimator.GetBoneTransform(boneMapper.Value);
            var imitatorBone = imitatorAnimator.GetBoneTransform(boneMapper.Value);

            if (baseBone == null || imitatorBone == null)
                continue;

            var eulerAngle = (Quaternion.Inverse(baseBone.rotation.normalized) * imitatorBone.rotation.normalized)
                .eulerAngles;

            eulerAngle.x = FormatRotationAngle(eulerAngle.x);
            eulerAngle.y = FormatRotationAngle(eulerAngle.y);
            eulerAngle.z = FormatRotationAngle(eulerAngle.z);

            bones[boneMapper.Key] = eulerAngle;
        }

        bonesJSON = JsonConvert.SerializeObject(bones, Formatting.Indented);
        mapped = true;

        if (sampleBaseGameObject == null)
        {
            sampleBaseGameObject = Instantiate(baseGameObject, transform.position + transform.forward + transform.right,
                transform.rotation);
            sampleBaseAnimator = sampleBaseGameObject.GetComponent<Animator>();
            sampleBaseAnimator.applyRootMotion = false;

            sampleBaseAnimator.runtimeAnimatorController = Instantiate(skinningTestController);
        }

        if (sampleImitatorGameObject == null)
        {
            sampleImitatorGameObject =
                Instantiate(selectedImitatorGameObject, transform.position + transform.forward - transform.right,
                    transform.rotation);

            sampleImitatorAnimator = sampleImitatorGameObject.GetComponent<Animator>();
            sampleImitatorAnimator.runtimeAnimatorController = null;
            sampleBaseAnimator.applyRootMotion = false;
        }
    }

    public void Build()
    {
        var path = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");

        if (path == null)
            return;

        var buildGameObject = Instantiate(selectedImitatorGameObject);
        var buildGameObjectAnimator = buildGameObject.GetComponent<Animator>();

        buildGameObjectAnimator.runtimeAnimatorController = null;

        var itemGameObject = new GameObject();
        var item = itemGameObject.AddComponent<Item>();
        foreach (var itemCollisionHandler in item.collisionHandlers)
        {
            DestroyImmediate(itemCollisionHandler);
        }

        item.rb.isKinematic = true;
        var customAvatar = itemGameObject.AddComponent<CustomAvatar>();
        customAvatar.animator = buildGameObjectAnimator;

        //run map bones
        MapBones(itemGameObject);

        buildGameObject.transform.SetParent(itemGameObject.transform);
        buildGameObject.transform.localPosition = Vector3.zero;
        buildGameObject.transform.localRotation = Quaternion.identity;
        
                
        foreach (var skinnedMeshRenderer in buildGameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedMeshRenderer.updateWhenOffscreen = true;
        }

        var builtObject = PrefabUtility.SaveAsPrefabAssetAndConnect(itemGameObject,
            path.ToUnityRelativePath() + "/" + gameObjectName + ".prefab",
            InteractionMode.AutomatedAction);

        //JSON
        GenerateJSONFiles(path);

        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = builtObject;
        
        EditorUtility.DisplayDialog("Complete", "File built successfully", "OK");
    }
    
    public void GenerateJSONFiles(string path)
    {
        var directories = new Stack<string>();
        var templatesPath = Application.dataPath + "/CustomAvatarFramework/Editor/Templates/AutoRig";
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
    
    
        var exportsPath = path.ToUnityRelativePath() + "/" + gameObjectName + "JSON";
    
        if (!Directory.Exists(exportsPath))
            Directory.CreateDirectory(exportsPath);
    
        foreach (var file in files)
        {
            if (Path.GetExtension(file) == ".json")
            {
                var newFile = file.Replace(templatesPath, exportsPath);
                newFile = newFile.Replace("CAF", gameObjectName);
                var templateContent = File.ReadAllText(file);
                var exportContent = templateContent.Replace("CAF", gameObjectName).Replace("[BONESJSON]", bonesJSON);;
                var exportDirectory = Path.GetDirectoryName(newFile);
                if (!Directory.Exists(exportDirectory))
                    Directory.CreateDirectory(exportDirectory);
                File.WriteAllText(newFile, exportContent);
            }
        }
    }

    private void Update()
    {
        if (!mapped)
            return;

        foreach (var boneMapper in CustomAvatar.boneMappers)
        {
            var baseBone = sampleBaseAnimator.GetBoneTransform(boneMapper.Value);
            var imitatorBone = sampleImitatorAnimator.GetBoneTransform(boneMapper.Value);

            if (baseBone == null || imitatorBone == null)
                continue;

            imitatorBone.rotation = baseBone.rotation * Quaternion.Euler(bones[boneMapper.Key].x,
                bones[boneMapper.Key].y, bones[boneMapper.Key].z);
        }
    }

    private float FormatRotationAngle(float angle)
    {
        var rs = angle;
        if (rs > 360)
            rs %= 360;

        if (rs > 180)
            rs = rs - 360;

        return rs;
    }

    public void ResizeCharacterModel()
    {
        var baseArmLength = CalculateArmLength(baseAnimator, "base");

        var baseHeight = CalculateHeight(baseAnimator, "base");

        var imitatorArmLength = CalculateArmLength(imitatorAnimator, "imitator");

        var imitatorHeight = CalculateHeight(imitatorAnimator, "imitator");

        //avatar width
        var zScale = baseArmLength / imitatorArmLength;

        //avatar height
        var yScale = baseHeight / imitatorHeight;

        imitatorGameObject.transform.localScale = new Vector3(zScale, yScale, zScale);
        selectedImitatorGameObject.transform.localScale = new Vector3(zScale, yScale, zScale);
    }

    private float CalculateArmLength(Animator animator, string type)
    {
        var defaultArmLength = 1;

        if (animator == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator is null", "OK");
            return defaultArmLength;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left middle distal bone", "OK");
            return defaultArmLength;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftLowerArm) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left lower arm bone", "OK");
            return defaultArmLength;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftUpperArm) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left upper arm bone", "OK");
            return defaultArmLength;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.RightUpperArm) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have right upper arm bone", "OK");
            return defaultArmLength;
        }
        
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

    private float CalculateHeight(Animator animator, string type)
    {

        var defaultHeight = 1;
        
        if (animator == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator is null", "OK");
            return defaultHeight;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftToes) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left toe bone", "OK");
            return defaultHeight;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left lower leg bone", "OK");
            return defaultHeight;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have left upper leg bone", "OK");
            return defaultHeight;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.Head) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have head bone", "OK");
            return defaultHeight;
        }
        
        if (animator.GetBoneTransform(HumanBodyBones.Hips) == null)
        {
            EditorUtility.DisplayDialog("Error", $"{type} animator doesn't have hips bone", "OK");
            return defaultHeight;
        }
        
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

    public void MapBones(GameObject gameObject)
    {
        var customAvatar = gameObject.GetComponent<CustomAvatar>();
        var animator = customAvatar.animator;

        foreach (var boneMapper in CustomAvatar.boneMappers)
        {
            customAvatar.GetType().GetField(boneMapper.Key)
                .SetValue(customAvatar, animator.GetBoneTransform(boneMapper.Value));
        }

        Debug.Log("Bone Mapping Completed Successfully");
    }
}

#endif