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
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global

public class CustomAvatarMassMapper : MonoBehaviour
{
    [HideInInspector] public GameObject baseGameObject;
    [HideInInspector] public GameObject imitatorGameObject;

    public AddressableAssetSettings settings;

    [HideInInspector] public AnimatorController tPoseController;

    [HideInInspector] public Animator baseAnimator;
    [HideInInspector] public Animator imitatorAnimator;

    // Start is called before the first frame update

    [HideInInspector]
    public string bloodDecalMaterialPath = "Assets/CustomAvatarFramework/Resources/BloodDecalMaterial.mat";

    [HideInInspector] public Material bloodDecalMaterial;

    public Dictionary<string, Vector3> bones = new Dictionary<string, Vector3>();
    [HideInInspector] public Vector3 extraDimension = Vector3.zero;

    private string inputDirectoryPath;
    private string outputDirectoryPath;
    public string addressableGroupName { get; set; }


    private void Start()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings != null)
            return;
        EditorUtility.DisplayDialog("Error", "Unable to load settings", "OK");
    }

    private void Init()
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
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(
                        "Assets/CustomAvatarFramework/Resources/TPose.controller"));
        }

        baseAnimator.runtimeAnimatorController = Instantiate(tPoseController);

        //load blood decal material
        if (bloodDecalMaterial == null)
        {
            bloodDecalMaterial = Instantiate(
                AssetDatabase.LoadAssetAtPath<Material>(bloodDecalMaterialPath));
        }
    }

    public void SelectInputFolder()
    {
        inputDirectoryPath =
            EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");

        if (inputDirectoryPath == null)
            return;

        inputDirectoryPath = inputDirectoryPath.ToUnityRelativePath();
    }

    public void SelectOutputFolder()
    {
        outputDirectoryPath =
            EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");

        if (outputDirectoryPath == null)
            return;

        outputDirectoryPath = outputDirectoryPath.ToUnityRelativePath();
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
            EditorUtility.DisplayDialog("Error", type + " game object is null", "OK");
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

        CalculateExtraDimension();

        bones = new Dictionary<string, Vector3>();
        
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
    }

    public void Build()
    {
        StartCoroutine(BuildCoroutine());
    }

    public IEnumerator BuildCoroutine()
    {
        Init();

        addressableGroupName = addressableGroupName != null ? addressableGroupName.Replace(" ", "") : "PackedCAFAsset";

        var sourcePrefabs = LoadSourcePrefabsFromDirectory(inputDirectoryPath);

        yield return TryGenerateWindowsLabel();
        
        foreach (var sourcePrefab in sourcePrefabs)
        {
            //calibrate here first
            imitatorGameObject = Instantiate(sourcePrefab);
            imitatorAnimator = imitatorGameObject.GetComponent<Animator>();
            imitatorAnimator.runtimeAnimatorController = Instantiate(tPoseController);

            //wait for the tpose to be loaded
            yield return new WaitForSeconds(10);

            var creatureId = sourcePrefab.name.Replace(" ", "");

            Calibrate();
            
            var buildGameObject = Instantiate(sourcePrefab);
            var buildGameObjectAnimator = buildGameObject.GetComponent<Animator>();

            buildGameObject.AddCustomAvatarHeads();
            buildGameObject.AddCustomAvatarDynamicBones();
            buildGameObject.AddCustomAvatarDynamicBoneColliders();

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
            
            item.preview.transform.position = buildGameObjectAnimator.GetBoneTransform(HumanBodyBones.Head).position;
            item.preview.transform.localRotation = Quaternion.Euler(0, 180, 0);

            foreach (var skinnedMeshRenderer in buildGameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                skinnedMeshRenderer.updateWhenOffscreen = true;
                
                var customAvatarIgnore = skinnedMeshRenderer.GetComponent<CustomAvatarIgnore>();

                if (customAvatarIgnore != null)
                {
                    if (customAvatarIgnore.ignoreMOES)
                        continue;
                }
                
                skinnedMeshRenderer.AddRevealDecal();

                foreach (var sharedMaterial in skinnedMeshRenderer.sharedMaterials)
                {
                    //moe material 
                    var moeTexture = sharedMaterial.CreateMoeTexture();

                    if (moeTexture == null)
                        continue;
                    MOESConvertWindow.Initialize();
                    MOESConvertWindow.CreateTexture(moeTexture);
                    MOESConvertWindow.ModifyMaterial(moeTexture);

                    moeTexture.material.SetBloodTextures(bloodDecalMaterial);
                }
            }

            var avatarPath = outputDirectoryPath.ToUnityRelativePath() + "/" + creatureId + ".prefab";
            var iconPath = outputDirectoryPath.ToUnityRelativePath() + "/" + creatureId + "Icon.png";

            var builtItem = PrefabUtility.SaveAsPrefabAssetAndConnect(itemGameObject, avatarPath,
                InteractionMode.AutomatedAction);

            //JSON 
            GenerateJsonFiles(creatureId,outputDirectoryPath.AssetPathToFullPath());

            GenerateIcon(item, iconPath);

            AssetDatabase.Refresh();

            var currentAddressableGroup = settings.FindGroup(addressableGroupName);

            if (currentAddressableGroup == null)
            {
                var defaultAddressableGroup = settings.FindGroup("Default");

                currentAddressableGroup = settings.CreateGroup(addressableGroupName, false, false, true,
                    defaultAddressableGroup.Schemas,
                    typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            }

            settings.DefaultGroup = currentAddressableGroup;

            AddAssetToAddressableGroup(iconPath, creatureId + "Icon");
            AddAssetToAddressableGroup(avatarPath, creatureId);

            AssetDatabase.Refresh();
             
            Destroy(imitatorGameObject);
        }
        
        
        //create a common blood decal material
        var newBloodMaterialPath = outputDirectoryPath.ToUnityRelativePath() + "/" + "BloodDecal.mat";
        AssetDatabase.CreateAsset(bloodDecalMaterial, newBloodMaterialPath);
        AddAssetToAddressableGroup(newBloodMaterialPath, "BloodDecalMaterial");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Material>(newBloodMaterialPath);
        
        EditorUtility.DisplayDialog("Complete", "File built successfully", "OK");
        //exit play mode
        EditorApplication.ExecuteMenuItem("Edit/Play");
    } 

    private List<GameObject> LoadSourcePrefabsFromDirectory(string directoryAssetPath)
    {
        var fullPath = directoryAssetPath.AssetPathToFullPath();

        var inputObjectPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.TopDirectoryOnly);

        var sourcePrefabs = new List<GameObject>();

        foreach (var inputObjectPath in inputObjectPaths)
        {
            var sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(inputObjectPath.ToUnityRelativePath());

            if (sourcePrefab == null)
                continue;

            var errors = new List<string>();
            if (!sourcePrefab.CanBeMadeIntoCustomAvatar(out errors))
            {
                continue;
            }

            sourcePrefabs.Add(sourcePrefab);
        }

        return sourcePrefabs;
    }

    public IEnumerator TryGenerateWindowsLabel()
    {
        if (!settings.GetLabels().Contains("Windows"))
        {
            settings.AddLabel("Windows");
        }

        yield return null;
    }

    private void AddAssetToAddressableGroup(string path, string addressName)
    {
        var guid = AssetDatabase.AssetPathToGUID(path);

        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true, false);
        }

        var prefabGuid = AssetDatabase.AssetPathToGUID(path);
        var prefabEntry = settings.FindAssetEntry(prefabGuid);

        if (prefabEntry == null) return;

        entry.SetLabel("Windows", true);
        entry.SetAddress(addressName, false);
    }

    private void GenerateJsonFiles(string creatureId, string path)
    {
        var bonesJson = JsonConvert.SerializeObject(bones, Formatting.Indented);
        var extraDimensionJson = JsonConvert.SerializeObject(extraDimension, Formatting.Indented);
        var directories = new Stack<string>();
        var templatesPath = Application.dataPath + "/CustomAvatarFramework/Editor/Templates/AutoRig";
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


        var exportsPath = path.ToUnityRelativePath() + "/" + creatureId + "JSON";

        if (!Directory.Exists(exportsPath))
            Directory.CreateDirectory(exportsPath);

        foreach (var file in files)
        {
            if (Path.GetExtension(file) == ".json")
            {
                var newFile = file.Replace(templatesPath, exportsPath);
                newFile = newFile.Replace("CAF", creatureId);
                var templateContent = File.ReadAllText(file);
                var exportContent = templateContent.Replace("CAF", creatureId)
                    .Replace("[BONESJSON]", bonesJson)
                    .Replace("[EXTRADIMENSIONJSON]", extraDimensionJson);

                var exportDirectory = Path.GetDirectoryName(newFile);

                if (exportDirectory == null)
                    continue;
                if (!Directory.Exists(exportDirectory))
                    Directory.CreateDirectory(exportDirectory);
                File.WriteAllText(newFile, exportContent);
            }
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

    public ExamineResult ExamineInputGameObject(GameObject inputGameObject)
    {
        var result = new ExamineResult();

        var animator = inputGameObject.GetComponent<Animator>();

        result.hasAnimator = animator != null;

        if (!result.hasAnimator)
            return result;

        result.hasToeBone = animator.GetBoneTransform(HumanBodyBones.LeftToes) != null;

        result.hasFingerBone = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal) != null;

        return result;
    }

    public void CalculateExtraDimension()
    {
        var examineResult = ExamineInputGameObject(imitatorGameObject);

        var baseArmLength = CalculateArmLength(baseAnimator, examineResult);

        var baseHeight = CalculateHeight(baseAnimator, examineResult);

        Debug.Log("Base Height " + baseHeight);

        var imitatorArmLength = CalculateArmLength(imitatorAnimator, examineResult);

        var imitatorHeight = CalculateHeight(imitatorAnimator, examineResult);

        Debug.Log("Imitator Height " + imitatorHeight);

        //avatar width
        var zScale = baseArmLength / imitatorArmLength;

        //avatar height
        var yScale = baseHeight / imitatorHeight;

        extraDimension = new Vector3(zScale, yScale, zScale);
    }

    private float CalculateArmLength(Animator animator, ExamineResult examineResult)
    {
        var handLength = examineResult.hasFingerBone
            ? Vector3.Distance(
                animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).position,
                animator.GetBoneTransform(HumanBodyBones.LeftHand).position
            )
            : 0;

        var foreArmLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftHand).position,
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

        return shoulderLength + (handLength + foreArmLength + armLength) * 2;
    }

    private float CalculateHeight(Animator animator, ExamineResult examineResult)
    {
        var defaultHeight = 1;

        var footLength = examineResult.hasToeBone
            ? Vector3.Distance(
                animator.GetBoneTransform(HumanBodyBones.LeftToes).position,
                animator.GetBoneTransform(HumanBodyBones.LeftFoot).position
            )
            : 0;

        var calfLength = Vector3.Distance(
            animator.GetBoneTransform(HumanBodyBones.LeftFoot).position,
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

        return torsoLength + thighLength + calfLength + footLength;
    }

    public void MapBones(GameObject inputGameObject)
    {
        var customAvatar = inputGameObject.GetComponent<CustomAvatar>();
        var animator = customAvatar.animator;

        foreach (var boneMapper in CustomAvatar.boneMappers)
        {
            customAvatar.GetType().GetField(boneMapper.Key)
                .SetValue(customAvatar, animator.GetBoneTransform(boneMapper.Value));
        }

        Debug.Log("Bone Mapping Completed Successfully");
    }

    [MenuItem("Lil Husky Studio/Custom Avatar Builder")]
    public static void LoadScene()
    {
        EditorApplication.ExecuteMenuItem("Edit/Play");
        EditorSceneManager.LoadSceneInPlayMode("Assets/CustomAvatarFramework/Scenes/LilHuskyBuilder.unity",
            new LoadSceneParameters());
    }

    public void GenerateIcon(Item item, string path)
    {
        var iconResolution = 2048;

        RuntimePreviewGenerator.MarkTextureNonReadable = false;
        RuntimePreviewGenerator.BackgroundColor = new Color(0, 0, 0, 0);

        var generatedIcon =
            RuntimePreviewGenerator.GenerateModelPreview(item.transform, iconResolution, iconResolution, false, false);

        byte[] bytes = generatedIcon.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/RhtQmVh");
    }

    public void OpenPatreon()
    {
        Application.OpenURL("https://www.patreon.com/lilhuskyvr");
    }

    public void OpenDonate()
    {
        Application.OpenURL("https://www.nexusmods.com/Core/Libs/Common/Widgets/PayPalPopUp?user=73414238");
    }
}

#endif