#if UNITY_EDITOR
using System.Collections.Generic;
using CustomAvatarFramework;
using CustomAvatarFramework.Editor.Items;
using Newtonsoft.Json;
using ThunderRoad;
using UniGLTF;
using UnityEditor;
using UnityEngine;

public class CustomAvatarMapper : MonoBehaviour
{
    [HideInInspector] public GameObject baseGameObject;

    public GameObject imitatorGameObject;

    [HideInInspector] public RuntimeAnimatorController tPoseController;
    [HideInInspector] public RuntimeAnimatorController skinningTestController;

    [HideInInspector] public GameObject sampleBaseGameObject;
    [HideInInspector] public GameObject sampleImitatorGameObject;

    [HideInInspector] public Animator baseAnimator;
    [HideInInspector] public Animator imitatorAnimator;

    [HideInInspector] public Animator sampleBaseAnimator;

    [HideInInspector] public Animator sampleImitatorAnimator;
    // Start is called before the first frame update

    [TextArea] public string bonesJSON;

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
                Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CustomAvatarFramework/Resources/BaseMesh.prefab"),
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
        var path = EditorUtility.OpenFilePanel("Select model", Application.dataPath, "prefab");

        if (path == null)
            return;

        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path.ToUnityRelativePath());

        if (go == null)
        {
            EditorUtility.DisplayDialog("Error", "Unable to load file", "OK");
            return;
        }

        imitatorGameObject = Instantiate(go, transform.position, transform.rotation);

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

    public void Calibrate()
    {
        if (!Application.isPlaying)
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
                Instantiate(imitatorGameObject, transform.position + transform.forward - transform.right,
                    transform.rotation);

            sampleImitatorAnimator = sampleImitatorGameObject.GetComponent<Animator>();
            sampleImitatorAnimator.runtimeAnimatorController = null;
            sampleBaseAnimator.applyRootMotion = false;
        }
    }

    public void Build()
    {
        var buildGameObject = Instantiate(imitatorGameObject);
        var buildGameObjectAnimator = buildGameObject.GetComponent<Animator>();

        buildGameObjectAnimator.runtimeAnimatorController = null;

        var itemGameObject = new GameObject();
        var item = itemGameObject.AddComponent<Item>();
        item.rb.isKinematic = true;
        var customAvatar = itemGameObject.AddComponent<CustomAvatar>();
        customAvatar.animator = buildGameObjectAnimator;

        //run map bones
        MapBones(itemGameObject);

        buildGameObject.transform.SetParent(itemGameObject.transform);
        buildGameObject.transform.localPosition = Vector3.zero;
        buildGameObject.transform.localRotation = Quaternion.identity;

        PrefabUtility.SaveAsPrefabAsset(itemGameObject,
            Application.dataPath + "/CustomAvatarFramework/Exports/Kokoro/Kokoro.prefab");
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
        var baseArmLength = CalculateArmLength(baseAnimator);

        var baseHeight = CalculateHeight(baseAnimator);

        var imitatorArmLength = CalculateArmLength(imitatorAnimator);

        var imitatorHeight = CalculateHeight(imitatorAnimator);

        var zScale = baseArmLength / imitatorArmLength;

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