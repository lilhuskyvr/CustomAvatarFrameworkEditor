using System.Collections.Generic;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;

public class CustomAvatarMapper : MonoBehaviour
{
    public GameObject baseGameObject;
    public GameObject imitatorGameObject;

    public RuntimeAnimatorController tPoseController;
    public RuntimeAnimatorController skinningTestController;

    [HideInInspector]
    public GameObject sampleBaseGameObject;
    [HideInInspector]
    public GameObject sampleImitatorGameObject;

    [HideInInspector]
    public Animator baseAnimator;
    [HideInInspector]
    public Animator imitatorAnimator;

    [HideInInspector]
    public Animator sampleBaseAnimator;
    [HideInInspector]
    public Animator sampleImitatorAnimator;
    // Start is called before the first frame update

    [TextArea] public string bonesJSON;

    public Dictionary<string, Vector3> bones = new Dictionary<string, Vector3>();

    public bool mapped;

    private void Start()
    {
        baseAnimator = baseGameObject.GetComponent<Animator>();
        imitatorAnimator = imitatorGameObject.GetComponent<Animator>();

        baseAnimator.runtimeAnimatorController = Instantiate(tPoseController);
        imitatorAnimator.runtimeAnimatorController = Instantiate(tPoseController);
        

        sampleBaseGameObject = Instantiate(baseGameObject, transform.position + transform.right, transform.rotation);
        sampleBaseAnimator = sampleBaseGameObject.GetComponent<Animator>();
        sampleBaseAnimator.applyRootMotion = false;

        sampleBaseAnimator.runtimeAnimatorController = Instantiate(skinningTestController);
        sampleImitatorGameObject =
            Instantiate(imitatorGameObject, transform.position - transform.right, transform.rotation);

        sampleImitatorAnimator = sampleImitatorGameObject.GetComponent<Animator>();
        sampleImitatorAnimator.runtimeAnimatorController = null;
        sampleBaseAnimator.applyRootMotion = false;
    }

    [Button("Map")]
    void Calibrate()
    {
        if (!Application.isPlaying)
            return;

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

        // bonesJSON = JsonConvert.SerializeObject(bones, Formatting.Indented);
        mapped = true;
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

            imitatorBone.rotation = baseBone.rotation * Quaternion.Euler(bones[boneMapper.Key].x, bones[boneMapper.Key].y, bones[boneMapper.Key].z);
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
}