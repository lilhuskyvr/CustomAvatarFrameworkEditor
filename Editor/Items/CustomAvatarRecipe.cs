using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarRecipe : MonoBehaviour
    {
        [HideInInspector] public string slotName;
        public bool changeBlendShapes;

        public List<BlendShapeItem> blendShapeItems = new List<BlendShapeItem>();

        public void ChangeBlendShape(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (!changeBlendShapes)
                return;

            foreach (var item in blendShapeItems)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(item.index, item.value);
            }
        }

        public void AttachToBody(CustomAvatarBody customAvatarBody)
        {
            //dont do anything is body already has the slot
            if (customAvatarBody.filledSlotNames.Contains(slotName))
                return;
            var bodySkinnedMeshRenderer = customAvatarBody.GetComponent<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var newSmr = Instantiate(bodySkinnedMeshRenderer, bodySkinnedMeshRenderer.gameObject.transform.parent);
                newSmr.name = slotName;
                newSmr.sharedMesh = skinnedMeshRenderer.sharedMesh;
                newSmr.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
            }

            //registered slot name
            customAvatarBody.filledSlotNames.Add(slotName);
        }
    }
}