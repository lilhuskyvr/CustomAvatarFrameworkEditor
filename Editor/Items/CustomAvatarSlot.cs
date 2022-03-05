using System.Collections.Generic;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarSlot: MonoBehaviour
    {
        public bool required;
        [HideInInspector]
        public SkinnedMeshRenderer rootSkinnedMeshRenderer;
        public SkinnedMeshRenderer blendShapeSkinnedMeshRenderer;

        public List<CustomAvatarRecipe> recipes = new List<CustomAvatarRecipe>();

        public void Init()
        {
            rootSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            foreach (var recipe in recipes)
            {
                recipe.slot = this;
            }
        }
    }
}