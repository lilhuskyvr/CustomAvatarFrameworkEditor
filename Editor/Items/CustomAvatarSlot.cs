using System.Collections.Generic;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarSlot: MonoBehaviour
    {
        public bool required;
        public CustomAvatarRecipe customAvatarRecipe;
        public SkinnedMeshRenderer rootSkinnedMeshRenderer;
        public SkinnedMeshRenderer blendShapeSkinnedMeshRenderer;

        public List<CustomAvatarRecipe> recipes = new List<CustomAvatarRecipe>();
    }
}