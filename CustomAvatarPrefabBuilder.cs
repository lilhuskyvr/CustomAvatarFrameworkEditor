#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomAvatarFramework.Editor;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
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

public class CustomAvatarPrefabBuilder : MonoBehaviour
{
    public GameObject prefab;
    private int numberOfPrefabs;

    [Button]
    public void Build()
    {
        numberOfPrefabs = 0;
        
        var customAvatarSlots = prefab.GetComponentsInChildren<CustomAvatarSlot>();

        var recipeLists = new Queue<List<CustomAvatarRecipe>>();

        var firstCustomAvatarSlot = customAvatarSlots[0];
        firstCustomAvatarSlot.customAvatarRecipe.slotName = firstCustomAvatarSlot.slotName;


        recipeLists.Enqueue(new List<CustomAvatarRecipe> { firstCustomAvatarSlot.customAvatarRecipe });

        if (!firstCustomAvatarSlot.required)
        {
            recipeLists.Enqueue(new List<CustomAvatarRecipe> { null });
        }

        while (recipeLists.Count > 0)
        {
            var recipeList = recipeLists.Dequeue();

            if (recipeList.Count == customAvatarSlots.Length)
            {
                var instance = Instantiate(prefab, transform.position + numberOfPrefabs * 2 * -transform.right,
                    transform.rotation);
                
                //destroy all custom avatar slots
                foreach (var customAvatarSlot in instance.GetComponentsInChildren<CustomAvatarSlot>())
                {
                    DestroyImmediate(customAvatarSlot);
                }

                //annotate the number
                instance.name = prefab.name + (numberOfPrefabs + 1);

                var customAvatarBody = instance.GetComponentInChildren<CustomAvatarBody>();

                foreach (var recipe in recipeList)
                {
                    if (recipe == null)
                        continue;

                    var recipeInstance = Instantiate(recipe, instance.transform);

                    if (!customAvatarBody)
                        continue;

                    recipeInstance.AttachToBody(customAvatarBody);

                    recipeInstance.ChangeBlendShape(customAvatarBody.gameObject.GetComponent<SkinnedMeshRenderer>());
                }

                numberOfPrefabs++;
                continue;
            }

            var nextCustomAvatarSlot = customAvatarSlots[recipeList.Count];
            nextCustomAvatarSlot.customAvatarRecipe.slotName = nextCustomAvatarSlot.slotName;

            var newRecipe = new List<CustomAvatarRecipe>(recipeList);
            newRecipe.Add(nextCustomAvatarSlot.customAvatarRecipe);
            recipeLists.Enqueue(newRecipe);

            if (!nextCustomAvatarSlot.required)
            {
                var newRecipe2 = new List<CustomAvatarRecipe>(recipeList);
                newRecipe2.Add(null);
                recipeLists.Enqueue(newRecipe2);
            }
        }
    }
}

#endif