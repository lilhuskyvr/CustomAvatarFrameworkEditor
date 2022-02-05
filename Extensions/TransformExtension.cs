#if UNITY_EDITOR
using System.Linq;
using UnityEngine;

public static class TransformExtension
{
    public static Transform[] GetImmediateChildTransforms(this Transform transform)
    {
        return transform.GetComponentsInChildren<Transform>().Where(t => t.parent == transform).ToArray();
    }
    
    public static Transform[] GetChildTransforms(this Transform transform)
    {
        return transform.GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
    }

    public static bool HasChildren(this Transform transform)
    {
        return transform.GetComponentsInChildren<Transform>().Length > 1;
    }
}

#endif