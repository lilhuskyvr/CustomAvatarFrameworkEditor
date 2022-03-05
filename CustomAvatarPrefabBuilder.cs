#if UNITY_EDITOR
using System.Collections.Generic;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEditor;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global

public class CustomAvatarPrefabBuilder : MonoBehaviour
{
    public GameObject prefab;
    private int numberOfPrefabs;
    public int maxNumberOfPrefabs;

    [Button]
    public void Build()
    {
        numberOfPrefabs = 0;

        var customAvatarSlots = prefab.GetComponentsInChildren<CustomAvatarSlot>();

        var recipeLists = new Queue<List<CustomAvatarRecipe>>();

        var firstCustomAvatarSlot = customAvatarSlots[0];
        firstCustomAvatarSlot.Init();

        foreach (var customAvatarRecipe in firstCustomAvatarSlot.recipes)
        {
            recipeLists.Enqueue(new List<CustomAvatarRecipe> { customAvatarRecipe });
        }

        if (!firstCustomAvatarSlot.required)
        {
            recipeLists.Enqueue(new List<CustomAvatarRecipe> { null });
        }

        while (recipeLists.Count > 0)
        {
            var recipeList = recipeLists.Dequeue();

            if (recipeList.Count == customAvatarSlots.Length)
            {
                if (numberOfPrefabs < maxNumberOfPrefabs)
                {
                    var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                    //annotate the number
                    instance.name = prefab.name + (numberOfPrefabs + 1);

                    foreach (var recipe in recipeList)
                    {
                        if (recipe == null)
                            continue;

                        recipe.AttachToBody(instance);

                        recipe.ChangeBlendShape(instance);
                    }

                    foreach (var skinnedMeshRenderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        if (skinnedMeshRenderer.sharedMesh != null)
                            continue;
                        Destroy(skinnedMeshRenderer.gameObject);
                    }

                    PrefabUtility.SaveAsPrefabAssetAndConnect(instance,
                        "Assets/Ryan Reos/RyanReos_DaemonGirl_BusinessSuit/GeneratedPrefabs/" + instance.name +
                        ".prefab",
                        InteractionMode.AutomatedAction);

                    instance.transform.position = transform.position + numberOfPrefabs * -transform.right;
                    instance.transform.rotation = transform.rotation;

                    numberOfPrefabs++;
                }

                continue;
            }

            var nextCustomAvatarSlot = customAvatarSlots[recipeList.Count];
            nextCustomAvatarSlot.Init();

            foreach (var customAvatarRecipe in nextCustomAvatarSlot.recipes)
            {
                var newRecipe = new List<CustomAvatarRecipe>(recipeList) { customAvatarRecipe };
                recipeLists.Enqueue(newRecipe);
            }

            if (!nextCustomAvatarSlot.required)
            {
                var newRecipe2 = new List<CustomAvatarRecipe>(recipeList) { null };
                recipeLists.Enqueue(newRecipe2);
            }
        }
    }
}

#endif