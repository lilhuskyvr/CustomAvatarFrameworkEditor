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
        [HideInInspector]
        public CustomAvatarSlot slot;

        public void ChangeBlendShape(GameObject instance)
        {
            if (!changeBlendShapes)
                return;

            if (slot.blendShapeSkinnedMeshRenderer == null)
                return;

            var matchedBlendShapeSkinnedMeshRenderer = instance.GetComponentsInChildren<SkinnedMeshRenderer>()
                .FirstOrDefault(s => s.name == slot.blendShapeSkinnedMeshRenderer.name);
            
            if (matchedBlendShapeSkinnedMeshRenderer == null)
                return;
            
            foreach (var item in blendShapeItems)
            {
                matchedBlendShapeSkinnedMeshRenderer.SetBlendShapeWeight(item.index, item.value);
            }
        }

        public void AttachToBody(GameObject instance)
        {
            if (slot.rootSkinnedMeshRenderer == null)
                return;

            var matchedRootSkinnedMeshRenderer = instance.GetComponentsInChildren<SkinnedMeshRenderer>()
                .FirstOrDefault(s => s.name == slot.rootSkinnedMeshRenderer.name);
            
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