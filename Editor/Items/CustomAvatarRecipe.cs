using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarRecipe : MonoBehaviour
    {
        public bool changeBlendShapes;

        public List<BlendShapeItem> blendShapeItems = new List<BlendShapeItem>();
        [HideInInspector] public SkinnedMeshRenderer rootSkinnedMeshRenderer;
        [HideInInspector] public SkinnedMeshRenderer blendShapeSkinnedMeshRenderer;

        public void ChangeBlendShape()
        {
            if (!changeBlendShapes)
                return;

            if (blendShapeSkinnedMeshRenderer == null)
                return;

            foreach (var item in blendShapeItems)
            {
                blendShapeSkinnedMeshRenderer.SetBlendShapeWeight(item.index, item.value);
            }
        }

        public void AttachToBody(GameObject instance)
        {
            if (rootSkinnedMeshRenderer == null)
                return;

            var matchedRootSkinnedMeshRenderer = instance.GetComponentsInChildren<SkinnedMeshRenderer>()
                .FirstOrDefault(s => s.name == rootSkinnedMeshRenderer.name);
            
            if (matchedRootSkinnedMeshRenderer == null)
                return;

            foreach (var skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var newSmr = Instantiate(matchedRootSkinnedMeshRenderer, instance.transform);
                newSmr.name = name;
                newSmr.sharedMesh = skinnedMeshRenderer.sharedMesh;
                newSmr.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
            }
            
            DestroyImmediate(matchedRootSkinnedMeshRenderer.gameObject, true);
        }
    }
}