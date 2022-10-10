#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CustomAvatarFramework.Editor;
using CustomAvatarFramework.Editor.Items;
using Newtonsoft.Json;
using ThunderRoad;
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

public class CustomAvatarMapper : MonoBehaviour
{
    [HideInInspector] public GameObject baseGameObject;
    [HideInInspector] public GameObject selectedImitatorGameObject;
    [HideInInspector] public GameObject imitatorGameObject;

    public string gameObjectName { get; set; }

    public AddressableAssetSettings settings;

    [HideInInspector] public AnimatorController tPoseController;
    [HideInInspector] public AnimatorController skinningTestController;

    [HideInInspector] public GameObject sampleBaseGameObject;
    [HideInInspector] public GameObject sampleImitatorGameObject;

    [HideInInspector] public Animator baseAnimator;
    [HideInInspector] public Animator imitatorAnimator;

    [HideInInspector] public Animator sampleBaseAnimator;

    [HideInInspector] public Animator sampleImitatorAnimator;
    // Start is called before the first frame update

    public string bloodDecalMaterialPath = "Assets/CustomAvatarFramework/Resources/BloodDecalMaterial.mat";

    [HideInInspector] public Material bloodDecalMaterial;

    public Dictionary<string, Vector3> bones = new Dictionary<string, Vector3>();
    public Vector3 extraDimension = Vector3.zero;

    public bool mapped;

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

        if (skinningTestController == null)
        {
            skinningTestController =
                Instantiate(
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(
                        "Assets/CustomAvatarFramework/Resources/SkinningTest.controller"));
        }

        baseAnimator.runtimeAnimatorController = Instantiate(tPoseController);

        //load blood decal material
        if (bloodDecalMaterial == null)
        {
            bloodDecalMaterial = Instantiate(
                AssetDatabase.LoadAssetAtPath<Material>(bloodDecalMaterialPath));
        }
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

        //init here
        Init();
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

        StartCoroutine(BuildCoroutine(path));
    }

    public IEnumerator BuildCoroutine(string path)
    {
        yield return TryGenerateWindowsLabel();

        var buildGameObject = Instantiate(selectedImitatorGameObject);
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
        item.preview.transform.position = buildGameObjectAnimator.GetBoneTransform(HumanBodyBones.Head).position;
        item.preview.transform.localRotation = Quaternion.Euler(0, 180, 0);
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

        var avatarPath = path.ToUnityRelativePath() + "/" + gameObjectName + ".prefab";
        var iconPath = path.ToUnityRelativePath() + "/" + gameObjectName + "Icon.png";

        var builtItem = PrefabUtility.SaveAsPrefabAssetAndConnect(itemGameObject, avatarPath,
            InteractionMode.AutomatedAction);

        //JSON
        GenerateJsonFiles(path);

        GenerateIcon(item, iconPath);

        AssetDatabase.Refresh();

        var currentAddressableGroup = settings.FindGroup(gameObjectName);

        if (currentAddressableGroup == null)
        {
            var defaultAddressableGroup = settings.FindGroup("Default");

            currentAddressableGroup = settings.CreateGroup(gameObjectName, false, false, true,
                defaultAddressableGroup.Schemas,
                typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
        }

        settings.DefaultGroup = currentAddressableGroup;

        AddAssetToAddressableGroup(iconPath, gameObjectName + "Icon");
        AddAssetToAddressableGroup(avatarPath, gameObjectName);

        var newBloodMaterialPath = path.ToUnityRelativePath() + "/" + "BloodDecal.mat";
        AssetDatabase.CreateAsset(bloodDecalMaterial, newBloodMaterialPath);
        AddAssetToAddressableGroup(newBloodMaterialPath, "BloodDecalMaterial");

        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = builtItem;

        EditorUtility.DisplayDialog("Complete", "File built successfully", "OK");
        //exit play mode
        EditorApplication.ExecuteMenuItem("Edit/Play");

        yield return null;
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

    private void GenerateJsonFiles(string path)
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
                var exportContent = templateContent.Replace("CAF", gameObjectName)
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

        sampleImitatorGameObject.transform.localScale = extraDimension;
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